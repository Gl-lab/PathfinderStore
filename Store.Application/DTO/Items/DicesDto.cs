using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.DTO.Items
{
    public class DicesDto
    {
        public int Count { get; set; }
        public virtual DiceType D { get; set; }
    }
}