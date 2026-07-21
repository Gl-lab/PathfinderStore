using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IEquipmentRepository
{
    IReadOnlyCollection<EquipmentDefinition> GetAll();
    EquipmentDefinition GetEquipment( string equipmentId );
    IReadOnlyCollection<ClassKitDefinition> GetClassKits();
    ClassKitDefinition GetClassKit( string characterClassId );
}
