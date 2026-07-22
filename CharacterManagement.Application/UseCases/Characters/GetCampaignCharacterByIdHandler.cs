using MediatR;
using Pathfinder.CharacterManagement.Application.Access;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.Features.Characters.Queries.Mapping;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class GetCampaignCharacterByIdHandler
    : IRequestHandler<GetCampaignCharacterByIdCommand, CampaignCharacterDto>
{
    private readonly ICharacterRepository _characterRepository;
    private readonly CharacterDetailsDtoMapper _characterDetailsDtoMapper;
    private readonly ICharacterCampaignAccessPolicy _accessPolicy;

    public GetCampaignCharacterByIdHandler(
        ICharacterRepository characterRepository,
        CharacterDetailsDtoMapper characterDetailsDtoMapper,
        ICharacterCampaignAccessPolicy accessPolicy )
    {
        _characterRepository = characterRepository;
        _characterDetailsDtoMapper = characterDetailsDtoMapper;
        _accessPolicy = accessPolicy;
    }

    public async Task<CampaignCharacterDto> Handle(
        GetCampaignCharacterByIdCommand request,
        CancellationToken cancellationToken )
    {
        CharacterCampaignAccess access = await _accessPolicy.GetAccessAsync(
            request.CampaignId,
            request.UserId,
            request.CharacterId,
            cancellationToken );
        if ( !access.CanView )
        {
            throw new CharacterManagementException( "Campaign character was not found." );
        }

        DraftCharacter? character = await _characterRepository.GetByIdAsync( request.CharacterId );
        if ( character is null )
        {
            throw new CharacterManagementException( "Campaign character was not found." );
        }

        return new CampaignCharacterDto(
            _characterDetailsDtoMapper.Convert( character, request.CampaignId ),
            access.CanAct );
    }
}
