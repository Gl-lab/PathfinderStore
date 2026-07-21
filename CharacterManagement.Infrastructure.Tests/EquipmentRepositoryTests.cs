using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class EquipmentRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsNormalizedMinimumPlayerCoreCatalog()
    {
        EquipmentRepository repository = new EquipmentRepository();

        IReadOnlyCollection<EquipmentDefinition> equipment = repository.GetAll();

        Assert.Equal( 43, equipment.Count );
        Assert.Equal( equipment.Count, equipment.Select( definition => definition.Id ).Distinct().Count() );
        Assert.Equal( 23, equipment.Count( definition => definition.Category == EquipmentCategory.Weapon ) );
        Assert.Equal( 6, equipment.Count( definition => definition.Category == EquipmentCategory.Armor ) );
        Assert.All( equipment, definition => Assert.Equal( "Player Core", definition.Source.Book ) );
    }

    [Theory]
    [InlineData( "equipment.dagger", 20, 1, EquipmentCategory.Weapon )]
    [InlineData( "equipment.studded_leather_armor", 300, 10, EquipmentCategory.Armor )]
    [InlineData( "equipment.arrows", 10, 1, EquipmentCategory.Ammunition )]
    [InlineData( "equipment.adventurers_pack", 150, 10, EquipmentCategory.Gear )]
    public void GetEquipment_ReturnsNormalizedDefinition(
        string equipmentId,
        int expectedPriceCopper,
        int expectedBulkTenths,
        EquipmentCategory expectedCategory )
    {
        EquipmentRepository repository = new EquipmentRepository();

        EquipmentDefinition equipment = repository.GetEquipment( equipmentId );

        Assert.Equal( expectedPriceCopper, equipment.PriceCopper );
        Assert.Equal( expectedBulkTenths, equipment.BulkTenths );
        Assert.Equal( expectedCategory, equipment.Category );
    }

    [Theory]
    [InlineData( "equipment.arrows", "Bow" )]
    [InlineData( "equipment.bolts", "Crossbow" )]
    [InlineData( "equipment.sling_bullets", "Sling" )]
    public void GetEquipment_AmmunitionPreservesCompatibleWeaponGroup(
        string equipmentId,
        string expectedWeaponGroup )
    {
        EquipmentRepository repository = new EquipmentRepository();

        EquipmentDefinition ammunition = repository.GetEquipment( equipmentId );

        Assert.Equal( expectedWeaponGroup, ammunition.Ammunition?.WeaponGroup );
    }

    [Fact]
    public void GetClassKits_ReturnsAllSupportedClassesWithResolvableReferencesAndCorrectPrices()
    {
        EquipmentRepository repository = new EquipmentRepository();
        IReadOnlyDictionary<string, int> expectedPrices = new Dictionary<string, int>
        {
            [ "class.bard" ] = 752,
            [ "class.cleric" ] = 170,
            [ "class.druid" ] = 400,
            [ "class.fighter" ] = 580,
            [ "class.ranger" ] = 370,
            [ "class.rogue" ] = 620,
            [ "class.witch" ] = 182,
            [ "class.wizard" ] = 260,
        };

        IReadOnlyCollection<ClassKitDefinition> kits = repository.GetClassKits();

        Assert.Equal( expectedPrices.Count, kits.Count );
        foreach ( ClassKitDefinition kit in kits )
        {
            int actualPrice = kit.Items.Sum( item =>
                repository.GetEquipment( item.EquipmentId ).PriceCopper * item.PurchaseQuantity );
            Assert.Equal( expectedPrices[ kit.CharacterClassId ], actualPrice );

            IEnumerable<ClassKitItem> optionItems = kit.OptionGroups
                .SelectMany( group => group.Options )
                .SelectMany( option => option.Items );
            Assert.All( optionItems, item => repository.GetEquipment( item.EquipmentId ) );
        }
    }

    [Fact]
    public void RogueKit_IncludesClimbingKitRequiredByPublishedTotal()
    {
        EquipmentRepository repository = new EquipmentRepository();

        ClassKitDefinition rogueKit = repository.GetClassKit( "class.rogue" );

        Assert.Contains( rogueKit.Items, item => item.EquipmentId == "equipment.climbing_kit" );
    }

    [Fact]
    public void ClericKit_ModelsFavoredWeaponAsTypedDependency()
    {
        EquipmentRepository repository = new EquipmentRepository();

        ClassKitDefinition clericKit = repository.GetClassKit( "class.cleric" );
        ClassKitOption favoredWeapon = Assert.Single(
            clericKit.OptionGroups.SelectMany( group => group.Options ),
            option => option.Dependency == ClassKitOptionDependency.DeityFavoredWeapon );

        Assert.Empty( favoredWeapon.Items );
    }

    [Fact]
    public void GetEquipment_UnknownId_Throws()
    {
        EquipmentRepository repository = new EquipmentRepository();

        Assert.Throws<ArgumentOutOfRangeException>( () =>
            repository.GetEquipment( "equipment.unknown" ) );
    }
}
