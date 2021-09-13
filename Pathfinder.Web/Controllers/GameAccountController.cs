using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.Interfaces;
using Pathfinder.Web.Controllers.Base;
using Pathfinder.Application.DTO;

namespace Pathfinder.Web.Controllers
{
    public class GameAccountController: AuthorizedController
    {
        private readonly IAccountService _accountService;
 
        public GameAccountController(IAccountService accountService)
        {
            this._accountService = accountService;
        }

        [HttpGet]
        public async Task<ActionResult<AccountDto>> Get()
        {
            var result = await _accountService.GetCurrentAccountAsync().ConfigureAwait(false);
            return Ok(result);
        }
    }
}