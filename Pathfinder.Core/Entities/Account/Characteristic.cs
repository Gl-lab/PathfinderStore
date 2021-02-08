using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
namespace Pathfinder.Core.Entities.Account
{
    public class Characteristic: Entity
    {
        public int Value { get; set; }
        public int Modifier => (Value - 10)/2;
        
    }
}