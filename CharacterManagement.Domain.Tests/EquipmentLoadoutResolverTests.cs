using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.CharacterManagement.Domain.Rules.Equipment;

namespace Pathfinder.CharacterManagement.Domain.Tests;

public sealed class EquipmentLoadoutResolverTests
{
    [Fact]
    public void Resolve_EquippedWeaponAndArmor_MatchesProficienciesAndComputesBulk()
    {
        IReadOnlyCollection<EquipmentDefinition> catalog =
        [
            Weapon( "equipment.dagger", EquipmentWeaponCategory.Simple, 1 ),
            Armor( "equipment.leather_armor", EquipmentArmorCategory.Light, 10 ),
            Gear( "equipment.pack", 20 ),
        ];
        IReadOnlyList<EffectiveProficiency> proficiencies = ProficiencyResolver.Resolve(
        [
            new ProficiencyGrant( ProficiencyTargets.SimpleWeapons, ProficiencyRank.Trained, "class.test" ),
            new ProficiencyGrant( ProficiencyTargets.LightArmor, ProficiencyRank.Expert, "class.test" ),
        ] );

        EquipmentLoadoutResult result = EquipmentLoadoutResolver.Resolve(
            [
                new CharacterEquipmentItem( "equipment.dagger", 1 ),
                new CharacterEquipmentItem( "equipment.leather_armor", 1 ),
                new CharacterEquipmentItem( "equipment.pack", 1 ),
            ],
            catalog,
            [ "equipment.dagger", "equipment.leather_armor" ],
            proficiencies,
            2 );

        Assert.Equal( 31, result.TotalBulkTenths );
        Assert.Equal( 70, result.EncumberedAtBulkTenths );
        Assert.Equal( 120, result.MaximumBulkTenths );
        Assert.False( result.IsEncumbered );
        Assert.Equal( 1, result.Items.Single( item => item.EquipmentId == "equipment.dagger" ).EquippedQuantity );
        Assert.Equal(
            ProficiencyRank.Trained,
            result.Proficiencies.Single( item => item.EquipmentId == "equipment.dagger" ).Rank );
        Assert.Equal(
            ProficiencyRank.Expert,
            result.Proficiencies.Single( item => item.EquipmentId == "equipment.leather_armor" ).Rank );
    }

    [Fact]
    public void Resolve_WeaponProficiency_UsesBestSpecificOrCategoryRank()
    {
        ProficiencyTarget specificTarget = new ProficiencyTarget(
            "proficiency.attack.weapon.rapier",
            "Rapier",
            ProficiencyCategory.Attack );
        IReadOnlyList<EffectiveProficiency> proficiencies = ProficiencyResolver.Resolve(
        [
            new ProficiencyGrant( specificTarget, ProficiencyRank.Trained, "deity.test" ),
            new ProficiencyGrant( ProficiencyTargets.MartialWeapons, ProficiencyRank.Expert, "class.test" ),
        ] );

        EquipmentLoadoutResult result = EquipmentLoadoutResolver.Resolve(
            [ new CharacterEquipmentItem( "equipment.rapier", 1 ) ],
            [ Weapon( "equipment.rapier", EquipmentWeaponCategory.Martial, 10 ) ],
            [ "equipment.rapier" ],
            proficiencies,
            0 );

        EquipmentProficiencyMatch match = Assert.Single( result.Proficiencies );
        Assert.Equal( ProficiencyTargets.MartialWeapons.Id, match.ProficiencyTargetId );
        Assert.Equal( ProficiencyRank.Expert, match.Rank );

        EquipmentLoadoutResult specificOnlyResult = EquipmentLoadoutResolver.Resolve(
            [ new CharacterEquipmentItem( "equipment.rapier", 1 ) ],
            [ Weapon( "equipment.rapier", EquipmentWeaponCategory.Martial, 10 ) ],
            [ "equipment.rapier" ],
            [ new EffectiveProficiency( specificTarget, ProficiencyRank.Trained, [ "deity.test" ] ) ],
            0 );

        Assert.Equal(
            specificTarget.Id,
            Assert.Single( specificOnlyResult.Proficiencies ).ProficiencyTargetId );
    }

    [Fact]
    public void Resolve_TwoArmorItemsOrGearEquipped_Throws()
    {
        IReadOnlyCollection<EquipmentDefinition> catalog =
        [
            Armor( "equipment.leather_armor", EquipmentArmorCategory.Light, 10 ),
            Armor( "equipment.hide_armor", EquipmentArmorCategory.Medium, 20 ),
            Gear( "equipment.pack", 10 ),
        ];
        IReadOnlyList<CharacterEquipmentItem> items = catalog
            .Select( definition => new CharacterEquipmentItem( definition.Id, 1 ) )
            .ToArray();

        Assert.Throws<CharacterManagementException>( () => EquipmentLoadoutResolver.Resolve(
            items,
            catalog,
            [ "equipment.leather_armor", "equipment.hide_armor" ],
            [],
            0 ) );
        Assert.Throws<CharacterManagementException>( () => EquipmentLoadoutResolver.Resolve(
            items,
            catalog,
            [ "equipment.pack" ],
            [],
            0 ) );
    }

    private static EquipmentDefinition Gear( string id, int bulkTenths )
    {
        return new EquipmentDefinition(
            id,
            id,
            EquipmentCategory.Gear,
            EquipmentRarity.Common,
            0,
            bulkTenths,
            1,
            new SourceReference( "Player Core", 1 ) );
    }

    private static EquipmentDefinition Weapon(
        string id,
        EquipmentWeaponCategory category,
        int bulkTenths )
    {
        return new EquipmentDefinition(
            id,
            id,
            EquipmentCategory.Weapon,
            EquipmentRarity.Common,
            0,
            bulkTenths,
            1,
            new SourceReference( "Player Core", 1 ),
            weapon: new EquipmentWeaponStatistics(
                category,
                "Sword",
                EquipmentWeaponType.Melee,
                4,
                "P",
                "1",
                null,
                [] ) );
    }

    private static EquipmentDefinition Armor(
        string id,
        EquipmentArmorCategory category,
        int bulkTenths )
    {
        return new EquipmentDefinition(
            id,
            id,
            EquipmentCategory.Armor,
            EquipmentRarity.Common,
            0,
            bulkTenths,
            1,
            new SourceReference( "Player Core", 1 ),
            armor: new EquipmentArmorStatistics( category, "Armor", 1, 4, 0, 0, 0, [] ) );
    }
}
