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
        Assert.All( characterClasses, characterClass => Assert.NotEmpty( characterClass.InitialProficiencies ) );
        Assert.All( characterClasses, characterClass => Assert.Equal(
            characterClass.InitialProficiencies.Count,
            characterClass.InitialProficiencies
                .Select( grant => grant.Target.Id )
                .Distinct( StringComparer.Ordinal )
                .Count() ) );
        Assert.All( characterClasses, characterClass => Assert.NotEmpty( characterClass.Rules ) );
    }

    [Theory]
    [InlineData( "class.bard", 10 )]
    [InlineData( "class.cleric", 8 )]
    [InlineData( "class.druid", 10 )]
    [InlineData( "class.fighter", 13 )]
    [InlineData( "class.ranger", 11 )]
    [InlineData( "class.rogue", 10 )]
    [InlineData( "class.witch", 8 )]
    [InlineData( "class.wizard", 8 )]
    public void GetCharacterClass_ReturnsCompleteInitialProficiencyMatrix(
        string characterClassId,
        int expectedGrantCount )
    {
        CharacterClassRepository repository = new CharacterClassRepository();

        CharacterClass characterClass = repository.GetCharacterClass( characterClassId );

        Assert.Equal( expectedGrantCount, characterClass.InitialProficiencies.Count );
        ProficiencyGrant classDc = Assert.Single( characterClass.InitialProficiencies
            .Where( grant => grant.Target.Category == ProficiencyCategory.ClassDc ) );
        Assert.Equal( ProficiencyRank.Trained, classDc.Rank );
        Assert.Equal( $"{characterClass.Id}.initial_proficiencies", classDc.SourceGrantId );
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
        Assert.Equal(
            ProficiencyRank.Expert,
            GetRank( fighter, ProficiencyTargets.SimpleWeapons.Id ) );
        Assert.Equal(
            ProficiencyRank.Trained,
            GetRank( fighter, ProficiencyTargets.AdvancedWeapons.Id ) );
        Assert.Equal(
            ProficiencyRank.Trained,
            GetRank( fighter, ProficiencyTargets.HeavyArmor.Id ) );
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
    public void GetCharacterClass_WizardHasResolvedChoicesAndDeferredMechanics()
    {
        CharacterClassRepository repository = new CharacterClassRepository();

        CharacterClass wizard = repository.GetCharacterClass( "class.wizard" );

        Assert.DoesNotContain(
            CharacterClassDependencyType.ClassChoiceCatalog,
            wizard.DeferredDependencies );
        Assert.Contains(
            CharacterClassDependencyType.SpellPreparationRules,
            wizard.DeferredDependencies );
        Assert.Contains(
            CharacterClassDependencyType.ItemCatalog,
            wizard.DeferredDependencies );
        Assert.All(
            wizard.Rules.Where( rule => rule.Kind == CharacterClassRuleKind.MandatoryChoice ),
            rule => Assert.DoesNotContain(
                CharacterClassDependencyType.ClassChoiceCatalog,
                rule.DeferredDependencies ) );
    }

    [Fact]
    public void GetAll_AfterPriorityOne_HasNoUnresolvedGenericClassChoices()
    {
        CharacterClassRepository repository = new CharacterClassRepository();

        IReadOnlyCollection<CharacterClass> characterClasses = repository.GetAll();

        Assert.All( characterClasses, characterClass => Assert.DoesNotContain(
            CharacterClassDependencyType.ClassChoiceCatalog,
            characterClass.DeferredDependencies ) );
        Assert.All(
            characterClasses.SelectMany( characterClass => characterClass.Rules ),
            rule => Assert.DoesNotContain(
                CharacterClassDependencyType.ClassChoiceCatalog,
                rule.DeferredDependencies ) );
    }

    [Fact]
    public void GetCharacterClass_UnknownId_Throws()
    {
        CharacterClassRepository repository = new CharacterClassRepository();

        Assert.Throws<ArgumentOutOfRangeException>( () =>
            repository.GetCharacterClass( "class.unknown" ) );
    }

    private static ProficiencyRank GetRank( CharacterClass characterClass, string targetId )
    {
        return Assert.Single( characterClass.InitialProficiencies
                .Where( grant => grant.Target.Id == targetId ) )
            .Rank;
    }
}
