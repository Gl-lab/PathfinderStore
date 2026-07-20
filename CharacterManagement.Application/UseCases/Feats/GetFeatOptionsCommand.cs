using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Feats;

public sealed record GetFeatOptionsCommand(
    FeatCategory Category,
    int Level,
    string? RequiredTrait ) : IRequest<IReadOnlyCollection<FeatDefinitionDto>>;
