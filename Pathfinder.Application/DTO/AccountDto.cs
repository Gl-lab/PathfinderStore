using Pathfinder.Application.Models.Auth.Users;
using Pathfinder.Application.Models.Base;
using System;
using System.Collections.Generic;

namespace Pathfinder.Application.DTO
{
    public class AccountDto : BaseModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<CharacterDto> Characters { get; set; }
        public virtual CharacterDto CurrentCharacter { get; set; }

        public virtual UserModel User { get; set; }
    }
}
