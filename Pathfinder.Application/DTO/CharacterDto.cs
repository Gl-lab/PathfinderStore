using Pathfinder.Application.Models.Base;
using System;

namespace Pathfinder.Application.DTO
{
    public class CharacterDto : BaseModel
    {
        public string Name { get; set; }
        public int Balance { get; set; }
        // public virtual ICollection<Backpack> Items { get; set; } = new List<Backpack>();
        // public virtual Race Race { get; set; }
        // public virtual GroupCharacteristic Characteristics { get; set; }
    }
}
