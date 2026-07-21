using MediatR;
using Pathfinder.CharacterManagement.Application.Builders;
using Pathfinder.CharacterManagement.Application.Avatars;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class CreateCharacterHandler : IRequestHandler<CreateCharacterCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICharacterBuilder _characterBuilder;
    private readonly IAvatarSelector _avatarSelector;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCharacterHandler(
        IAccountRepository accountRepository,
        ICharacterBuilder characterBuilder,
        IUnitOfWork unitOfWork,
        IAvatarSelector avatarSelector )
    {
        _accountRepository = accountRepository;
        _characterBuilder = characterBuilder;
        _unitOfWork = unitOfWork;
        _avatarSelector = avatarSelector;
    }

    public async Task Handle( CreateCharacterCommand request, CancellationToken cancellationToken )
    {
        Account? account = await _accountRepository.GetByUserIdAsync( request.UserId );
        if ( account is null )
        {
            throw new CharacterManagementException( $"Account was not found for user {request.UserId}." );
        }

        AbilityType backgroundRestrictedBoost = request.Character.BackgroundRestrictedBoost
            ?? throw new CharacterManagementException( "Background restricted boost must be specified." );
        AbilityType backgroundFreeBoost = request.Character.BackgroundFreeBoost
            ?? throw new CharacterManagementException( "Background free boost must be specified." );
        AbilityType classKeyAbility = request.Character.ClassKeyAbility
            ?? throw new CharacterManagementException( "Class key ability must be specified." );
        IReadOnlyList<AbilityType> finalFreeBoosts = request.Character.FinalFreeBoosts
            ?? throw new CharacterManagementException( "Final free boosts must be specified." );

        AvatarId avatarId = _avatarSelector.Select( new AvatarSelectionCriteria(
            request.Character.AncestryType,
            request.Character.ClassId,
            request.Character.Gender,
            request.Character.HeritageId,
            ResolveSpecializationId( request.Character ),
            request.Character.BackgroundId ) );

        _characterBuilder.CreateCharacter(
            account.Id,
            request.Character.Name,
            request.Character.AncestryType,
            request.Character.Concept,
            request.Character.Age,
            request.Character.Gender,
            avatarId );
        _characterBuilder.SetAncestryPackage( request.Character.HeritageId, request.Character.AncestryFeatId );
        _characterBuilder.ApplyFreeBoosts( request.Character.FreeBoosts );
        _characterBuilder.SetBackground(
            request.Character.BackgroundId,
            backgroundRestrictedBoost,
            backgroundFreeBoost,
            request.Character.BackgroundTrainingChoices );
        _characterBuilder.SetClass(
            request.Character.ClassId,
            classKeyAbility,
            request.Character.RogueRacketId,
            request.Character.RogueTrainingChoices,
            request.Character.ClericDoctrineId,
            request.Character.DeityId,
            request.Character.DivineFont,
            request.Character.DivineSanctification,
            request.Character.DeitySkillReplacementId,
            request.Character.HuntersEdgeId,
            request.Character.DruidicOrderId,
            request.Character.BardMuseId,
            request.Character.WitchPatronId,
            request.Character.WitchPatronFamiliarSpellId,
            request.Character.ArcaneSchoolId,
            request.Character.ArcaneThesisId,
            request.Character.ClericDomainId,
            request.Character.ClericCantripIds,
            request.Character.ClericPreparedSpellIds,
            request.Character.BardCantripIds,
            request.Character.BardSpellIds,
            request.Character.DruidCantripIds,
            request.Character.DruidPreparedSpellIds,
            request.Character.WitchFamiliarCantripIds,
            request.Character.WitchFamiliarSpellIds,
            request.Character.WitchPreparedCantripIds,
            request.Character.WitchPreparedSpellIds,
            request.Character.WitchFocusHexId,
            request.Character.WizardSpellbookCantripIds,
            request.Character.WizardSpellbookSpellIds,
            request.Character.WizardCurriculumCantripId,
            request.Character.WizardCurriculumSpellIds,
            request.Character.WizardPreparedCantripIds,
            request.Character.WizardPreparedSpellIds,
            request.Character.WizardPreparedCurriculumCantripId,
            request.Character.WizardPreparedCurriculumSpellId,
            request.Character.ClassFeatChoices );
        _characterBuilder.SetFinalFreeBoosts( finalFreeBoosts );
        _characterBuilder.SetAdditionalLanguages( request.Character.AdditionalLanguageIds );
        _characterBuilder.SetClassTraining(
            request.Character.ClassId,
            request.Character.ClassSkillGrantChoices,
            request.Character.AdditionalClassTrainingChoices );
        _characterBuilder.SetStartingEquipment(
            request.Character.ClassKitOptionIds,
            request.Character.DeityFavoredWeaponEquipmentId );

        DraftCharacter draftCharacter = _characterBuilder.Build();
        account.AddDraftCharacter( draftCharacter );
        await _unitOfWork.Commit();
    }

    private static string? ResolveSpecializationId( CreateCharacterRequestDto character )
    {
        return character.RogueRacketId ??
               character.ClericDoctrineId ??
               character.HuntersEdgeId ??
               character.DruidicOrderId ??
               character.BardMuseId ??
               character.WitchPatronId ??
               character.ArcaneSchoolId;
    }
}
