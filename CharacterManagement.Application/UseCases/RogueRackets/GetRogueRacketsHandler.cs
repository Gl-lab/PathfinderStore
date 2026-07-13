using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.RogueRackets;

public sealed class GetRogueRacketsHandler : IRequestHandler<GetRogueRacketsCommand, IReadOnlyCollection<RogueRacketDto>>
{
    private readonly IRogueRacketRepository _rogueRacketRepository;

    public GetRogueRacketsHandler( IRogueRacketRepository rogueRacketRepository )
    {
        _rogueRacketRepository = rogueRacketRepository;
    }

    public Task<IReadOnlyCollection<RogueRacketDto>> Handle(
        GetRogueRacketsCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<RogueRacketDto> rackets = _rogueRacketRepository
            .GetAll()
            .Select( RogueRacketDtoMapper.Map )
            .OrderBy( racket => racket.Name )
            .ToArray();

        return Task.FromResult( rackets );
    }
}
