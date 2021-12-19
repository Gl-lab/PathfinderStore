using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.DTO;
using Pathfinder.Application.DTO.Auth.Account;
using Pathfinder.Application.DTO.Auth;
using Pathfinder.Application.UseCases.Authorization.Account;
using Pathfinder.Core.Entities.Authentication.Permissions;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IMediator _mediator;

        public AccountController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
        public async Task<ActionResult<AccountDto>> Get()
        {
            AccountDto result = await _mediator.Send(new GetCurrentAccountCommand());
            return Ok(result);
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult<LoginOutput>> Login([FromBody] LoginInput input)
        {
            LoginOutput loginOutput =
                await _mediator.Send(new LoginCommand(input.UserNameOrEmail, input.UserNameOrEmail));

            if (loginOutput == null)
            {
                return BadRequest(new List<NameValueDto>
                {
                    new("UserNameOrPasswordIncorrect", "The user name or password is incorrect!")
                });
            }

            return Ok(loginOutput);
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult> Register([FromBody] RegisterInput input)
        {
            var result = await _mediator.Send(new RegisterUserCommand(input.UserName, input.Email, input.Password));
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => new NameValueDto(e.Code, e.Description)).ToList());
            }

            return Ok();
        }

        [HttpPost("/api/[action]")]
        [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordInput input)
        {
            var result = await _mediator.Send(new ChangePasswordCommand(User.Identity.Name, input.CurrentPassword,
                input.NewPassword, input.PasswordRepeat));

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => new NameValueDto(e.Code, e.Description)).ToList());
            }

            return Ok();
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordInput input)
        {
            var result =
                await _mediator.Send(new ResetPasswordCommand(input.UserNameOrEmail, input.Password, input.Token));
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => new NameValueDto(e.Code, e.Description)).ToList());
            }

            return Ok();
        }


        [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> CreateCharacter(CharacterDto newCharacter)
        {
            await _mediator.Send(new CreateCharacterCommand(newCharacter));
            return Ok();
        }

        [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult> DeleteCharacter(int deletedCharacterId)
        {
            await _mediator.Send(new DeleteCharacterCommand(deletedCharacterId));
            return Ok();
        }

        [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
        [HttpPost]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult> SetCurrentCharacter([FromForm] int characterId)
        {
            await _mediator.Send(new SetCurrentCharacterCommand(characterId));
            return Ok();
        }
    }
}