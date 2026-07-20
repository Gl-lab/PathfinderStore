using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Spells;

public sealed record GetSpellOptionsCommand(
    SpellTradition Tradition,
    int Rank,
    SpellKind Kind ) : IRequest<IReadOnlyCollection<SpellDefinitionDto>>;
