using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.DTO;
using Pathfinder.Application.UseCases.Authorization.Account;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    public class GameAccountController : AuthorizedController
    {
        private readonly IMediator _mediator;

        public GameAccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<AccountDto>> Get()
        {
            var result = await _mediator.Send(new GetCurrentAccountCommand());
            return Ok(result);
        }
    }
}