using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Secure.Domain.Authentication.Permissions;
using Pathfinder.Secure.Domain.Authentication.Role;
using Pathfinder.Secure.Domain.Authentication.User;
using Pathfinder.Secure.Infrastructure.Data;
using CharacterManagement.Infrastructure.Tests.TestSupport;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class DefaultUsersCrossContextTests
{
    [Fact]
    public async Task EnsureCreated_DefaultUsersHaveRequiredDataInAllContexts()
    {
        await using SecureDbContext secureDbContext = CreateSecureDbContext();
        await using CharacterManagementDbContext characterManagementDbContext = TestCharacterManagementDbContextFactory.Create();

        await AssertDefaultUser(
            secureDbContext,
            characterManagementDbContext,
            DefaultUsers.Admin,
            DefaultRoles.Admin,
            "System",
            "Administrator",
            DefaultPermissions.All()
                .Select( permission => permission.Id )
                .ToArray() );
        await AssertDefaultUser(
            secureDbContext,
            characterManagementDbContext,
            DefaultUsers.Member,
            DefaultRoles.Member,
            "Test",
            "User",
            [ DefaultPermissions.MemberAccess.Id ] );
    }

    private static SecureDbContext CreateSecureDbContext()
    {
        DbContextOptions<SecureDbContext> options = new DbContextOptionsBuilder<SecureDbContext>()
            .UseInMemoryDatabase( Guid.NewGuid().ToString( "N" ) )
            .Options;
        SecureDbContext dbContext = new SecureDbContext( options );
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    private static async Task AssertDefaultUser(
        SecureDbContext secureDbContext,
        CharacterManagementDbContext characterManagementDbContext,
        User expectedUser,
        Role expectedRole,
        string expectedName,
        string expectedSurname,
        IReadOnlyCollection<int> expectedPermissionIds )
    {
        User user = await secureDbContext.Users
            .AsNoTracking()
            .SingleAsync( candidate => candidate.Id == expectedUser.Id );
        UserRole userRole = await secureDbContext.UserRoles
            .AsNoTracking()
            .SingleAsync( candidate => candidate.UserId == user.Id );
        List<int> permissionIds = await secureDbContext.RolePermission
            .AsNoTracking()
            .Where( candidate => candidate.RoleId == userRole.RoleId )
            .Select( candidate => candidate.PermissionId )
            .OrderBy( permissionId => permissionId )
            .ToListAsync();
        Account account = await characterManagementDbContext.Account
            .AsNoTracking()
            .SingleAsync( candidate => candidate.UserId == user.Id );

        Assert.Equal( expectedUser.UserName, user.UserName );
        Assert.Equal( expectedUser.Email, user.Email );
        Assert.True( user.EmailConfirmed );
        Assert.False( String.IsNullOrWhiteSpace( user.PasswordHash ) );
        Assert.Equal( expectedRole.Id, userRole.RoleId );
        Assert.Equal( expectedPermissionIds.OrderBy( permissionId => permissionId ), permissionIds );
        Assert.Equal( user.Id, account.Id );
        Assert.Equal( user.Id, account.UserId );
        Assert.Equal( expectedName, account.Name );
        Assert.Equal( expectedSurname, account.Surname );
    }
}
