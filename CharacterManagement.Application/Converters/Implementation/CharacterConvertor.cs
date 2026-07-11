using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters.Implementation;

public sealed class CharacterConvertor : ICharacterConvertor
{
    public DraftCharacter Convert( CharacterDto character ) => throw new NotSupportedException();

    public CharacterDto Convert( DraftCharacter draftCharacter )
    {
        ArgumentNullException.ThrowIfNull( draftCharacter );

        return new CharacterDto
        {
            Id = draftCharacter.Id,
            Name = draftCharacter.Name,
            Concept = draftCharacter.Concept,
            Age = draftCharacter.Age,
            AncestryType = draftCharacter.AncestryType,
            Characteristics = new GroupCharacteristicDto
            {
                Strength = Convert( draftCharacter.AbilityScores.Strength ),
                Dexterity = Convert( draftCharacter.AbilityScores.Dexterity ),
                Constitution = Convert( draftCharacter.AbilityScores.Constitution ),
                Intelligence = Convert( draftCharacter.AbilityScores.Intelligence ),
                Wisdom = Convert( draftCharacter.AbilityScores.Wisdom ),
                Charisma = Convert( draftCharacter.AbilityScores.Charisma ),
                MaxPortableWeight = draftCharacter.AbilityScores.MaxPortableWeight,
            },
            Backpack = null,
        };
    }

    private static CharacteristicDto Convert( Characteristic characteristic )
    {
        return new CharacteristicDto
        {
            Value = characteristic.Value,
            Modifier = characteristic.Modifier,
        };
    }
}
