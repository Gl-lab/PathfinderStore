using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class FeatRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsUniqueFirstLevelCatalogAcrossSupportedCategories()
    {
        FeatRepository repository = CreateRepository();

        IReadOnlyCollection<FeatDefinition> feats = repository.GetAll();

        Assert.Equal( 118, feats.Count );
        Assert.Equal( feats.Count, feats.Select( feat => feat.Id ).Distinct( StringComparer.Ordinal ).Count() );
        Assert.All( feats, feat => Assert.Equal( 1, feat.Level ) );
        Assert.Equal( 40, feats.Count( feat => feat.Category == FeatCategory.Ancestry ) );
        Assert.Equal( 31, feats.Count( feat => feat.Category == FeatCategory.Skill ) );
        Assert.Equal( 47, feats.Count( feat => feat.Category == FeatCategory.Class ) );
    }

    [Fact]
    public void GetAll_ContainsEveryExistingAncestryAndBackgroundFeatReference()
    {
        AncestryRepository ancestryRepository = new AncestryRepository();
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        FeatRepository repository = new FeatRepository( ancestryRepository, backgroundRepository );
        IReadOnlySet<string> featIds = repository
            .GetAll()
            .Select( feat => feat.Id )
            .ToHashSet( StringComparer.Ordinal );

        string[] ancestryFeatIds = ancestryRepository
            .GetAll()
            .SelectMany( ancestry => ancestry.AncestryFeats )
            .Select( feat => feat.Id )
            .ToArray();
        string[] backgroundFeatIds = backgroundRepository
            .GetAll()
            .SelectMany( background => background.Grants )
            .Where( grant => grant.Kind == BackgroundGrantKind.SkillFeat )
            .SelectMany( grant => grant.RequiresChoice
                ? grant.Options.Select( option => option.Id )
                : [ grant.TargetId ?? String.Empty ] )
            .ToArray();

        Assert.All( ancestryFeatIds, featId => Assert.Contains( featId, featIds ) );
        Assert.All( backgroundFeatIds, featId => Assert.Contains( featId, featIds ) );
    }

    [Theory]
    [InlineData( "Bard", "feat.bardic_lore", 7 )]
    [InlineData( "Cleric", "feat.domain_initiate", 7 )]
    [InlineData( "Druid", "feat.animal_companion", 9 )]
    [InlineData( "Fighter", "feat.double_slice", 8 )]
    [InlineData( "Ranger", "feat.twin_takedown", 6 )]
    [InlineData( "Rogue", "feat.nimble_dodge", 7 )]
    [InlineData( "Witch", "feat.cackle", 6 )]
    [InlineData( "Wizard", "feat.spellbook_prodigy", 5 )]
    public void GetAll_ContainsPlayerCoreClassOptionsForEverySupportedClass(
        string classTrait,
        string expectedFeatId,
        int expectedCount )
    {
        FeatRepository repository = CreateRepository();

        FeatDefinition[] feats = repository
            .GetAll()
            .Where( feat => feat.Category == FeatCategory.Class )
            .Where( feat => feat.Traits.Contains( classTrait, StringComparer.Ordinal ) )
            .ToArray();

        Assert.Equal( expectedCount, feats.Length );
        Assert.Contains(
            feats,
            feat => feat.Id == expectedFeatId );
    }

    [Fact]
    public void GetFeat_ProvidesPrerequisitesAndTypedDeferredDependencies()
    {
        FeatRepository repository = CreateRepository();

        FeatDefinition spellbookProdigy = repository.GetFeat( "feat.spellbook_prodigy" );

        Assert.Equal( [ "Trained in Arcana." ], spellbookProdigy.Prerequisites );
        Assert.Contains( FeatDependencyType.SkillCatalog, spellbookProdigy.DeferredDependencies );
        Assert.Contains( FeatDependencyType.SpellCatalog, spellbookProdigy.DeferredDependencies );
        Assert.Equal( "Player Core", spellbookProdigy.Source.Book );
    }

    [Fact]
    public void GetFeat_DerivesSkillFeatTrainingPrerequisitesFromBackgroundGrants()
    {
        FeatRepository repository = CreateRepository();

        Assert.Equal(
            [ "Trained in Acrobatics." ],
            repository.GetFeat( "skill_feat.cat_fall" ).Prerequisites );
        Assert.Equal(
            [ "Trained in Athletics." ],
            repository.GetFeat( "skill_feat.quick_jump" ).Prerequisites );
        Assert.Equal(
            [ "Trained in Thievery." ],
            repository.GetFeat( "skill_feat.pickpocket" ).Prerequisites );
        Assert.Contains(
            FeatDependencyType.FeatParameterChoice,
            repository.GetFeat( "skill_feat.assurance" ).DeferredDependencies );
    }

    private static FeatRepository CreateRepository() => new FeatRepository(
        new AncestryRepository(),
        new BackgroundRepository() );
}
