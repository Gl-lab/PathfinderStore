using Pathfinder.CharacterManagement.Application.Equipment;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Equipment;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Infrastructure.Equipment;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;
using CharacterManagement.Infrastructure.Tests.TestSupport;

namespace Pathfinder.CharacterManagement.Infrastructure.Tests;

public sealed class StartingEquipmentAllowedEquipmentReaderTests
{
    [Fact]
    public async Task Read_StartingEquipment_ReturnsApplicationOwnedAllowedState()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        DraftCharacter character = DraftCharacter.Create( 1, "Valeros", AncestryType.Human );
        dbContext.Character.Add( character );
        dbContext.Entry( character )
            .Property( item => item.StartingEquipmentItems )
            .CurrentValue =
            [
                new CharacterEquipmentItem( "equipment.dagger", 1, 1 ),
                new CharacterEquipmentItem( "equipment.leather_armor", 1, 1 ),
            ];
        IReadOnlyList<EffectiveProficiency> proficiencies = ProficiencyResolver.Resolve(
        [
            new ProficiencyGrant(
                ProficiencyTargets.SimpleWeapons,
                ProficiencyRank.Trained,
                "class.test" ),
            new ProficiencyGrant(
                ProficiencyTargets.LightArmor,
                ProficiencyRank.Expert,
                "class.test" ),
        ] );
        StartingEquipmentAllowedEquipmentReader reader = new StartingEquipmentAllowedEquipmentReader(
            new EquipmentRepository() );

        AllowedEquipmentLoadout result = reader.Read( character, proficiencies );

        Assert.Equal( 220, result.TotalPriceCopper );
        Assert.Equal( 1280, result.RemainingWealthCopper );
        Assert.Equal( 11, result.TotalBulkTenths );
        AllowedEquipmentItem dagger = result.Items.Single( item => item.Id == "equipment.dagger" );
        Assert.Equal( ProficiencyRank.Trained, dagger.ProficiencyRank );
        Assert.Equal( 4, dagger.Weapon?.DamageDie );
        Assert.Null( dagger.Armor );
        AllowedEquipmentItem armor = result.Items.Single( item => item.Id == "equipment.leather_armor" );
        Assert.Equal( ProficiencyRank.Expert, armor.ProficiencyRank );
        Assert.Equal( 1, armor.Armor?.ArmorClassBonus );
        Assert.Null( armor.Weapon );
    }
}
