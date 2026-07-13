using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class DeityRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsTwentyTwoPlayerCoreEntriesWithOneIneligibleFaith()
    {
        DeityRepository repository = new DeityRepository();

        IReadOnlyCollection<Deity> deities = repository.GetAll();

        Assert.Equal( 22, deities.Count );
        Assert.Equal( 22, deities.Select( deity => deity.Id ).Distinct().Count() );
        Assert.Equal( 21, deities.Count( deity => deity.CanGrantClericPowers ) );
        Assert.False( repository.GetDeity( "deity.atheism" ).CanGrantClericPowers );
        Assert.True( repository.GetDeity( "deity.green_faith" ).CanGrantClericPowers );
        Assert.All( deities, deity => Assert.Equal( "Player Core", deity.Source.Book ) );
    }

    [Fact]
    public void GetDeity_Iomedae_ReturnsRequiredSanctificationAndFavoredWeapon()
    {
        Deity deity = new DeityRepository().GetDeity( "deity.iomedae" );

        Assert.Equal( "skill.intimidation", deity.DivineSkillId );
        Assert.Equal( DivineSanctification.Holy, deity.RequiredSanctification );
        DeityFavoredWeapon weapon = Assert.Single( deity.FavoredWeapons );
        Assert.Equal( "weapon.longsword", weapon.Id );
        Assert.Equal( FavoredWeaponCategory.Martial, weapon.Category );
        Assert.Equal( ProficiencyRank.Trained, Assert.Single( deity.ProficiencyGrants ).Rank );
    }

    [Fact]
    public void GetDeity_Nethys_ReturnsOneGrantedSpellPerRank()
    {
        Deity deity = new DeityRepository().GetDeity( "deity.nethys" );

        Assert.Equal( 9, deity.GrantedSpells.Count );
        Assert.Equal( Enumerable.Range( 1, 9 ), deity.GrantedSpells.Select( spell => spell.Rank ) );
    }
}
