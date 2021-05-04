using Pathfinder.Application.DTO.Base;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Application.DTO
{
    public class CharacterDto : BaseDto
    {
        public string Name { get; set; }
        public int Balance { get; set; }
        public int RaceId { get; set; }
        // public virtual ICollection<Backpack> Items { get; set; } = new List<Backpack>();
        public virtual Race Race { get; set; }
        public virtual GroupCharacteristicDto Characteristics { get; set; }
    }
}