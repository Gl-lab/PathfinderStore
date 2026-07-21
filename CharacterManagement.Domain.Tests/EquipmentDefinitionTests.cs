using Pathfinder.CharacterManagement.Domain.Entity;

namespace CharacterManagement.Domain.Tests;

public sealed class EquipmentDefinitionTests
{
    [Fact]
    public void Constructor_WeaponWithoutWeaponStatistics_Throws()
    {
        Assert.Throws<ArgumentException>( () => new EquipmentDefinition(
            "equipment.invalid",
            "Invalid",
            EquipmentCategory.Weapon,
            EquipmentRarity.Common,
            0,
            0,
            1,
            new SourceReference( "Player Core", 277 ) ) );
    }

    [Fact]
    public void Constructor_NegativePrice_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>( () => new EquipmentDefinition(
            "equipment.invalid",
            "Invalid",
            EquipmentCategory.Gear,
            EquipmentRarity.Common,
            -1,
            0,
            1,
            new SourceReference( "Player Core", 287 ) ) );
    }

    [Fact]
    public void ClassKit_UsesFixedFirstLevelStartingWealth()
    {
        Assert.Equal( 1500, ClassKitDefinition.StartingWealthCopper );
    }
}
