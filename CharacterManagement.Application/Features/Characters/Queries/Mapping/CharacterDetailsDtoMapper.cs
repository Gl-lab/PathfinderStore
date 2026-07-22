using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.Avatars;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Completion;
using Pathfinder.CharacterManagement.Application.Equipment;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Feats;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

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
    private readonly IClericDomainRepository? _clericDomainRepository;
    private readonly ISpellRepository? _spellRepository;
    private readonly IAvatarCatalog? _avatarCatalog;
    private readonly IFeatRepository? _featRepository;
    private readonly CharacterCompletionEvaluator? _completionEvaluator;
    private readonly IAllowedEquipmentReader? _allowedEquipmentReader;

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
        IArcaneThesisRepository? arcaneThesisRepository = null,
        IClericDomainRepository? clericDomainRepository = null,
        ISpellRepository? spellRepository = null,
        IAvatarCatalog? avatarCatalog = null,
        IFeatRepository? featRepository = null,
        CharacterCompletionEvaluator? completionEvaluator = null,
        IAllowedEquipmentReader? allowedEquipmentReader = null )
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
        _clericDomainRepository = clericDomainRepository;
        _spellRepository = spellRepository;
        _avatarCatalog = avatarCatalog;
        _featRepository = featRepository;
        _completionEvaluator = completionEvaluator;
        _allowedEquipmentReader = allowedEquipmentReader;
    }

    public DraftCharacter Convert( CharacterDto character ) => throw new NotSupportedException();

    public CharacterDto Convert( DraftCharacter draftCharacter ) => Convert( draftCharacter, null );

    public CharacterDto Convert( DraftCharacter draftCharacter, int? campaignId )
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
        ClericDomain? clericDomain = ResolveClericDomain( draftCharacter );
        ClericSpellLoadoutDto? clericSpellLoadout = ResolveClericSpellLoadout(
            draftCharacter,
            deity );
        ClericFocusPoolDto? clericFocusPool = ResolveClericFocusPool( clericDomain );
        BardSpellLoadoutDto? bardSpellLoadout = ResolveBardSpellLoadout(
            draftCharacter,
            bardMuse );
        BardCompositionPackageDto? bardComposition = ResolveBardComposition(
            characterClass,
            bardMuse );
        DruidSpellLoadoutDto? druidSpellLoadout = ResolveDruidSpellLoadout(
            draftCharacter,
            druidicOrder );
        DruidFocusPoolDto? druidFocusPool = ResolveDruidFocusPool( druidicOrder );
        WitchSpellLoadoutDto? witchSpellLoadout = ResolveWitchSpellLoadout(
            draftCharacter,
            witchPatron );
        WitchHexPackageDto? witchHexPackage = ResolveWitchHexPackage(
            draftCharacter,
            witchPatron );
        WizardSpellLoadoutDto? wizardSpellLoadout = ResolveWizardSpellLoadout(
            draftCharacter,
            arcaneSchool );
        WizardSchoolMagicDto? wizardSchoolMagic = ResolveWizardSchoolMagic(
            draftCharacter,
            arcaneSchool );
        IReadOnlyList<EffectiveProficiency> effectiveProficiencies = characterClass is null
            ? []
            : ProficiencyResolver.Resolve(
                characterClass.InitialProficiencies
                    .Concat( rogueRacket?.ProficiencyGrants ?? [] )
                    .Concat( clericDoctrine?.ProficiencyGrants ?? [] )
                    .Concat( deity?.ProficiencyGrants ?? [] )
                    .Concat( ResolveSpellcastingProficiencyGrants( characterClass, witchPatron ) ) );
        IReadOnlyCollection<CharacterFeat> characterFeats = _featRepository is null
            ? []
            : CharacterFeatResolver.Resolve(
                draftCharacter,
                background,
                characterClass,
                bardMuse,
                druidicOrder,
                clericDoctrine,
                arcaneSchool,
                arcaneThesis,
                _featRepository.GetAll() );
        FeatTrainingResult? featTraining = _featRepository is null
            ? null
            : FeatTrainingResolver.Resolve(
                characterFeats,
                draftCharacter.TrainedSkills,
                draftCharacter.TrainedLore );
        AllowedEquipmentLoadout? allowedEquipment = ReadAllowedEquipment(
            draftCharacter,
            effectiveProficiencies,
            campaignId );

        return new CharacterDto
        {
            Id = draftCharacter.Id,
            Name = draftCharacter.Name,
            Concept = draftCharacter.Concept,
            Age = draftCharacter.Age,
            Gender = draftCharacter.Gender,
            AvatarId = draftCharacter.AvatarId.Value,
            AvatarPath = _avatarCatalog?.ResolvePath( draftCharacter.AvatarId ) ?? AvatarCatalog.UnknownPath,
            CreationStatus = draftCharacter.CreationStatus,
            CompletedAtUtc = draftCharacter.CompletedAtUtc,
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
                    arcaneThesis,
                    clericDomain,
                    clericSpellLoadout,
                    clericFocusPool,
                    bardSpellLoadout,
                    bardComposition,
                    druidSpellLoadout,
                    druidFocusPool,
                    witchSpellLoadout,
                    witchHexPackage,
                    wizardSpellLoadout,
                    wizardSchoolMagic ),
            FinalFreeBoosts = draftCharacter.AppliedFinalFreeBoosts.ToArray(),
            DerivedStatistics = ancestry is null || characterClass is null
                ? null
                : CharacterDerivedStatisticsDtoMapper.Map(
                    draftCharacter,
                    ancestry,
                    characterClass,
                    effectiveProficiencies,
                    _skillRepository,
                    featTraining,
                    allowedEquipment,
                    witchPatron?.SpellTradition ?? characterClass.SpellTradition ),
            Training = CharacterTrainingDtoMapper.Map( draftCharacter, _skillRepository, featTraining ),
            Proficiencies = characterClass is null
                ? []
                : CharacterClassDtoMapper.MapProficiencies( effectiveProficiencies ),
            Feats = characterFeats
                .Select( CharacterFeatDtoMapper.Map )
                .ToArray(),
            Completion = _completionEvaluator?.Evaluate( draftCharacter ) ?? new CharacterCompletionDto(),
            StartingEquipment = MapStartingEquipment( draftCharacter, allowedEquipment ),
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

    private CharacterStartingEquipmentDto? MapStartingEquipment(
        DraftCharacter character,
        AllowedEquipmentLoadout? loadout )
    {
        if ( character.SelectedClassKitId is null )
        {
            return null;
        }

        if ( loadout is null )
        {
            throw new InvalidOperationException( "Allowed equipment loadout is required to map starting equipment." );
        }

        IReadOnlyList<CharacterEquipmentLineDto> items = loadout.Items
            .Select( item =>
            {
                return new CharacterEquipmentLineDto
                {
                    Definition = new CharacterEquipmentDefinitionDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Category = item.Category,
                        PriceCopper = item.PriceCopper,
                        BulkTenths = item.BulkTenths,
                        UnitsPerPurchase = item.UnitsPerPurchase,
                    },
                    PurchaseQuantity = item.PurchaseQuantity,
                    UnitQuantity = item.UnitQuantity,
                    EquippedQuantity = item.EquippedQuantity,
                    ProficiencyTargetId = item.ProficiencyTargetId,
                    ProficiencyRank = item.ProficiencyRank,
                };
            } )
            .ToArray();
        return new CharacterStartingEquipmentDto
        {
            ClassKitId = character.SelectedClassKitId,
            SelectedOptionIds = character.SelectedClassKitOptionIds.ToArray(),
            Items = items,
            TotalPriceCopper = loadout.TotalPriceCopper,
            RemainingWealthCopper = loadout.RemainingWealthCopper,
            TotalBulkTenths = loadout.TotalBulkTenths,
            EncumberedAtBulkTenths = loadout.EncumberedAtBulkTenths,
            MaximumBulkTenths = loadout.MaximumBulkTenths,
            IsEncumbered = loadout.IsEncumbered,
            ExceedsMaximumBulk = loadout.ExceedsMaximumBulk,
        };
    }

    private AllowedEquipmentLoadout? ReadAllowedEquipment(
        DraftCharacter character,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        int? campaignId )
    {
        if ( character.SelectedClassKitId is null )
        {
            return null;
        }

        if ( _allowedEquipmentReader is null )
        {
            throw new InvalidOperationException( "Allowed equipment reader is required to map starting equipment." );
        }

        return _allowedEquipmentReader.Read( character, proficiencies, campaignId );
    }

    private static IReadOnlyList<ProficiencyGrant> ResolveSpellcastingProficiencyGrants(
        CharacterClass characterClass,
        WitchPatron? witchPatron )
    {
        if ( ( characterClass.Id != "class.witch" ) || ( witchPatron is null ) )
        {
            return [];
        }

        string sourceGrantId = "class.witch.initial_proficiencies";
        return
        [
            new ProficiencyGrant(
                ProficiencyTargets.SpellAttack( witchPatron.SpellTradition ),
                ProficiencyRank.Trained,
                sourceGrantId ),
            new ProficiencyGrant(
                ProficiencyTargets.SpellDc( witchPatron.SpellTradition ),
                ProficiencyRank.Trained,
                sourceGrantId ),
        ];
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

    private ClericDomain? ResolveClericDomain( DraftCharacter character )
    {
        if ( character.SelectedClericDomainId is null )
        {
            return null;
        }

        if ( _clericDomainRepository is null )
        {
            throw new InvalidOperationException(
                "Cleric domain repository is required to map a selected domain." );
        }

        return _clericDomainRepository.GetClericDomain( character.SelectedClericDomainId );
    }

    private ClericSpellLoadoutDto? ResolveClericSpellLoadout(
        DraftCharacter character,
        Deity? deity )
    {
        if ( ( character.SelectedClassId != "class.cleric" ) || ( deity is null ) )
        {
            return null;
        }

        if ( _spellRepository is null )
        {
            throw new InvalidOperationException(
                "Spell repository is required to map a Cleric spell loadout." );
        }

        IReadOnlyCollection<SpellDefinition> catalog = _spellRepository.GetAll();
        IReadOnlyDictionary<string, SpellDefinition> definitions = catalog
            .ToDictionary( spell => spell.Id, StringComparer.Ordinal );
        IReadOnlyDictionary<string, ClericAvailableSpell> availableSpells =
            ClericSpellAvailabilityResolver
                .ResolveRankOneSpells( deity, catalog )
                .ToDictionary( spell => spell.Spell.Id, StringComparer.Ordinal );
        SpellDefinition? fontSpell = character.SelectedDivineFont switch
        {
            DivineFont.Heal => definitions[ "spell.heal" ],
            DivineFont.Harm => definitions[ "spell.harm" ],
            _ => null,
        };

        return new ClericSpellLoadoutDto
        {
            Cantrips = character.PreparedClericCantripIds
                .Select( spellId => SpellDefinitionDtoMapper.Map( definitions[ spellId ] ) )
                .ToArray(),
            PreparedSpells = character.PreparedClericSpellIds
                .Select( spellId => new ClericPreparedSpellDto
                {
                    Spell = SpellDefinitionDtoMapper.Map( definitions[ spellId ] ),
                    AccessSources = availableSpells[ spellId ].AccessSources,
                } )
                .ToArray(),
            DivineFontSpells = fontSpell is null
                ? []
                : Enumerable
                    .Range( 0, 4 )
                    .Select( _ => SpellDefinitionDtoMapper.Map( fontSpell ) )
                    .ToArray(),
        };
    }

    private ClericFocusPoolDto? ResolveClericFocusPool( ClericDomain? clericDomain )
    {
        if ( clericDomain is null )
        {
            return null;
        }

        if ( _spellRepository is null )
        {
            throw new InvalidOperationException(
                "Spell repository is required to map a Cleric focus pool." );
        }

        ClericFocusPool focusPool = ClericFocusPoolResolver.Resolve(
            clericDomain,
            _spellRepository.GetAll() );
        return ClericDomainDtoMapper.Map( focusPool );
    }

    private BardSpellLoadoutDto? ResolveBardSpellLoadout(
        DraftCharacter character,
        BardMuse? bardMuse )
    {
        if ( ( character.SelectedClassId != "class.bard" ) ||
             ( bardMuse is null ) ||
             ( character.BardCantripIds.Count == 0 ) ||
             ( character.BardSpellIds.Count == 0 ) )
        {
            return null;
        }

        if ( _spellRepository is null )
        {
            throw new InvalidOperationException(
                "Spell repository is required to map a Bard spell loadout." );
        }

        IReadOnlyDictionary<string, SpellDefinition> definitions = _spellRepository
            .GetAll()
            .ToDictionary( spell => spell.Id, StringComparer.Ordinal );
        string museGrantedSpellId = bardMuse.Benefits
            .Single( benefit => benefit.Kind == BardMuseBenefitKind.RepertoireSpell )
            .Id;
        BardRepertoireSpellDto[] repertoire = character.BardSpellIds
            .Select( spellId => new BardRepertoireSpellDto
            {
                Spell = SpellDefinitionDtoMapper.Map( definitions[ spellId ] ),
                Source = BardRepertoireSpellSource.Selected,
                SourceGrantId = "class_feature.bard.spell_repertoire",
            } )
            .Append( new BardRepertoireSpellDto
            {
                Spell = SpellDefinitionDtoMapper.Map( definitions[ museGrantedSpellId ] ),
                Source = BardRepertoireSpellSource.MuseGranted,
                SourceGrantId = bardMuse.Id,
            } )
            .ToArray();

        return new BardSpellLoadoutDto
        {
            Cantrips = character.BardCantripIds
                .Select( spellId => SpellDefinitionDtoMapper.Map( definitions[ spellId ] ) )
                .ToArray(),
            RankOneRepertoire = repertoire,
            RankOneSpellSlotCount = 2,
        };
    }

    private BardCompositionPackageDto? ResolveBardComposition(
        CharacterClass? characterClass,
        BardMuse? bardMuse )
    {
        if ( ( characterClass?.Id != "class.bard" ) || ( bardMuse is null ) )
        {
            return null;
        }

        if ( _spellRepository is null )
        {
            throw new InvalidOperationException(
                "Spell repository is required to map Bard composition spells." );
        }

        BardCompositionPackage package = BardCompositionResolver.Resolve(
            _spellRepository.GetAll() );
        return new BardCompositionPackageDto
        {
            MaximumFocusPoints = package.MaximumFocusPoints,
            CompositionCantrip = SpellDefinitionDtoMapper.Map( package.CompositionCantrip ),
            FocusSpell = SpellDefinitionDtoMapper.Map( package.FocusSpell ),
            SourceGrantId = package.SourceGrantId,
        };
    }

    private DruidSpellLoadoutDto? ResolveDruidSpellLoadout(
        DraftCharacter character,
        DruidicOrder? druidicOrder )
    {
        if ( ( character.SelectedClassId != "class.druid" ) ||
             ( druidicOrder is null ) ||
             ( character.PreparedDruidCantripIds.Count == 0 ) ||
             ( character.PreparedDruidSpellIds.Count == 0 ) )
        {
            return null;
        }

        if ( _spellRepository is null )
        {
            throw new InvalidOperationException(
                "Spell repository is required to map a Druid spell loadout." );
        }

        IReadOnlyDictionary<string, SpellDefinition> definitions = _spellRepository
            .GetAll()
            .ToDictionary( spell => spell.Id, StringComparer.Ordinal );
        return new DruidSpellLoadoutDto
        {
            Cantrips = character.PreparedDruidCantripIds
                .Select( spellId => SpellDefinitionDtoMapper.Map( definitions[ spellId ] ) )
                .ToArray(),
            PreparedSpells = character.PreparedDruidSpellIds
                .Select( spellId => SpellDefinitionDtoMapper.Map( definitions[ spellId ] ) )
                .ToArray(),
        };
    }

    private DruidFocusPoolDto? ResolveDruidFocusPool( DruidicOrder? druidicOrder )
    {
        if ( druidicOrder is null )
        {
            return null;
        }

        if ( _spellRepository is null )
        {
            throw new InvalidOperationException(
                "Spell repository is required to map a Druid focus pool." );
        }

        DruidFocusPool focusPool = DruidFocusPoolResolver.Resolve(
            druidicOrder,
            _spellRepository.GetAll() );
        return new DruidFocusPoolDto
        {
            MaximumFocusPoints = focusPool.MaximumFocusPoints,
            FocusSpell = SpellDefinitionDtoMapper.Map( focusPool.FocusSpell ),
            SourceGrantId = focusPool.SourceGrantId,
        };
    }

    private WitchSpellLoadoutDto? ResolveWitchSpellLoadout(
        DraftCharacter character,
        WitchPatron? patron )
    {
        if ( ( patron is null ) ||
             ( character.WitchFamiliarCantripIds.Count == 0 ) ||
             ( character.WitchFamiliarSpellIds.Count == 0 ) ||
             ( character.PreparedWitchCantripIds.Count == 0 ) ||
             ( character.PreparedWitchSpellIds.Count == 0 ) )
        {
            return null;
        }

        IReadOnlyDictionary<string, SpellDefinition> definitions = GetSpellDefinitions();
        string? familiarSpellChoiceId = patron.FamiliarSpellOptions.Count > 1
            ? character.SelectedWitchPatronFamiliarSpellId
            : null;
        string patronSpellId = patron
            .ResolveFamiliarSpell( familiarSpellChoiceId )
            .Id;
        return new WitchSpellLoadoutDto
        {
            FamiliarCantrips = MapSpells( character.WitchFamiliarCantripIds, definitions ),
            FamiliarRankOneSpells = MapSpells( character.WitchFamiliarSpellIds, definitions ),
            PatronGrantedSpell = SpellDefinitionDtoMapper.Map( definitions[ patronSpellId ] ),
            PreparedCantrips = MapSpells( character.PreparedWitchCantripIds, definitions ),
            PreparedSpells = MapSpells( character.PreparedWitchSpellIds, definitions ),
        };
    }

    private WitchHexPackageDto? ResolveWitchHexPackage(
        DraftCharacter character,
        WitchPatron? patron )
    {
        if ( ( patron is null ) || String.IsNullOrWhiteSpace( character.SelectedWitchFocusHexId ) )
        {
            return null;
        }

        WitchHexPackage package = WitchHexPackageResolver.Resolve(
            patron,
            character.SelectedWitchFocusHexId,
            GetSpellDefinitions().Values.ToArray() );
        return new WitchHexPackageDto
        {
            MaximumFocusPoints = package.MaximumFocusPoints,
            PatronHexCantrip = SpellDefinitionDtoMapper.Map( package.PatronHexCantrip ),
            FocusHex = SpellDefinitionDtoMapper.Map( package.FocusHex ),
            SourceGrantId = package.SourceGrantId,
        };
    }

    private IReadOnlyDictionary<string, SpellDefinition> GetSpellDefinitions()
    {
        if ( _spellRepository is null )
        {
            throw new InvalidOperationException( "Spell repository is required to map Witch spells." );
        }

        return _spellRepository
            .GetAll()
            .ToDictionary( spell => spell.Id, StringComparer.Ordinal );
    }

    private WizardSpellLoadoutDto? ResolveWizardSpellLoadout(
        DraftCharacter character,
        ArcaneSchool? school )
    {
        if ( ( school is null ) ||
             ( character.WizardSpellbookCantripIds.Count == 0 ) ||
             ( character.WizardSpellbookSpellIds.Count == 0 ) ||
             ( character.PreparedWizardCantripIds.Count == 0 ) ||
             ( character.PreparedWizardSpellIds.Count == 0 ) )
        {
            return null;
        }

        IReadOnlyDictionary<string, SpellDefinition> definitions = GetSpellDefinitions();
        return new WizardSpellLoadoutDto
        {
            SpellbookCantrips = MapSpells( character.WizardSpellbookCantripIds, definitions ),
            SpellbookRankOneSpells = MapSpells( character.WizardSpellbookSpellIds, definitions ),
            CurriculumCantrip = MapOptionalSpell( character.SelectedWizardCurriculumCantripId, definitions ),
            CurriculumRankOneSpells = MapSpells( character.WizardCurriculumSpellIds, definitions ),
            PreparedCantrips = MapSpells( character.PreparedWizardCantripIds, definitions ),
            PreparedRankOneSpells = MapSpells( character.PreparedWizardSpellIds, definitions ),
            PreparedCurriculumCantrip = MapOptionalSpell(
                character.SelectedPreparedWizardCurriculumCantripId,
                definitions ),
            PreparedCurriculumRankOneSpell = MapOptionalSpell(
                character.SelectedPreparedWizardCurriculumSpellId,
                definitions ),
            CurriculumRankOneSpellSlotCount = school.HasCurriculum ? 1 : 0,
        };
    }

    private WizardSchoolMagicDto? ResolveWizardSchoolMagic(
        DraftCharacter character,
        ArcaneSchool? school )
    {
        if ( ( character.SelectedClassId != "class.wizard" ) ||
             ( school is null ) ||
             ( character.WizardSpellbookCantripIds.Count == 0 ) )
        {
            return null;
        }

        string initialSchoolSpellId = school.Benefits
            .Single( benefit => benefit.Kind == ArcaneSchoolBenefitKind.InitialSchoolSpell )
            .Id;
        IReadOnlyDictionary<string, SpellDefinition> definitions = GetSpellDefinitions();
        return new WizardSchoolMagicDto
        {
            MaximumFocusPoints = 1,
            DrainBondedItemUsesPerDay = 1,
            InitialSchoolSpell = SpellDefinitionDtoMapper.Map( definitions[ initialSchoolSpellId ] ),
            SourceGrantId = school.Id,
        };
    }

    private static SpellDefinitionDto? MapOptionalSpell(
        string? spellId,
        IReadOnlyDictionary<string, SpellDefinition> definitions )
    {
        return String.IsNullOrWhiteSpace( spellId )
            ? null
            : SpellDefinitionDtoMapper.Map( definitions[ spellId ] );
    }

    private static IReadOnlyList<SpellDefinitionDto> MapSpells(
        IReadOnlyList<string> spellIds,
        IReadOnlyDictionary<string, SpellDefinition> definitions )
    {
        return spellIds
            .Select( spellId => SpellDefinitionDtoMapper.Map( definitions[ spellId ] ) )
            .ToArray();
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
