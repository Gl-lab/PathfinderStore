using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters.Implementation;

public sealed class CharacterConvertor : ICharacterConvertor
{
    private readonly IAncestryRepository? _ancestryRepository;
    private readonly IBackgroundRepository? _backgroundRepository;
    private readonly ICharacterClassRepository? _characterClassRepository;

    public CharacterConvertor(
        IAncestryRepository? ancestryRepository = null,
        IBackgroundRepository? backgroundRepository = null,
        ICharacterClassRepository? characterClassRepository = null )
    {
        _ancestryRepository = ancestryRepository;
        _backgroundRepository = backgroundRepository;
        _characterClassRepository = characterClassRepository;
    }

    public DraftCharacter Convert( CharacterDto character ) => throw new NotSupportedException();

    public CharacterDto Convert( DraftCharacter draftCharacter )
    {
        ArgumentNullException.ThrowIfNull( draftCharacter );

        Ancestry? ancestry = _ancestryRepository?.GetAncestry( draftCharacter.AncestryType );
        Background? background = draftCharacter.SelectedBackgroundId is null
            ? null
            : _backgroundRepository?.GetBackground( draftCharacter.SelectedBackgroundId );
        CharacterClass? characterClass = draftCharacter.SelectedClassId is null
            ? null
            : _characterClassRepository?.GetCharacterClass( draftCharacter.SelectedClassId );

        return new CharacterDto
        {
            Id = draftCharacter.Id,
            Name = draftCharacter.Name,
            Concept = draftCharacter.Concept,
            Age = draftCharacter.Age,
            AncestryType = draftCharacter.AncestryType,
            AncestryPackage = ancestry is null ? null : AncestryDtoMapper.MapPackage( draftCharacter, ancestry ),
            BackgroundPackage = background is null
                ? null
                : BackgroundDtoMapper.MapPackage( draftCharacter, background ),
            ClassPackage = characterClass is null
                ? null
                : CharacterClassDtoMapper.MapPackage( draftCharacter, characterClass ),
            FinalFreeBoosts = draftCharacter.AppliedFinalFreeBoosts.ToArray(),
            DerivedStatistics = ancestry is null || characterClass is null
                ? null
                : CharacterDerivedStatisticsDtoMapper.Map(
                    draftCharacter,
                    ancestry,
                    characterClass ),
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
