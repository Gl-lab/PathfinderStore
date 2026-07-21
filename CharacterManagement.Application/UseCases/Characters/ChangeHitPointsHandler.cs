using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class ChangeHitPointsHandler
    : IRequestHandler<ChangeHitPointsCommand, CharacterHitPointStateDto>
{
    private readonly ICharacterRepository _characterRepository;
    private readonly IAncestryRepository _ancestryRepository;
    private readonly ICharacterClassRepository _characterClassRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeHitPointsHandler(
        ICharacterRepository characterRepository,
        IAncestryRepository ancestryRepository,
        ICharacterClassRepository characterClassRepository,
        IUnitOfWork unitOfWork )
    {
        _characterRepository = characterRepository;
        _ancestryRepository = ancestryRepository;
        _characterClassRepository = characterClassRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CharacterHitPointStateDto> Handle(
        ChangeHitPointsCommand request,
        CancellationToken cancellationToken )
    {
        DraftCharacter? character = await _characterRepository.GetByIdAsync(
            request.CharacterId,
            request.UserId );
        if ( character is null )
        {
            throw new CharacterManagementException(
                $"Character {request.CharacterId} was not found for current user." );
        }

        if ( String.IsNullOrWhiteSpace( character.SelectedClassId ) )
        {
            throw new Pathfinder.CharacterManagement.Domain.Exceptions.CharacterManagementException(
                "Character class is required to calculate maximum hit points." );
        }

        Ancestry ancestry = _ancestryRepository.GetAncestry( character.AncestryType );
        CharacterClass characterClass = _characterClassRepository.GetCharacterClass(
            character.SelectedClassId );
        int maximumHitPoints = CharacterHitPoints
            .Calculate( character, ancestry, characterClass )
            .MaximumHitPoints;

        ApplyOperation( character, request.Operation, request.Amount, maximumHitPoints );
        await _unitOfWork.Commit();
        CharacterHitPointState state = character.GetHitPointState( maximumHitPoints );
        return new CharacterHitPointStateDto
        {
            Current = state.Current,
            Temporary = state.Temporary,
            Maximum = state.Maximum,
        };
    }

    private static void ApplyOperation(
        DraftCharacter character,
        HitPointOperation operation,
        int amount,
        int maximumHitPoints )
    {
        switch ( operation )
        {
            case HitPointOperation.ApplyDamage:
                character.ApplyDamage( amount, maximumHitPoints );
                break;
            case HitPointOperation.Restore:
                character.RestoreHitPoints( amount, maximumHitPoints );
                break;
            case HitPointOperation.GrantTemporary:
                character.GrantTemporaryHitPoints( amount, maximumHitPoints );
                break;
            case HitPointOperation.ClearTemporary:
                character.ClearTemporaryHitPoints( maximumHitPoints );
                break;
            default:
                throw new ArgumentOutOfRangeException( nameof( operation ), operation, null );
        }
    }
}
