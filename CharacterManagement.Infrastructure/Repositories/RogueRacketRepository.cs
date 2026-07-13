using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class RogueRacketRepository : IRogueRacketRepository
{
    private static readonly Dictionary<string, RogueRacket> Rackets = CreateRackets()
        .ToDictionary( racket => racket.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<RogueRacket> GetAll() => Rackets.Values.ToList();

    public RogueRacket GetRogueRacket( string rogueRacketId )
    {
        if ( String.IsNullOrWhiteSpace( rogueRacketId ) )
        {
            throw new ArgumentException( "Rogue racket id cannot be empty.", nameof( rogueRacketId ) );
        }

        if ( !Rackets.TryGetValue( rogueRacketId, out RogueRacket? racket ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( rogueRacketId ),
                $"Rogue racket '{rogueRacketId}' is not defined." );
        }

        return racket;
    }

    private static IReadOnlyCollection<RogueRacket> CreateRackets()
    {
        return
        [
            Create(
                "mastermind",
                "Mastermind",
                166,
                AbilityType.Intelligence,
                [
                    FixedSkill( "mastermind", "society" ),
                    ChoiceSkill( "mastermind", "knowledge", "arcana", "nature", "occultism", "religion" ),
                ],
                [],
                Effect( "mastermind", "recall_knowledge", "Recall Knowledge", "Identifying a creature can make it off-guard." ) ),
            Create(
                "ruffian",
                "Ruffian",
                166,
                AbilityType.Strength,
                [ FixedSkill( "ruffian", "intimidation" ) ],
                [
                    new ProficiencyGrant(
                        ProficiencyTargets.MediumArmor,
                        ProficiencyRank.Trained,
                        "rogue_racket.ruffian.proficiency.medium_armor" ),
                ],
                Effect( "ruffian", "brutal_sneak_attack", "Brutal Sneak Attack", "Extends Sneak Attack weapon eligibility and critical specialization." ) ),
            Create(
                "scoundrel",
                "Scoundrel",
                166,
                AbilityType.Charisma,
                [ FixedSkill( "scoundrel", "deception" ), FixedSkill( "scoundrel", "diplomacy" ) ],
                [],
                Effect( "scoundrel", "feint", "Feint", "Extends Feint and grants a conditional Step." ) ),
            Create(
                "thief",
                "Thief",
                167,
                null,
                [ FixedSkill( "thief", "thievery" ) ],
                [],
                Effect( "thief", "dexterity_damage", "Dexterity Damage", "Uses Dexterity instead of Strength for eligible finesse melee damage." ) ),
        ];
    }

    private static RogueRacket Create(
        string id,
        string name,
        int page,
        AbilityType? alternativeKeyAbility,
        IReadOnlyList<RogueSkillGrantDescriptor> skillGrants,
        IReadOnlyList<ProficiencyGrant> proficiencyGrants,
        RogueRacketEffectDescriptor effect )
    {
        return new RogueRacket(
            $"rogue_racket.{id}",
            name,
            new SourceReference( "Player Core", page ),
            alternativeKeyAbility,
            skillGrants,
            proficiencyGrants,
            [ effect ],
            [ CharacterClassDependencyType.ClassFeatureRules ] );
    }

    private static RogueSkillGrantDescriptor FixedSkill( string racketId, string skillId )
    {
        return new RogueSkillGrantDescriptor(
            $"rogue_racket.{racketId}.skill.{skillId}",
            $"skill.{skillId}",
            [] );
    }

    private static RogueSkillGrantDescriptor ChoiceSkill(
        string racketId,
        string grantId,
        params string[] skillIds )
    {
        return new RogueSkillGrantDescriptor(
            $"rogue_racket.{racketId}.skill.{grantId}",
            null,
            skillIds.Select( skillId => $"skill.{skillId}" ).ToArray() );
    }

    private static RogueRacketEffectDescriptor Effect(
        string racketId,
        string effectId,
        string name,
        string summary )
    {
        return new RogueRacketEffectDescriptor(
            $"rogue_racket.{racketId}.effect.{effectId}",
            name,
            summary );
    }
}
