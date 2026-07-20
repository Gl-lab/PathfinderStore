using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Training;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class BackgroundRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsCompleteBaselineCatalog()
    {
        BackgroundRepository repository = new BackgroundRepository();

        IReadOnlyCollection<Background> backgrounds = repository.GetAll();

        Assert.Equal( 35, backgrounds.Count );
        Assert.Equal( backgrounds.Count, backgrounds.Select( background => background.Id ).Distinct().Count() );
        Assert.All( backgrounds, background => Assert.Equal( 2, background.RestrictedBoostOptions.Count ) );
        Assert.All( backgrounds, background => Assert.Equal( 1, background.FreeBoostCount ) );
        Assert.All( backgrounds, background => Assert.Equal( 3, background.Grants.Count ) );
        Assert.All( backgrounds, background =>
        {
            Assert.Equal( background.Grants.Count, background.Grants.Select( grant => grant.Id ).Distinct().Count() );
            Assert.Single( background.Grants, grant => grant.Kind == BackgroundGrantKind.SkillTraining );
            Assert.Single( background.Grants, grant => grant.Kind == BackgroundGrantKind.LoreTraining );
            Assert.Single( background.Grants, grant => grant.Kind == BackgroundGrantKind.SkillFeat );
        } );
    }

    [Fact]
    public void GetBackground_FixedGrants_SeparateGrantAndTargetIds()
    {
        BackgroundRepository repository = new BackgroundRepository();

        Background background = repository.GetBackground( "background.acrobat" );
        BackgroundGrantDescriptor skill = background.Grants
            .Single( grant => grant.Kind == BackgroundGrantKind.SkillTraining );
        BackgroundGrantDescriptor lore = background.Grants
            .Single( grant => grant.Kind == BackgroundGrantKind.LoreTraining );

        Assert.Equal( "background.acrobat.skill", skill.Id );
        Assert.Equal( "skill.acrobatics", skill.TargetId );
        Assert.False( skill.RequiresChoice );
        Assert.Equal( "background.acrobat.lore", lore.Id );
        Assert.Equal( "lore.circus", lore.TargetId );
    }

    [Fact]
    public void GetBackground_FiniteAndOpenChoices_AreExplicit()
    {
        BackgroundRepository repository = new BackgroundRepository();

        Background guard = repository.GetBackground( "background.guard" );
        BackgroundGrantDescriptor guardLore = guard.Grants
            .Single( grant => grant.Kind == BackgroundGrantKind.LoreTraining );
        Background emissary = repository.GetBackground( "background.emissary" );
        BackgroundGrantDescriptor emissaryLore = emissary.Grants
            .Single( grant => grant.Kind == BackgroundGrantKind.LoreTraining );

        Assert.True( guardLore.RequiresChoice );
        Assert.False( guardLore.AllowsCustomLore );
        Assert.Equal( [ "lore.legal", "lore.warfare" ], guardLore.Options.Select( option => option.Id ) );
        Assert.True( emissaryLore.RequiresChoice );
        Assert.True( emissaryLore.AllowsCustomLore );
        Assert.Empty( emissaryLore.Options );
    }

    [Fact]
    public void GetBackground_KnownId_ReturnsBackground()
    {
        BackgroundRepository repository = new BackgroundRepository();

        Background background = repository.GetBackground( "background.acrobat" );

        Assert.Equal( "Acrobat", background.Name );
        Assert.Equal(
            [ AbilityType.Strength, AbilityType.Dexterity ],
            background.RestrictedBoostOptions );
    }

    [Fact]
    public void GetBackground_UnknownId_Throws()
    {
        BackgroundRepository repository = new BackgroundRepository();

        Assert.Throws<ArgumentOutOfRangeException>( () =>
            repository.GetBackground( "background.unknown" ) );
    }

    [Fact]
    public void GetAll_EveryBackgroundTrainingGrantCanBeResolved()
    {
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        SkillRepository skillRepository = new SkillRepository();

        foreach ( Background background in backgroundRepository.GetAll() )
        {
            IReadOnlyList<BackgroundTrainingChoice> choices = background.Grants
                .Where( grant =>
                    grant.RequiresChoice &&
                    ( grant.Kind == BackgroundGrantKind.SkillTraining ||
                      grant.Kind == BackgroundGrantKind.LoreTraining ||
                      grant.Kind == BackgroundGrantKind.SkillFeat ) )
                .Select( grant => grant.AllowsCustomLore
                    ? new BackgroundTrainingChoice( grant.Id, null, "Test Terrain" )
                    : new BackgroundTrainingChoice( grant.Id, grant.Options.First().Id, null ) )
                .ToList();

            BackgroundTrainingResult result = BackgroundTrainingResolver.Resolve(
                background,
                choices,
                skillRepository.GetAll() );

            Assert.Single( result.Skills );
            Assert.Single( result.Lore );
            Assert.False( String.IsNullOrWhiteSpace( result.SkillFeatId ) );
        }
    }
}
