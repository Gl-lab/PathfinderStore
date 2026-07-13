using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.CharacterManagement.Domain.Entity;

public sealed class CharacterHitPoints : ValueObject
{
    public int AncestryHitPoints { get; }
    public int ClassHitPoints { get; }
    public int ConstitutionModifier { get; }
    public int MaximumHitPoints => AncestryHitPoints + ClassHitPoints + ConstitutionModifier;

    private CharacterHitPoints(
        int ancestryHitPoints,
        int classHitPoints,
        int constitutionModifier )
    {
        AncestryHitPoints = ancestryHitPoints;
        ClassHitPoints = classHitPoints;
        ConstitutionModifier = constitutionModifier;
    }

    public static CharacterHitPoints Calculate(
        DraftCharacter character,
        Ancestry ancestry,
        CharacterClass characterClass )
    {
        ArgumentNullException.ThrowIfNull( character );
        ArgumentNullException.ThrowIfNull( ancestry );
        ArgumentNullException.ThrowIfNull( characterClass );

        if ( character.AncestryType != ancestry.AncestryType )
        {
            throw new CharacterManagementException(
                "Ancestry catalog entry does not match the character ancestry." );
        }

        if ( character.SelectedClassId != characterClass.Id )
        {
            throw new CharacterManagementException(
                "Class catalog entry does not match the character class." );
        }

        int ancestryHitPoints = ancestry.GetEffectiveBaseHitPoints(
            character.SelectedHeritageId,
            character.SelectedAncestryFeatId );

        return new CharacterHitPoints(
            ancestryHitPoints,
            characterClass.BaseHitPoints,
            character.AbilityScores.Constitution.Modifier );
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AncestryHitPoints;
        yield return ClassHitPoints;
        yield return ConstitutionModifier;
    }
}
