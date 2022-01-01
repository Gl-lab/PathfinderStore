using Pathfinder.Application.DTO.Base;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Application.DTO
{
    public class CharacterDto : BaseDto
    {
        public string Name { get; set; }

        public int RaceId { get; set; }
        public virtual BackpackDto Backpack { get; set; }
        public virtual Race Race { get; set; }
        public virtual GroupCharacteristicDto Characteristics { get; set; }
    }

    public class BackpackDto : BaseDto
    {
        public WalletDto Wallet { get; init; }
    }

    public class WalletDto : BaseDto
    {
        public int Balance { get; set; }
    }
}