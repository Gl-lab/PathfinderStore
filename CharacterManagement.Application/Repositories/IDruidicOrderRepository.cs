using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IDruidicOrderRepository
{
    IReadOnlyCollection<DruidicOrder> GetAll();
    DruidicOrder GetDruidicOrder( string druidicOrderId );
}
