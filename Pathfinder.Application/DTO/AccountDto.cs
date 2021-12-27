using System.Collections.Generic;
using Pathfinder.Application.DTO.Authentication.Users;
using Pathfinder.Application.DTO.Base;

namespace Pathfinder.Application.DTO
{
    public class AccountDto : BaseDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<CharacterDto> Characters { get; set; }
        public virtual CharacterDto CurrentCharacter { get; set; }

        public virtual UserDto User { get; set; }
    }
}