using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.ItemCatalog.Domain.Rules;

namespace Pathfinder.ItemCatalog.Domain.Tests;

public sealed class ItemRevisionRulesTests
{
    [Fact]
    public void ShieldUsesIndependentAttackEquipmentDefenseAndDurabilityComponents()
    {
        AttackComponent shieldBash = AttackComponent.Create(
            "Shield bash",
            1,
            DamageDieSize.D4,
            ItemDamageType.Bludgeoning,
            1 );

        ItemRevisionRules rules = ItemRevisionRules.Create(
            ItemCategory.Shield,
            attacks: [ shieldBash ],
            shield: ShieldComponent.Create( 2 ),
            equipment: EquipmentComponent.Create( EquipmentUsage.Held, 1 ),
            durability: DurabilityComponent.Create( 5, 20, 10 ) );

        Assert.Same( shieldBash, Assert.Single( rules.Attacks ) );
        Assert.Equal( 2, rules.Shield?.RaisedArmorClassBonus );
        Assert.Equal( EquipmentUsage.Held, rules.Equipment?.Usage );
        Assert.Equal( 5, rules.Durability?.Hardness );
        Assert.Null( rules.Armor );
    }

    [Fact]
    public void ShieldRulesRejectMissingIndependentComponents()
    {
        Assert.Throws<ItemCatalogException>( () => ItemRevisionRules.Create(
            ItemCategory.Shield,
            shield: ShieldComponent.Create( 2 ),
            equipment: EquipmentComponent.Create( EquipmentUsage.Held, 1 ),
            durability: DurabilityComponent.Create( 5, 20, 10 ) ) );
    }

    [Fact]
    public void PrimaryCategoryDoesNotControlBehaviorComponents()
    {
        ArmorComponent armor = ArmorComponent.Create(
            ArmorCategory.Light,
            1,
            4,
            -1,
            0,
            1 );

        ItemRevisionRules rules = ItemRevisionRules.Create(
            ItemCategory.OtherEquipment,
            armor: armor );

        Assert.Equal( ItemCategory.OtherEquipment, rules.PrimaryCategory );
        Assert.Same( armor, rules.Armor );
    }

    [Fact]
    public void RulesRejectDescriptionWithoutBehaviorComponent()
    {
        Assert.Throws<ItemCatalogException>( () =>
            ItemRevisionRules.Create( ItemCategory.OtherEquipment ) );
    }

    [Fact]
    public void ChargeComponentRejectsActivationCostAboveMaximum()
    {
        Assert.Throws<ItemCatalogException>( () =>
            ChargeComponent.Create( 3, 4, ChargeRecoveryRule.DailyPreparations ) );
    }

    [Fact]
    public void DurabilityRejectsBrokenThresholdAboveMaximumHitPoints()
    {
        Assert.Throws<ItemCatalogException>( () => DurabilityComponent.Create( 5, 20, 21 ) );
    }
}