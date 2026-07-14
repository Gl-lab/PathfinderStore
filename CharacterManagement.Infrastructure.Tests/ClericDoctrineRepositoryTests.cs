using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class ClericDoctrineRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsTwoPlayerCoreDoctrines()
    {
        ClericDoctrineRepository repository = new ClericDoctrineRepository();

        IReadOnlyCollection<ClericDoctrine> doctrines = repository.GetAll();

        Assert.Equal( 2, doctrines.Count );
        Assert.Equal( 2, doctrines.Select( doctrine => doctrine.Id ).Distinct().Count() );
        Assert.Contains( doctrines, doctrine => doctrine.Id == "cleric_doctrine.cloistered" );
        Assert.Contains( doctrines, doctrine => doctrine.Id == "cleric_doctrine.warpriest" );
        Assert.All( doctrines, doctrine => Assert.Equal( "Player Core", doctrine.Source.Book ) );
    }

    [Fact]
    public void GetClericDoctrine_Cloistered_ReturnsImplementedDomainInitiate()
    {
        ClericDoctrineRepository repository = new ClericDoctrineRepository();

        ClericDoctrine doctrine = repository.GetClericDoctrine( "cleric_doctrine.cloistered" );

        Assert.Empty( doctrine.ProficiencyGrants );
        ClericDoctrineEffectDescriptor effect = Assert.Single( doctrine.Effects );
        Assert.Equal( "cleric_doctrine.cloistered.effect.domain_initiate", effect.Id );
        Assert.Empty( effect.DeferredDependencies );
        Assert.Empty( doctrine.DeferredDependencies );
    }

    [Fact]
    public void GetClericDoctrine_Warpriest_ReturnsTypedProficienciesAndDeferredEffects()
    {
        ClericDoctrineRepository repository = new ClericDoctrineRepository();

        ClericDoctrine doctrine = repository.GetClericDoctrine( "cleric_doctrine.warpriest" );

        Assert.Equal( 3, doctrine.ProficiencyGrants.Count );
        Assert.Contains(
            doctrine.ProficiencyGrants,
            grant => grant.Target.Id == ProficiencyTargets.Fortitude.Id &&
                     grant.Rank == ProficiencyRank.Expert );
        Assert.Contains(
            doctrine.ProficiencyGrants,
            grant => grant.Target.Id == ProficiencyTargets.LightArmor.Id );
        Assert.Contains(
            doctrine.ProficiencyGrants,
            grant => grant.Target.Id == ProficiencyTargets.MediumArmor.Id );
        Assert.Contains(
            doctrine.Effects,
            effect => effect.Id == "cleric_doctrine.warpriest.effect.shield_block" );
        Assert.Contains(
            doctrine.Effects,
            effect => effect.Id == "cleric_doctrine.warpriest.effect.deadly_simplicity" );
        Assert.Contains( CharacterClassDependencyType.WeaponCatalog, doctrine.DeferredDependencies );
    }

    [Fact]
    public void ResolveProficiencies_CloisteredUsesBaselineAndWarpriestAppliesDoctrineGrants()
    {
        CharacterClassRepository classRepository = new CharacterClassRepository();
        ClericDoctrineRepository doctrineRepository = new ClericDoctrineRepository();
        CharacterClass cleric = classRepository.GetCharacterClass( "class.cleric" );
        ClericDoctrine cloistered = doctrineRepository.GetClericDoctrine( "cleric_doctrine.cloistered" );
        ClericDoctrine warpriest = doctrineRepository.GetClericDoctrine( "cleric_doctrine.warpriest" );

        IReadOnlyList<EffectiveProficiency> cloisteredProficiencies = ProficiencyResolver.Resolve(
            cleric.InitialProficiencies.Concat( cloistered.ProficiencyGrants ) );
        IReadOnlyList<EffectiveProficiency> warpriestProficiencies = ProficiencyResolver.Resolve(
            cleric.InitialProficiencies.Concat( warpriest.ProficiencyGrants ) );

        EffectiveProficiency cloisteredFortitude = Assert.Single(
            cloisteredProficiencies.Where( item => item.Target.Id == ProficiencyTargets.Fortitude.Id ) );
        Assert.Equal( ProficiencyRank.Trained, cloisteredFortitude.Rank );
        Assert.DoesNotContain(
            cloisteredProficiencies,
            item => item.Target.Id == ProficiencyTargets.LightArmor.Id );
        Assert.DoesNotContain(
            cloisteredProficiencies,
            item => item.Target.Id == ProficiencyTargets.MediumArmor.Id );

        EffectiveProficiency warpriestFortitude = Assert.Single(
            warpriestProficiencies.Where( item => item.Target.Id == ProficiencyTargets.Fortitude.Id ) );
        Assert.Equal( ProficiencyRank.Expert, warpriestFortitude.Rank );
        Assert.Equal( 2, warpriestFortitude.SourceGrantIds.Count );
        Assert.Contains(
            warpriestProficiencies,
            item => item.Target.Id == ProficiencyTargets.LightArmor.Id &&
                    item.Rank == ProficiencyRank.Trained );
        Assert.Contains(
            warpriestProficiencies,
            item => item.Target.Id == ProficiencyTargets.MediumArmor.Id &&
                    item.Rank == ProficiencyRank.Trained );
        Assert.Equal(
            warpriestProficiencies.Count,
            warpriestProficiencies.Select( item => item.Target.Id ).Distinct().Count() );
    }
}
