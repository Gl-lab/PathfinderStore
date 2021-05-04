using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
namespace Pathfinder.Core.Entities.Account
{
    public class Size
    {
        public SizeType Id { get; set; }
        public string Name { get; init; }
        
    }

    public enum SizeType: byte
    {
        Tiny,
        Small,
        Medium,
        Large,
        Huge,
        Gargantuan
    }
    
    
}