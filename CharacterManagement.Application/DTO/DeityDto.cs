using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class DeityDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
    public bool CanGrantClericPowers { get; set; }
    public string? DivineSkillId { get; set; }
    public IReadOnlyList<DeityFavoredWeaponDto> FavoredWeapons { get; set; } = [];
    public IReadOnlyList<DivineFont> DivineFontOptions { get; set; } = [];
    public IReadOnlyList<DivineSanctification> SanctificationOptions { get; set; } = [];
    public DivineSanctification? RequiredSanctification { get; set; }
    public IReadOnlyList<string> PrimaryDomainIds { get; set; } = [];
    public IReadOnlyList<DeityGrantedSpellDto> GrantedSpells { get; set; } = [];
}

public sealed class DeityFavoredWeaponDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public FavoredWeaponCategory Category { get; set; }
}

public sealed class DeityGrantedSpellDto
{
    public int Rank { get; set; }
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
}

public sealed class DeityPackageDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string DivineSkillId { get; set; } = String.Empty;
    public string? DivineSkillReplacementId { get; set; }
    public IReadOnlyList<DeityFavoredWeaponDto> FavoredWeapons { get; set; } = [];
    public DivineFont DivineFont { get; set; }
    public DivineSanctification? Sanctification { get; set; }
    public IReadOnlyList<string> PrimaryDomainIds { get; set; } = [];
    public IReadOnlyList<DeityGrantedSpellDto> GrantedSpells { get; set; } = [];
}
