using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public sealed class DraftCharacterConceptTests
{
    [Fact]
    public void Create_WithConcept_TrimsAndStoresConcept()
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Merisiel",
            AncestryType.Elf,
            "  A daring explorer.  " );

        Assert.Equal( "A daring explorer.", character.Concept );
    }

    [Fact]
    public void Create_WithWhitespaceConcept_StoresNull()
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Merisiel",
            AncestryType.Elf,
            "   " );

        Assert.Null( character.Concept );
    }

    [Fact]
    public void Create_WithTooLongConcept_ThrowsDomainException()
    {
        string concept = new( 'a', 1001 );

        Assert.Throws<CharacterManagementException>( () => DraftCharacter.Create(
            1,
            "Merisiel",
            AncestryType.Elf,
            concept ) );
    }

    [Fact]
    public void Create_WithAge_StoresAge()
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Merisiel",
            AncestryType.Elf,
            age: 114 );

        Assert.Equal( 114, character.Age );
    }

    [Fact]
    public void Create_WithNonPositiveAge_ThrowsDomainException()
    {
        Assert.Throws<CharacterManagementException>( () => DraftCharacter.Create(
            1,
            "Merisiel",
            AncestryType.Elf,
            age: 0 ) );
    }
}
