namespace Pathfinder.CharacterManagement.Domain.Rules.Equipment;

public sealed record CharacterRuntimeEquipmentItem(
    Guid ItemInstanceKey,
    bool IsEquipped );
