using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Pathfinder.Secure.Application.Convertors;
using Pathfinder.Secure.Application.DTO.Authentication.Account;
using Pathfinder.Secure.Application.DTO.Authentication.Roles;
using Pathfinder.Secure.Application.DTO.Authentication.Users;
using Pathfinder.Secure.Application.Repositories;
using Pathfinder.Secure.Domain.Authentication.Role;
using Pathfinder.Secure.Domain.Authentication.User;

namespace Pathfinder.Secure.Application.Services.Authentication;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private User _currentUser;
    private readonly IUserConvertor _userConvertor;
    private readonly IRoleConvertor _roleConvertor;

    public UserService(
        UserManager<User> userManager,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IRoleConvertor roleConvertor,
        IUserConvertor userConvertor )
    {
        _userManager = userManager;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _roleConvertor = roleConvertor;
        _userConvertor = userConvertor;
    }

    // public async Task<IPagedList<UserWithoutPasswordDto>> GetUsersAsync( UserListInput input )
    // {
    //     IQueryable<User>? query = _userManager.Users.Where(
    //         predicate =>
    //             predicate.UserName.Contains(
    //                 input.FilteringOptions[ 0 ].Value as string ) ||
    //             predicate.Email.Contains(
    //                 input.FilteringOptions[ 0 ].Value as string ) );
    //     int usersCount = await query.CountAsync().ConfigureAwait( false );
    //
    //     IEnumerable<UserWithoutPasswordDto> userListOutput =
    //         ( await query.ToArrayAsync().ConfigureAwait( false ) ).Select( x =>
    //             _userConvertor.ConvertWithoutPassword( x ) );
    //     int pageCount = usersCount / input.PageSize;
    //     return new PagedList<UserWithoutPasswordDto>( input.PageIndex, input.PageSize, usersCount, pageCount,
    //         userListOutput );
    // }

    public async Task<GetUserForCreateOrUpdateOutput> GetUserForCreateOrUpdateAsync( int id )
    {
        List<RoleDto> allRoles =
            ( await _roleRepository.GetListAsync().ConfigureAwait( false ) ).Select( x =>
                _roleConvertor.Convert( x ) ).ToList();
        GetUserForCreateOrUpdateOutput getUserForCreateOrUpdateOutput = new GetUserForCreateOrUpdateOutput
        {
            AllRoles = allRoles
        };

        if ( id == 0 )
        {
            return getUserForCreateOrUpdateOutput;
        }

        return await GetUserForCreateOrUpdateOutputAsync( id, allRoles ).ConfigureAwait( false );
    }

    public async Task<IdentityResult> AddUserAsync( CreateOrUpdateUserInput input )
    {
        User user = new User
        {
            Id = input.User.Id,
            UserName = input.User.UserName,
            Email = input.User.Email
        };

        IdentityResult? createUserResult =
            await _userManager.CreateAsync( user, input.User.Password ).ConfigureAwait( false );
        if ( createUserResult.Succeeded )
        {
            await GrantRolesToUserAsync( input.GrantedRoleIds, user ).ConfigureAwait( false );
        }

        return createUserResult;
    }

    public async Task<IdentityResult> EditUserAsync( CreateOrUpdateUserInput input )
    {
        User? user = await _userManager.FindByIdAsync( input.User.Id.ToString() ).ConfigureAwait( false );
        if ( user.UserName == input.User.UserName && user.Id != input.User.Id )
        {
            return IdentityResult.Failed( new IdentityError
            {
                Code = "UserNameAlreadyExist",
                Description = "This user name is already exist!"
            } );
        }

        if ( String.IsNullOrEmpty( input.User.Password ) )
        {
            return await UpdateUser( input, user ).ConfigureAwait( false );
        }

        IdentityResult changePasswordResult =
            await ChangePassword( user, input.User.Password ).ConfigureAwait( false );
        if ( !changePasswordResult.Succeeded )
        {
            return changePasswordResult;
        }

        return await UpdateUser( input, user ).ConfigureAwait( false );
    }

    public async Task<IdentityResult> RemoveUserAsync( int id )
    {
        User? user = await _userManager
                          .Users.FirstOrDefaultAsync( u => u.Id == id )
                          .ConfigureAwait( false );
        if ( user == null )
        {
            return IdentityResult.Failed( new IdentityError
            {
                Code = "UserNotFound",
                Description = "User not found!"
            } );
        }

        if ( DefaultUsers.All().Select( u => u.UserName ).Contains( user.UserName ) )
        {
            return IdentityResult.Failed( new IdentityError()
            {
                Code = "CannotRemoveSystemUser",
                Description = "You cannot remove system user!"
            } );
        }

        IdentityResult? removeUserResult = await _userManager.DeleteAsync( user ).ConfigureAwait( false );
        if ( !removeUserResult.Succeeded )
        {
            return removeUserResult;
        }

        user.UserRoles.Clear();

        return removeUserResult;
    }

    public async Task<IdentityResult> CreateUser( RegisterInput user )
    {
        User applicationUser = new User
        {
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = true
        };

        return await _userManager.CreateAsync( applicationUser, user.Password );
    }

    public async Task<IdentityResult> CreateUser( string userName, string email, string password )
    {
        User applicationUser = new User
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true,
            AccessFailedCount = 5,
        };

        IdentityResult? createUserResult = await _userManager.CreateAsync( applicationUser, password );
        if ( createUserResult.Succeeded )
        {
            await GrantRolesToUserAsync( new List<int> { DefaultRoles.Member.Id }, applicationUser );
        }

        return createUserResult;
    }

    public async Task<User?> FindByEmailAsync( string email )
    {
        return await _userManager.FindByEmailAsync( email );
    }

    public async Task<User?> FindByNameAsync( string name )
    {
        return await _userManager.FindByNameAsync( name );
    }

    public async Task<IdentityResult> ChangePasswordAsync( User user, string currentPassword, string newPassword )
    {
        return await _userManager.ChangePasswordAsync( user, currentPassword, newPassword );
    }

    public async Task<ClaimsIdentity?> CreateClaimsIdentityAsync( string userNameOrEmail, string password )
    {
        if ( String.IsNullOrEmpty( userNameOrEmail ) || String.IsNullOrEmpty( password ) )
        {
            return null;
        }

        User? userToVerify = await FindUserByUserNameOrEmail( userNameOrEmail ).ConfigureAwait( false );

        if ( userToVerify == null )
        {
            return null;
        }

        if ( await _userManager.CheckPasswordAsync( userToVerify, password ) )
        {
            return new ClaimsIdentity( new GenericIdentity( userNameOrEmail, "Token" ), new[]
            {
                new Claim( JwtRegisteredClaimNames.Sub, userNameOrEmail ),
                new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() ),
                new Claim( ClaimTypes.NameIdentifier, userToVerify.Id.ToString() )
            } );
        }

        return null;
    }

    public async Task<IdentityResult> ResetPasswordAsync( User user, string token, string password )
    {
        return await _userManager.ResetPasswordAsync( user, token, password );
    }

    private async Task<User?> FindUserByUserNameOrEmail( string userNameOrEmail )
    {
        return await _userManager.FindByNameAsync( userNameOrEmail ).ConfigureAwait( false ) ??
            await _userManager.FindByEmailAsync( userNameOrEmail ).ConfigureAwait( false );
    }

    public void SetCurrentUser( User user )
    {
        _currentUser = user;
    }

    public async Task SetCurrentUserByLogin( string login )
    {
        _currentUser = await _userManager.FindByNameAsync( login ).ConfigureAwait( false );
    }

    public User GetCurrentUser()
    {
        return _currentUser;
    }

    private async Task GrantRolesToUserAsync( IEnumerable<int> grantedRoleIds, User user )
    {
        List<UserRole> userRoles = grantedRoleIds
                                  .Select( roleId => new UserRole { RoleId = roleId, UserId = user.Id } )
                                  .ToList();
        await _userRoleRepository.AddRangeAsync( userRoles ).ConfigureAwait( false );
    }

    private async Task<IdentityResult> ChangePassword( User user, string password )
    {
        IdentityResult? changePasswordResult =
            await _userManager.RemovePasswordAsync( user ).ConfigureAwait( false );
        if ( changePasswordResult.Succeeded )
        {
            changePasswordResult = await _userManager.AddPasswordAsync( user, password ).ConfigureAwait( false );
        }

        return changePasswordResult;
    }

    private async Task<IdentityResult> UpdateUser( CreateOrUpdateUserInput input, User user )
    {
        user.UserName = input.User.UserName;
        user.Email = input.User.Email;
        user.UserRoles.Clear();
        user.SecurityStamp = Guid.NewGuid().ToString();

        IdentityResult? updateUserResult = await _userManager.UpdateAsync( user ).ConfigureAwait( false );
        if ( updateUserResult.Succeeded )
        {
            await GrantRolesToUserAsync( input.GrantedRoleIds, user ).ConfigureAwait( false );
        }

        return updateUserResult;
    }

    private async Task<GetUserForCreateOrUpdateOutput> GetUserForCreateOrUpdateOutputAsync( int id,
        List<RoleDto> allRoles )
    {
        User? user = await _userManager.FindByIdAsync( id.ToString() ).ConfigureAwait( false );
        UserDto userDto = _userConvertor.ConvertWithPassword( user );
        IEnumerable<Role> grantedRoles = user.UserRoles.Select( ur => ur.Role );

        return new GetUserForCreateOrUpdateOutput
        {
            User = userDto,
            AllRoles = allRoles,
            GrantedRoleIds = grantedRoles.Select( r => r.Id ).ToList()
        };
    }
}