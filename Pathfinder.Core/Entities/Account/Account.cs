using System;
using System.Collections.Generic;
using Pathfinder.Core.Entities.Auth.Users;
using Pathfinder.Core.Entities.Base;

namespace Pathfinder.Core.Entities.Account
{
    public class Account: Entity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid UserId { get; set; }
        public virtual ICollection<Character> Characters { get; set; }

        public virtual User User { get; set; }
    }
}