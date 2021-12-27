using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.DTO;
using Pathfinder.Application.DTO.Items;
using Pathfinder.Application.UseCases.Authorization.Account;
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
                return Ok(await _mediator.Send(new GetCurrentAccountCommand()));
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
        public async Task<ActionResult> IncreaseBalance(int value)
        {
            try
            {
                return Ok(await _mediator.Send(new IncreaseCharacterBalanceCommand(value)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("DecreaseBalance")]
        public async Task<ActionResult> DecreaseBalance(int value)
        {
            try
            {
                return Ok(await _mediator.Send(new DecreaseCharacterBalanceCommand(value)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}