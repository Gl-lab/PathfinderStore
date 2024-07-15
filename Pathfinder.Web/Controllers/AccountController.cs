using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authorization.Authentication.Permissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.UseCases.Account;
using Pathfinder.Web.Controllers.Base;
using Secure.Application.DTO.Authentication;
using Secure.Application.DTO.Authentication.Account;
using Secure.Application.UseCases.Authorization.Account;

namespace Pathfinder.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IMediator _mediator;

        public AccountController(
            IMediator mediator )
        {
            _mediator = mediator;
        }

        // [HttpGet]
        // [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
        // public async Task<ActionResult<AccountDto>> Get()
        // {
        //     AccountDto result = await _mediator.Send(new GetCurrentAccountCommand());
        //     return Ok(result);
        // }

        [HttpPost( "/api/[action]" )]
        public async Task<ActionResult<LoginOutput>> Login( [FromBody] LoginInput input )
        {
            try
            {
                LoginOutput? loginOutput =
                    await _mediator.Send( new LoginCommand( input.UserNameOrEmail, input.Password ) );

                if ( loginOutput is null )
                {
                    return BadRequest( new List<NameValueDto>
                    {
                        new( "UserNameOrPasswordIncorrect", "The user name or password is incorrect!" )
                    } );
                }

                return Ok( loginOutput );
            }
            catch ( Exception e )
            {
                return BadRequest( e.Message );
            }
        }

        [HttpPost( "/api/[action]" )]
        public async Task<ActionResult> Register( [FromBody] RegisterInput input )
        {
            RegisterUserOutput result =
                await _mediator.Send( new RegisterUserCommand( input.UserName, input.Email, input.Password ) );
            if ( !result.IdentityResult.Succeeded || result.UserId is null )
            {
                return BadRequest( result.IdentityResult.Errors.Select( e => new NameValueDto( e.Code, e.Description ) ).ToList() );
            }
            
            await _mediator.Send( new CreateNewAccountCommand( result.UserId.Value ) );
            
            return Ok();
        }

        [HttpPost( "/api/[action]" )]
        [Authorize( Policy = DefaultPermissions.PermissionNameForMemberAccess )]
        public async Task<ActionResult> ChangePassword( [FromBody] ChangePasswordInput input )
        {
            if ( User is not { Identity.Name: not null } )
            {
                return Unauthorized();
            }

            IdentityResult result = await _mediator.Send( new ChangePasswordCommand( User.Identity.Name,
                input.CurrentPassword,
                input.NewPassword, input.PasswordRepeat ) );

            if ( !result.Succeeded )
            {
                return BadRequest( result.Errors.Select( e => new NameValueDto( e.Code, e.Description ) ).ToList() );
            }

            return Ok();
        }

        [HttpPost( "/api/[action]" )]
        public async Task<ActionResult> ResetPassword( [FromBody] ResetPasswordInput input )
        {
            IdentityResult result =
                await _mediator.Send( new ResetPasswordCommand( input.UserNameOrEmail, input.Password, input.Token ) );
            if ( !result.Succeeded )
            {
                return BadRequest( result.Errors.Select( e => new NameValueDto( e.Code, e.Description ) ).ToList() );
            }

            return Ok();
        }

        //
        // [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
        // [HttpPost]
        // [Route("[action]")]
        // public async Task<ActionResult> CreateCharacter(CharacterDto newCharacter)
        // {
        //     await _mediator.Send(new CreateCharacterCommand(newCharacter));
        //     return Ok();
        // }

        // [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
        // [HttpDelete]
        // [Route("[action]")]
        // public async Task<ActionResult> DeleteCharacter(int deletedCharacterId)
        // {
        //     await _mediator.Send(new DeleteCharacterCommand(deletedCharacterId));
        //     return Ok();
        // }
    }
}