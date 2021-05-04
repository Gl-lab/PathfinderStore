using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pathfinder.Application.DTO;
using Pathfinder.Application.DTO.Auth.Account;
using Pathfinder.Application.DTO.Auth;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.Entities.Auth.Permissions;
using Pathfinder.Core.Entities.Auth.Users;
using Pathfinder.Web.Authentication;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<User> userManager;
        private readonly JwtTokenConfiguration jwtTokenConfiguration;
        private readonly IConfiguration configuration;
        private readonly ILogger<AccountController> logger;
        private readonly IAccountService accountService;

        public AccountController(
            UserManager<User> userManager,
            IOptions<JwtTokenConfiguration> jwtTokenConfiguration,
            IConfiguration configuration,
            ILogger<AccountController> logger, 
            IAccountService accountService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.logger = logger;
            this.jwtTokenConfiguration = jwtTokenConfiguration.Value;
            this.accountService = accountService;
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
        public async Task<ActionResult<AccountDto>> Get()
        {
            var result = await accountService.GetCurrentAccountAsync().ConfigureAwait(false);
            return Ok(result);
        }
        
        [HttpPost("/api/[action]")]
        public async Task<ActionResult<LoginOutput>> Login([FromBody]LoginInput input)
        {
            var userToVerify = await CreateClaimsIdentityAsync(input.UserNameOrEmail, input.Password).ConfigureAwait(false);
            if (userToVerify == null)
            {
                return BadRequest(new List<NameValueDto>
                {
                    new NameValueDto("UserNameOrPasswordIncorrect", "The user name or password is incorrect!")
                });
            }

            var token = new JwtSecurityToken
            (
                issuer: jwtTokenConfiguration.Issuer,
                audience: jwtTokenConfiguration.Audience,
                claims: userToVerify.Claims,
                notBefore: jwtTokenConfiguration.StartDate,
                expires: jwtTokenConfiguration.EndDate,
                signingCredentials: jwtTokenConfiguration.SigningCredentials
            );

            return Ok(new LoginOutput { Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult> Register([FromBody]RegisterInput input)
        {
            var user = await userManager.FindByEmailAsync(input.Email).ConfigureAwait(false);
            if (user != null)
            {
                return BadRequest(new List<NameValueDto>
                {
                    new NameValueDto("EmailAlreadyExist", "This email already exists!")
                });
            }

            var applicationUser = new User
            {
                UserName = input.UserName,
                Email = input.Email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(applicationUser, input.Password).ConfigureAwait(false);

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
            if (input.NewPassword != input.PasswordRepeat)
            {
                return BadRequest(new List<NameValueDto>
                {
                    new NameValueDto("PasswordsDoesNotMatch", "Passwords doesn't match!")
                });
            }

            var user = await userManager.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);
            var result = await userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => new NameValueDto(e.Code, e.Description)).ToList());
            }

            return Ok();
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult<ForgotPasswordOutput>> ForgotPassword([FromBody] ForgotPasswordInput input)
        {
            var user = await FindUserByUserNameOrEmail(input.UserNameOrEmail).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound(new List<NameValueDto>
                {
                    new NameValueDto("UserNotFound", "User is not found!")
                });
            }

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            var callbackUrl = configuration["App:ClientUrl"] + "/account/reset-password?token=" + resetToken;
            var message = new MailMessage(
                from: configuration["Email:Smtp:Username"],
                to: "alirizaadiyahsi@gmail.com",
                subject: "Reset your password",
                body: $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>"
            )
            {
                IsBodyHtml = true
            };

            logger.LogInformation(Environment.NewLine + Environment.NewLine +
                                   "******************* Reset Password Link *******************" +
                                   Environment.NewLine + Environment.NewLine +
                                   callbackUrl +
                                   Environment.NewLine + Environment.NewLine +
                                   "***********************************************************" +
                                   Environment.NewLine);
            return Ok(new ForgotPasswordOutput { ResetToken = resetToken });
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordInput input)
        {
            var user = await FindUserByUserNameOrEmail(input.UserNameOrEmail).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound(new List<NameValueDto>
                {
                    new NameValueDto("UserNotFound", "User is not found!")
                });
            }

            var result = await userManager.ResetPasswordAsync(user, input.Token, input.Password).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => new NameValueDto(e.Code, e.Description)).ToList());
            }

            return Ok();
        }

        private async Task<ClaimsIdentity> CreateClaimsIdentityAsync(string userNameOrEmail, string password)
        {
            if (string.IsNullOrEmpty(userNameOrEmail) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var userToVerify = await FindUserByUserNameOrEmail(userNameOrEmail).ConfigureAwait(false);

            if (userToVerify == null)
            {
                return null;
            }

            if (await userManager.CheckPasswordAsync(userToVerify, password).ConfigureAwait(false))
            {
                return new ClaimsIdentity(new GenericIdentity(userNameOrEmail, "Token"), new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userNameOrEmail),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userToVerify.Id.ToString())
                });
            }

            return null;
        }

        private async Task<User> FindUserByUserNameOrEmail(string userNameOrEmail)
        {
            return await userManager.FindByNameAsync(userNameOrEmail).ConfigureAwait(false) ??
                   await userManager.FindByEmailAsync(userNameOrEmail).ConfigureAwait(false);
        }

        [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> CreateCharacter(CharacterDto newCharacter)
        {
            await accountService.CreateCharacterAsync(newCharacter).ConfigureAwait(false);
            return Ok();
        }

        [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult> DeleteCharacter(int deletedCharacterId)
        {
            await accountService.DeleteCharacterAsync(deletedCharacterId).ConfigureAwait(false);
            return Ok();
        }
        
        [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
        [HttpPost]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult> SetCurrentCharacter([FromForm]int characterId)
        {
            await accountService.SetCurrentCharacterAsync(characterId).ConfigureAwait(false);
            return Ok();
        }
    }
}