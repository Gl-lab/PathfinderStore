using System.Collections.Generic;
using Domain.Contracts;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Core.Entities.Account
{
    public class Account: Entity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public IUser User { get; set; }
        public List<Character> Characters { get; set; }
        //public virtual Character CurrentCharacter { get; set; }
        
    }
}