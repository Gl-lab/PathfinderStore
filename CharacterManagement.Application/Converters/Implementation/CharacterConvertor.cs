using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters.Implementation;

public sealed class CharacterConvertor : ICharacterConvertor
{
    private readonly IAncestryRepository? _ancestryRepository;
    private readonly IBackgroundRepository? _backgroundRepository;
    private readonly ICharacterClassRepository? _characterClassRepository;
    private readonly ISkillRepository? _skillRepository;
    private readonly IRogueRacketRepository? _rogueRacketRepository;

    public CharacterConvertor(
        IAncestryRepository? ancestryRepository = null,
        IBackgroundRepository? backgroundRepository = null,
        ICharacterClassRepository? characterClassRepository = null,
        ISkillRepository? skillRepository = null,
        IRogueRacketRepository? rogueRacketRepository = null )
    {
        _ancestryRepository = ancestryRepository;
        _backgroundRepository = backgroundRepository;
        _characterClassRepository = characterClassRepository;
        _skillRepository = skillRepository;
        _rogueRacketRepository = rogueRacketRepository;
    }

    public DraftCharacter Convert( CharacterDto character ) => throw new NotSupportedException();

    public CharacterDto Convert( DraftCharacter draftCharacter )
    {
        ArgumentNullException.ThrowIfNull( draftCharacter );

        Ancestry? ancestry = _ancestryRepository?.GetAncestry( draftCharacter.AncestryType );
        Background? background = draftCharacter.SelectedBackgroundId is null
            ? null
            : _backgroundRepository?.GetBackground( draftCharacter.SelectedBackgroundId );
        CharacterClass? characterClass = ResolveCharacterClass( draftCharacter );
        RogueRacket? rogueRacket = ResolveRogueRacket( draftCharacter );

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
                : CharacterClassDtoMapper.MapPackage( draftCharacter, characterClass, rogueRacket ),
            FinalFreeBoosts = draftCharacter.AppliedFinalFreeBoosts.ToArray(),
            DerivedStatistics = ancestry is null || characterClass is null
                ? null
                : CharacterDerivedStatisticsDtoMapper.Map(
                    draftCharacter,
                    ancestry,
                    characterClass ),
            Training = CharacterTrainingDtoMapper.Map( draftCharacter, _skillRepository ),
            Proficiencies = characterClass is null
                ? []
                : CharacterClassDtoMapper.MapProficiencies( ProficiencyResolver.Resolve(
                    characterClass.InitialProficiencies.Concat( rogueRacket?.ProficiencyGrants ?? [] ) ) ),
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

    private RogueRacket? ResolveRogueRacket( DraftCharacter character )
    {
        if ( character.SelectedRogueRacketId is null )
        {
            return null;
        }

        if ( _rogueRacketRepository is null )
        {
            throw new InvalidOperationException(
                "Rogue racket repository is required to map a selected Rogue's Racket." );
        }

        return _rogueRacketRepository.GetRogueRacket( character.SelectedRogueRacketId );
    }

    private CharacterClass? ResolveCharacterClass( DraftCharacter character )
    {
        if ( character.SelectedClassId is null )
        {
            return null;
        }

        if ( _characterClassRepository is null )
        {
            throw new InvalidOperationException(
                "Character class repository is required to map class proficiencies." );
        }

        return _characterClassRepository.GetCharacterClass( character.SelectedClassId );
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
