using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.Interfaces;
using Pathfinder.Web.Controllers.Base;
using Pathfinder.Application.DTO;
using Pathfinder.Application.DTO.Items;

namespace Pathfinder.Web.Controllers
{
    public class CharacterController: AuthorizedController
    {
        private readonly ICharacterService characterService;

        public CharacterController(ICharacterService characterService)
        {
            this.characterService = characterService;
        }
        
        [HttpGet]
        public async Task<ActionResult<CharacterDto>> Get()
        {
            try
            {
                return Ok(await characterService.GetCharacterAsync().ConfigureAwait(false));
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
                return Ok(await characterService.GetWeapons());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        /*[HttpPut]
        [Route("items/use")]
        public async Task<ActionResult> ItemUse()
        {
            return Ok();
        }*/
        
        [HttpDelete]
        [Route("items/drop")]
        public async Task<ActionResult> ItemDrop()
        {
            try
            {
                return Ok(await characterService.GetWeapons());
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
                return Ok(await characterService.IncreaseBalance(value));
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
                return Ok(await characterService.DecreaseBalance(value));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}