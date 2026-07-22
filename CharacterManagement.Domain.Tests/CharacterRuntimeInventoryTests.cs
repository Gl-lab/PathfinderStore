using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.CharacterManagement.Domain.Rules.Equipment;

namespace CharacterManagement.Domain.Tests;

public sealed class CharacterRuntimeInventoryTests
{
    [Fact]
    public void CompletedCharacterStoresRuntimeInstanceReferencesAndEquippedState()
    {
        DraftCharacter character = CreateCompletedCharacter();
        Guid equippedKey = Guid.NewGuid();
        Guid carriedKey = Guid.NewGuid();

        character.SetRuntimeInventory(
        [
            new CharacterRuntimeEquipmentItem( equippedKey, true ),
            new CharacterRuntimeEquipmentItem( carriedKey, false ),
        ] );

        Assert.True( character.HasRuntimeInventory );
        Assert.Equal( 2, character.RuntimeEquipmentItems.Count );
        Assert.True( character.RuntimeEquipmentItems.Single(
            item => item.ItemInstanceKey == equippedKey ).IsEquipped );
    }

    [Fact]
    public void DraftCharacterCannotReceiveRuntimeInventory()
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human,
            gender: CharacterGender.Male );

        Assert.Throws<CharacterManagementException>( () => character.SetRuntimeInventory( [] ) );
    }

    [Fact]
    public void RuntimeInstanceReferencesMustBeUniqueAndNonEmpty()
    {
        DraftCharacter character = CreateCompletedCharacter();
        Guid instanceKey = Guid.NewGuid();

        Assert.Throws<CharacterManagementException>( () => character.SetRuntimeInventory(
        [
            new CharacterRuntimeEquipmentItem( instanceKey, true ),
            new CharacterRuntimeEquipmentItem( instanceKey, false ),
        ] ) );
        Assert.Throws<CharacterManagementException>( () => character.SetRuntimeInventory(
        [
            new CharacterRuntimeEquipmentItem( Guid.Empty, false ),
        ] ) );
    }

    private static DraftCharacter CreateCompletedCharacter()
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human,
            gender: CharacterGender.Male );
        character.FinalizeCreation( DateTimeOffset.UtcNow );
        return character;
    }
}
