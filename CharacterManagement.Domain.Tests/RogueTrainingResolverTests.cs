using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.CharacterManagement.Domain.Rules.Training;

namespace CharacterManagement.Domain.Tests;

public sealed class RogueTrainingResolverTests
{
    private static readonly SourceReference Source = new SourceReference( "Player Core", 166 );

    [Fact]
    public void Resolve_Thief_AddsStealthAndThieveryAfterBackground()
    {
        RogueRacket racket = CreateRacket(
            "thief",
            new RogueSkillGrantDescriptor( "rogue_racket.thief.skill.thievery", "skill.thievery", [] ) );

        RogueTrainingResult result = RogueTrainingResolver.Resolve(
            racket,
            [],
            CreateSkills(),
            [ new TrainedSkill( "skill.athletics", "background.guard.skill.athletics" ) ] );

        Assert.Equal(
            [ "skill.athletics", "skill.stealth", "skill.thievery" ],
            result.Skills.Select( skill => skill.SkillId ) );
        Assert.Equal( "class.rogue.skill.stealth", result.Skills[ 1 ].SourceGrantId );
    }

    [Fact]
    public void Resolve_DuplicateFixedGrant_RequiresAndAppliesReplacement()
    {
        RogueRacket racket = CreateRacket(
            "thief",
            new RogueSkillGrantDescriptor( "rogue_racket.thief.skill.thievery", "skill.thievery", [] ) );
        IReadOnlyList<TrainedSkill> existing =
        [
            new TrainedSkill( "skill.thievery", "background.detective.skill.thievery" ),
        ];

        Assert.Throws<CharacterManagementException>( () => RogueTrainingResolver.Resolve(
            racket,
            [],
            CreateSkills(),
            existing ) );

        RogueTrainingResult result = RogueTrainingResolver.Resolve(
            racket,
            [ new RogueTrainingChoice( "rogue_racket.thief.skill.thievery", null, "skill.athletics" ) ],
            CreateSkills(),
            existing );

        Assert.Contains( result.Skills, skill =>
            skill.SkillId == "skill.athletics" &&
            skill.SourceGrantId == "rogue_racket.thief.skill.thievery" );
    }

    [Fact]
    public void Resolve_Mastermind_RequiresFiniteKnowledgeChoice()
    {
        RogueRacket racket = CreateRacket(
            "mastermind",
            new RogueSkillGrantDescriptor(
                "rogue_racket.mastermind.skill.knowledge",
                null,
                [ "skill.arcana", "skill.nature" ] ) );

        Assert.Throws<CharacterManagementException>( () => RogueTrainingResolver.Resolve(
            racket,
            [],
            CreateSkills(),
            [] ) );

        RogueTrainingResult result = RogueTrainingResolver.Resolve(
            racket,
            [ new RogueTrainingChoice( "rogue_racket.mastermind.skill.knowledge", "skill.arcana", null ) ],
            CreateSkills(),
            [] );

        Assert.Contains( result.Skills, skill => skill.SkillId == "skill.arcana" );
    }

    [Fact]
    public void Resolve_RejectsReplacementWithoutConflictAndUnknownChoice()
    {
        RogueRacket racket = CreateRacket(
            "thief",
            new RogueSkillGrantDescriptor( "rogue_racket.thief.skill.thievery", "skill.thievery", [] ) );

        Assert.Throws<CharacterManagementException>( () => RogueTrainingResolver.Resolve(
            racket,
            [ new RogueTrainingChoice( "rogue_racket.thief.skill.thievery", null, "skill.athletics" ) ],
            CreateSkills(),
            [] ) );
        Assert.Throws<CharacterManagementException>( () => RogueTrainingResolver.Resolve(
            racket,
            [ new RogueTrainingChoice( "rogue_racket.other.skill.unknown", null, null ) ],
            CreateSkills(),
            [] ) );
    }

    private static RogueRacket CreateRacket(
        string id,
        params RogueSkillGrantDescriptor[] grants )
    {
        return new RogueRacket(
            $"rogue_racket.{id}",
            id,
            Source,
            null,
            grants,
            [],
            [],
            [] );
    }

    private static IReadOnlyCollection<SkillDefinition> CreateSkills()
    {
        return
        [
            new SkillDefinition( "skill.arcana", "Arcana", AbilityType.Intelligence, Source ),
            new SkillDefinition( "skill.athletics", "Athletics", AbilityType.Strength, Source ),
            new SkillDefinition( "skill.nature", "Nature", AbilityType.Wisdom, Source ),
            new SkillDefinition( "skill.stealth", "Stealth", AbilityType.Dexterity, Source ),
            new SkillDefinition( "skill.thievery", "Thievery", AbilityType.Dexterity, Source ),
        ];
    }
}
