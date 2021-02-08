using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
namespace Pathfinder.Core.Entities.Account
{
    public class RaceSize
    {
        public RaceSizeType Id { get; set; }
        public string Name { get; init; }
        
    }

    public enum RaceSizeType
    {
        Tiny,
        Small,
        Medium,
        Large,
        Huge,
        Gargantuan
    }
    
    
}