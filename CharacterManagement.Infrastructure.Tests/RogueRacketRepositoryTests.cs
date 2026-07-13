using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class RogueRacketRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsFourPlayerCoreRackets()
    {
        RogueRacketRepository repository = new RogueRacketRepository();

        IReadOnlyCollection<RogueRacket> rackets = repository.GetAll();

        Assert.Equal( 4, rackets.Count );
        Assert.Equal( 4, rackets.Select( racket => racket.Id ).Distinct().Count() );
        Assert.Contains( rackets, racket => racket.Id == "rogue_racket.mastermind" );
        Assert.Contains( rackets, racket => racket.Id == "rogue_racket.ruffian" );
        Assert.Contains( rackets, racket => racket.Id == "rogue_racket.scoundrel" );
        Assert.Contains( rackets, racket => racket.Id == "rogue_racket.thief" );
    }

    [Fact]
    public void GetRogueRacket_Ruffian_ReturnsSkillKeyAbilityAndArmorGrant()
    {
        RogueRacketRepository repository = new RogueRacketRepository();

        RogueRacket ruffian = repository.GetRogueRacket( "rogue_racket.ruffian" );

        Assert.Equal( AbilityType.Strength, ruffian.AlternativeKeyAbility );
        Assert.Equal( "skill.intimidation", Assert.Single( ruffian.SkillGrants ).TargetId );
        ProficiencyGrant proficiency = Assert.Single( ruffian.ProficiencyGrants );
        Assert.Equal( ProficiencyTargets.MediumArmor.Id, proficiency.Target.Id );
        Assert.Equal( ProficiencyRank.Trained, proficiency.Rank );
    }

    [Fact]
    public void GetRogueRacket_Mastermind_ReturnsFiniteKnowledgeChoice()
    {
        RogueRacketRepository repository = new RogueRacketRepository();

        RogueRacket mastermind = repository.GetRogueRacket( "rogue_racket.mastermind" );

        RogueSkillGrantDescriptor choice = Assert.Single( mastermind.SkillGrants.Where( grant => grant.RequiresChoice ) );
        Assert.Equal(
            [ "skill.arcana", "skill.nature", "skill.occultism", "skill.religion" ],
            choice.Options );
    }
}
