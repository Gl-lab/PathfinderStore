using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Avatars;

public sealed record AvatarSelectionCriteria(
    AncestryType AncestryType,
    string CharacterClassId,
    CharacterGender Gender,
    string? HeritageId = null,
    string? SpecializationId = null,
    string? BackgroundId = null );
