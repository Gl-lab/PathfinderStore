using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Feats;

namespace Pathfinder.CharacterManagement.Application.UseCases.Feats;

public sealed class GetFeatOptionsHandler : IRequestHandler<GetFeatOptionsCommand, IReadOnlyCollection<FeatDefinitionDto>>
{
    private readonly IFeatRepository _featRepository;

    public GetFeatOptionsHandler( IFeatRepository featRepository )
    {
        _featRepository = featRepository;
    }

    public Task<IReadOnlyCollection<FeatDefinitionDto>> Handle(
        GetFeatOptionsCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<FeatDefinitionDto> feats = FeatCatalogResolver
            .ResolveOptions(
                _featRepository.GetAll(),
                request.Category,
                request.Level,
                request.RequiredTrait )
            .Select( FeatDefinitionDtoMapper.Map )
            .ToArray();

        return Task.FromResult( feats );
    }
}
