using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.CharacterManagement.Domain.Rules.Equipment;

namespace Pathfinder.CharacterManagement.Domain.Tests;

public sealed class StartingEquipmentResolverTests
{
    [Fact]
    public void Resolve_BaseAndSelectedOptions_AggregatesReferencesAndComputesPrice()
    {
        IReadOnlyCollection<EquipmentDefinition> catalog =
        [
            Gear( "equipment.pack", 700 ),
            Gear( "equipment.rope", 100 ),
        ];
        ClassKitDefinition kit = Kit(
            [ new ClassKitItem( "equipment.pack", 1 ) ],
            [
                new ClassKitOptionGroup(
                    "extras",
                    ClassKitOptionSelection.Any,
                    [ new ClassKitOption( "rope", "Rope", [ new ClassKitItem( "equipment.rope", 2 ) ] ) ] ),
            ] );

        StartingEquipmentSelection result = StartingEquipmentResolver.Resolve(
            kit,
            catalog,
            [ "rope" ] );

        Assert.Equal( 900, result.TotalPriceCopper );
        Assert.Equal(
            [
                new CharacterEquipmentItem( "equipment.pack", 1 ),
                new CharacterEquipmentItem( "equipment.rope", 2 ),
            ],
            result.Items );
    }

    [Fact]
    public void Resolve_MultipleSelectionsInExclusiveGroup_Throws()
    {
        ClassKitDefinition kit = Kit(
            [ new ClassKitItem( "equipment.pack", 1 ) ],
            [
                new ClassKitOptionGroup(
                    "weapon",
                    ClassKitOptionSelection.AtMostOne,
                    [
                        new ClassKitOption( "one", "One", [] ),
                        new ClassKitOption( "two", "Two", [] ),
                    ] ),
            ] );

        Assert.Throws<CharacterManagementException>( () => StartingEquipmentResolver.Resolve(
            kit,
            [ Gear( "equipment.pack", 100 ) ],
            [ "one", "two" ] ) );
    }

    [Fact]
    public void Resolve_OverStartingWealth_Throws()
    {
        ClassKitDefinition kit = Kit(
            [ new ClassKitItem( "equipment.pack", 1 ) ],
            [] );

        CharacterManagementException exception = Assert.Throws<CharacterManagementException>(
            () => StartingEquipmentResolver.Resolve(
                kit,
                [ Gear( "equipment.pack", ClassKitDefinition.StartingWealthCopper + 1 ) ],
                [] ) );

        Assert.Contains( "exceeding", exception.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Resolve_UnknownOption_Throws()
    {
        ClassKitDefinition kit = Kit(
            [ new ClassKitItem( "equipment.pack", 1 ) ],
            [] );

        Assert.Throws<CharacterManagementException>( () => StartingEquipmentResolver.Resolve(
            kit,
            [ Gear( "equipment.pack", 100 ) ],
            [ "unknown" ] ) );
    }

    private static EquipmentDefinition Gear( string id, int priceCopper )
    {
        return new EquipmentDefinition(
            id,
            id,
            EquipmentCategory.Gear,
            EquipmentRarity.Common,
            priceCopper,
            0,
            1,
            new SourceReference( "Player Core", 1 ) );
    }

    private static ClassKitDefinition Kit(
        IReadOnlyList<ClassKitItem> items,
        IReadOnlyList<ClassKitOptionGroup> groups )
    {
        return new ClassKitDefinition(
            "class_kit.fighter",
            "class.fighter",
            "Fighter's Kit",
            items,
            groups,
            new SourceReference( "Player Core", 1 ) );
    }
}
