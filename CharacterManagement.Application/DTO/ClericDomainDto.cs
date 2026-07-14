using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class SpellReferenceDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public int Rank { get; set; }
    public SpellKind Kind { get; set; }
}

public sealed class ClericDomainDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
    public SpellReferenceDto InitialFocusSpell { get; set; } = new SpellReferenceDto();
}

public sealed class ClericDomainPackageDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public SpellReferenceDto InitialFocusSpell { get; set; } = new SpellReferenceDto();
}
