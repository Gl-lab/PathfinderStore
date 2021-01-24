using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Core.Paging;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Models;
using System.Net;
using Pathfinder.Web.Controllers.Base;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Core.Entities.Auth.Users;
using System.Security.Claims;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Interfaces.Auth;

namespace Pathfinder.Web.Controllers
{
    public class GameAccountController: AuthorizedController
    {
        private readonly IAccountService accountService;
 
        public GameAccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpGet]
        public async Task<ActionResult<AccountDto>> Get()
        {
            var result = await accountService.GetCurrentAccountAsync().ConfigureAwait(false);
            return Ok(result);
        }
    }
}