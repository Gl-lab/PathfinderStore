using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.DTO;
using Pathfinder.Application.DTO.Account;
using Pathfinder.Application.DTO.Items;
using Pathfinder.Application.UseCases.Characters;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    public class CharacterController : AuthorizedController
    {
        private readonly IMediator _mediator;

        public CharacterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<CharacterDto>> Get()
        {
            try
            {
                return Ok(await _mediator.Send(new GetCurrentCharacterForAccountCommand()));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("items")]
        public async Task<ActionResult> Items()
        {
            return Ok();
        }

        [HttpGet]
        [Route("items/Weapons")]
        public async Task<ActionResult<ICollection<WeaponItemDto>>> Weapons()
        {
            try
            {
                return Ok(await _mediator.Send(new GetWeaponsForCurrentCharacterCommand()));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("items/drop")]
        public async Task<ActionResult> ItemDrop()
        {
            try
            {
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("IncreaseBalance")]
        public async Task<ActionResult> IncreaseBalance([FromBody] ChangeAccountBalanceDto changeAccountBalanceDto)
        {
            try
            {
                return Ok(await _mediator.Send(new IncreaseCharacterBalanceCommand(changeAccountBalanceDto.Value)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("DecreaseBalance")]
        public async Task<ActionResult> DecreaseBalance([FromBody] ChangeAccountBalanceDto changeAccountBalanceDto)
        {
            try
            {
                return Ok(await _mediator.Send(new DecreaseCharacterBalanceCommand(changeAccountBalanceDto.Value)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}