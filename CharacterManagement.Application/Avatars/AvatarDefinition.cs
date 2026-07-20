using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Avatars;

public sealed record AvatarDefinition(
    AvatarId Id,
    string Path,
    AncestryType? AncestryType = null,
    string? CharacterClassId = null,
    CharacterGender? Gender = null,
    string? HeritageId = null,
    string? SpecializationId = null,
    string? BackgroundId = null,
    int? Variant = null );
