using System.Collections.Generic;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Core.Entities.Account
{
    public class Characteristic
    {
        public AbilityType AbilityType { get; set; }
        public int Value { get; set; }
        public int Modifier => (Value - 10)/2;
    }
}