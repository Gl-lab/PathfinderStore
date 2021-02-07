using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.Interfaces;
using Pathfinder.Web.Controllers.Base;
using Pathfinder.Application.DTO;

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
            await characterService.GetCurrentCharacterAsync().ConfigureAwait(false);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> CreateCharacter(CharacterDto newCharacter)
        {
            await characterService.CreateCharacterAsync(newCharacter).ConfigureAwait(false);
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteCharacter(int deletedCharacterId)
        {
            await characterService.DeleteCharacterAsync(deletedCharacterId).ConfigureAwait(false);
            return Ok();
        }
    }
}