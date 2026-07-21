using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class EquipmentDtoMapper
{
    public static EquipmentDto Map( EquipmentDefinition equipment )
    {
        ArgumentNullException.ThrowIfNull( equipment );

        return new EquipmentDto
        {
            Id = equipment.Id,
            Name = equipment.Name,
            Category = equipment.Category,
            Rarity = equipment.Rarity,
            PriceCopper = equipment.PriceCopper,
            BulkTenths = equipment.BulkTenths,
            UnitsPerPurchase = equipment.UnitsPerPurchase,
            Source = equipment.Source,
            Weapon = equipment.Weapon,
            Armor = equipment.Armor,
            Shield = equipment.Shield,
            Ammunition = equipment.Ammunition,
        };
    }

    public static ClassKitDto Map( ClassKitDefinition classKit )
    {
        ArgumentNullException.ThrowIfNull( classKit );

        return new ClassKitDto
        {
            Id = classKit.Id,
            CharacterClassId = classKit.CharacterClassId,
            Name = classKit.Name,
            StartingWealthCopper = ClassKitDefinition.StartingWealthCopper,
            Items = classKit.Items,
            OptionGroups = classKit.OptionGroups,
            Source = classKit.Source,
        };
    }
}
