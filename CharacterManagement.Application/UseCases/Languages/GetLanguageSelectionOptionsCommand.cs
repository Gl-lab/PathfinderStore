using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Languages;

public sealed record GetLanguageSelectionOptionsCommand(
    AncestryType AncestryType,
    int IntelligenceScore ) : IRequest<LanguageSelectionOptionsDto>;
