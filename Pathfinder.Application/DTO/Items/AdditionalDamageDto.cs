using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.DTO.Items
{
    public class AdditionalDamageDto
    {
        public DicesDto Dices { get; set; } 
        public DamageType DamageType { get; set; }
    }
}