using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
namespace Pathfinder.Core.Entities.Account
{
    public class Characteristic: Entity
    {
        public virtual CharacteristicInfo CharacteristicInfo { get; set; }
        public int Value { get; set; }
        // public int Modifier { get => (Value - 10)/2; }
    }
}