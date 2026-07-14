using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Features.Characters.Queries.Mapping;

public sealed class CharacterDetailsDtoMapper
{
    private readonly IAncestryRepository? _ancestryRepository;
    private readonly IBackgroundRepository? _backgroundRepository;
    private readonly ICharacterClassRepository? _characterClassRepository;
    private readonly ISkillRepository? _skillRepository;
    private readonly IRogueRacketRepository? _rogueRacketRepository;
    private readonly IHuntersEdgeRepository? _huntersEdgeRepository;
    private readonly IDruidicOrderRepository? _druidicOrderRepository;
    private readonly IBardMuseRepository? _bardMuseRepository;
    private readonly IWitchPatronRepository? _witchPatronRepository;
    private readonly IArcaneSchoolRepository? _arcaneSchoolRepository;
    private readonly IArcaneThesisRepository? _arcaneThesisRepository;
    private readonly IClericDoctrineRepository? _clericDoctrineRepository;
    private readonly IDeityRepository? _deityRepository;

    public CharacterDetailsDtoMapper(
        IAncestryRepository? ancestryRepository = null,
        IBackgroundRepository? backgroundRepository = null,
        ICharacterClassRepository? characterClassRepository = null,
        ISkillRepository? skillRepository = null,
        IRogueRacketRepository? rogueRacketRepository = null,
        IHuntersEdgeRepository? huntersEdgeRepository = null,
        IDruidicOrderRepository? druidicOrderRepository = null,
        IBardMuseRepository? bardMuseRepository = null,
        IClericDoctrineRepository? clericDoctrineRepository = null,
        IDeityRepository? deityRepository = null,
        IWitchPatronRepository? witchPatronRepository = null,
        IArcaneSchoolRepository? arcaneSchoolRepository = null,
        IArcaneThesisRepository? arcaneThesisRepository = null )
    {
        _ancestryRepository = ancestryRepository;
        _backgroundRepository = backgroundRepository;
        _characterClassRepository = characterClassRepository;
        _skillRepository = skillRepository;
        _rogueRacketRepository = rogueRacketRepository;
        _huntersEdgeRepository = huntersEdgeRepository;
        _druidicOrderRepository = druidicOrderRepository;
        _bardMuseRepository = bardMuseRepository;
        _witchPatronRepository = witchPatronRepository;
        _arcaneSchoolRepository = arcaneSchoolRepository;
        _arcaneThesisRepository = arcaneThesisRepository;
        _clericDoctrineRepository = clericDoctrineRepository;
        _deityRepository = deityRepository;
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
        HuntersEdge? huntersEdge = ResolveHuntersEdge( draftCharacter );
        DruidicOrder? druidicOrder = ResolveDruidicOrder( draftCharacter );
        BardMuse? bardMuse = ResolveBardMuse( draftCharacter );
        WitchPatron? witchPatron = ResolveWitchPatron( draftCharacter );
        ArcaneSchool? arcaneSchool = ResolveArcaneSchool( draftCharacter );
        ArcaneThesis? arcaneThesis = ResolveArcaneThesis( draftCharacter );
        ClericDoctrine? clericDoctrine = ResolveClericDoctrine( draftCharacter );
        Deity? deity = ResolveDeity( draftCharacter );

        return new CharacterDto
        {
            Id = draftCharacter.Id,
            Name = draftCharacter.Name,
            Concept = draftCharacter.Concept,
            Age = draftCharacter.Age,
            Gender = draftCharacter.Gender,
            AncestryType = draftCharacter.AncestryType,
            AncestryPackage = ancestry is null ? null : AncestryDtoMapper.MapPackage( draftCharacter, ancestry ),
            BackgroundPackage = background is null
                ? null
                : BackgroundDtoMapper.MapPackage( draftCharacter, background ),
            ClassPackage = characterClass is null
                ? null
                : CharacterClassDtoMapper.MapPackage(
                    draftCharacter,
                    characterClass,
                    rogueRacket,
                    huntersEdge,
                    druidicOrder,
                    bardMuse,
                    clericDoctrine,
                    deity,
                    witchPatron,
                    arcaneSchool,
                    arcaneThesis ),
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
                    characterClass.InitialProficiencies
                        .Concat( rogueRacket?.ProficiencyGrants ?? [] )
                        .Concat( clericDoctrine?.ProficiencyGrants ?? [] )
                        .Concat( deity?.ProficiencyGrants ?? [] ) ) ),
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

    private HuntersEdge? ResolveHuntersEdge( DraftCharacter character )
    {
        if ( character.SelectedHuntersEdgeId is null )
        {
            return null;
        }

        if ( _huntersEdgeRepository is null )
        {
            throw new InvalidOperationException(
                "Hunter's Edge repository is required to map a selected Hunter's Edge." );
        }

        return _huntersEdgeRepository.GetHuntersEdge( character.SelectedHuntersEdgeId );
    }

    private ClericDoctrine? ResolveClericDoctrine( DraftCharacter character )
    {
        if ( character.SelectedClericDoctrineId is null )
        {
            return null;
        }

        if ( _clericDoctrineRepository is null )
        {
            throw new InvalidOperationException(
                "Cleric doctrine repository is required to map a selected Doctrine." );
        }

        return _clericDoctrineRepository.GetClericDoctrine( character.SelectedClericDoctrineId );
    }

    private DruidicOrder? ResolveDruidicOrder( DraftCharacter character )
    {
        if ( character.SelectedDruidicOrderId is null )
        {
            return null;
        }

        if ( _druidicOrderRepository is null )
        {
            throw new InvalidOperationException(
                "Druidic Order repository is required to map a selected Order." );
        }

        return _druidicOrderRepository.GetDruidicOrder( character.SelectedDruidicOrderId );
    }

    private BardMuse? ResolveBardMuse( DraftCharacter character )
    {
        if ( character.SelectedBardMuseId is null )
        {
            return null;
        }

        if ( _bardMuseRepository is null )
        {
            throw new InvalidOperationException(
                "Bard Muse repository is required to map a selected Muse." );
        }

        return _bardMuseRepository.GetBardMuse( character.SelectedBardMuseId );
    }

    private WitchPatron? ResolveWitchPatron( DraftCharacter character )
    {
        if ( character.SelectedWitchPatronId is null )
        {
            return null;
        }

        if ( _witchPatronRepository is null )
        {
            throw new InvalidOperationException(
                "Witch Patron repository is required to map a selected Patron." );
        }

        return _witchPatronRepository.GetWitchPatron( character.SelectedWitchPatronId );
    }

    private ArcaneSchool? ResolveArcaneSchool( DraftCharacter character )
    {
        if ( character.SelectedArcaneSchoolId is null )
        {
            return null;
        }

        if ( _arcaneSchoolRepository is null )
        {
            throw new InvalidOperationException(
                "Arcane School repository is required to map a selected School." );
        }

        return _arcaneSchoolRepository.GetArcaneSchool( character.SelectedArcaneSchoolId );
    }

    private ArcaneThesis? ResolveArcaneThesis( DraftCharacter character )
    {
        if ( character.SelectedArcaneThesisId is null )
        {
            return null;
        }

        if ( _arcaneThesisRepository is null )
        {
            throw new InvalidOperationException(
                "Arcane Thesis repository is required to map a selected Thesis." );
        }

        return _arcaneThesisRepository.GetArcaneThesis( character.SelectedArcaneThesisId );
    }

    private Deity? ResolveDeity( DraftCharacter character )
    {
        if ( character.SelectedDeityId is null )
        {
            return null;
        }

        if ( _deityRepository is null )
        {
            throw new InvalidOperationException(
                "Deity repository is required to map a selected deity." );
        }

        return _deityRepository.GetDeity( character.SelectedDeityId );
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
