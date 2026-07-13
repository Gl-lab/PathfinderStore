using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class CharacterDerivedStatisticsDtoMapper
{
    public static CharacterDerivedStatisticsDto Map(
        DraftCharacter character,
        Ancestry ancestry,
        CharacterClass characterClass )
    {
        CharacterHitPoints hitPoints = CharacterHitPoints.Calculate(
            character,
            ancestry,
            characterClass );

        return new CharacterDerivedStatisticsDto
        {
            HitPoints = new CharacterHitPointsDto
            {
                Maximum = hitPoints.MaximumHitPoints,
                Ancestry = hitPoints.AncestryHitPoints,
                Class = hitPoints.ClassHitPoints,
                ConstitutionModifier = hitPoints.ConstitutionModifier,
            },
        };
    }
}
