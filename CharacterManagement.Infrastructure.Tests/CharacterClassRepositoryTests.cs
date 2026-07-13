using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class CharacterClassRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsPlayerCoreBaseline()
    {
        CharacterClassRepository repository = new CharacterClassRepository();

        IReadOnlyCollection<CharacterClass> characterClasses = repository.GetAll();

        Assert.Equal( 8, characterClasses.Count );
        Assert.Equal(
            characterClasses.Count,
            characterClasses.Select( characterClass => characterClass.Id ).Distinct().Count() );
        Assert.All( characterClasses, characterClass => Assert.NotEmpty( characterClass.KeyAbilityOptions ) );
        Assert.All( characterClasses, characterClass => Assert.NotEmpty( characterClass.Rules ) );
    }

    [Fact]
    public void GetCharacterClass_Fighter_ReturnsBothKeyAbilities()
    {
        CharacterClassRepository repository = new CharacterClassRepository();

        CharacterClass fighter = repository.GetCharacterClass( "class.fighter" );

        Assert.Equal( 10, fighter.BaseHitPoints );
        Assert.Equal(
            [ AbilityType.Strength, AbilityType.Dexterity ],
            fighter.KeyAbilityOptions );
    }

    [Fact]
    public void GetCharacterClass_Rogue_DefersRacketKeyAbility()
    {
        CharacterClassRepository repository = new CharacterClassRepository();

        CharacterClass rogue = repository.GetCharacterClass( "class.rogue" );

        Assert.Equal( [ AbilityType.Dexterity ], rogue.KeyAbilityOptions );
        Assert.Contains( CharacterClassDependencyType.RogueRacketCatalog, rogue.DeferredDependencies );
    }

    [Fact]
    public void GetCharacterClass_UnknownId_Throws()
    {
        CharacterClassRepository repository = new CharacterClassRepository();

        Assert.Throws<ArgumentOutOfRangeException>( () =>
            repository.GetCharacterClass( "class.unknown" ) );
    }
}
