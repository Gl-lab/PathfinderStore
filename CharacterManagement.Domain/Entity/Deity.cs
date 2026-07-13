namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum DivineFont
{
    Heal,
    Harm
}

public enum DivineSanctification
{
    Holy,
    Unholy
}

public enum FavoredWeaponCategory
{
    Simple,
    Martial,
    Advanced,
    Unarmed
}

public sealed record DeityFavoredWeapon(
    string Id,
    string Name,
    FavoredWeaponCategory Category );

public sealed record DeityGrantedSpell(
    int Rank,
    string Id,
    string Name );

public sealed class Deity
{
    public string Id { get; }
    public string Name { get; }
    public SourceReference Source { get; }
    public bool CanGrantClericPowers { get; }
    public string? DivineSkillId { get; }
    public IReadOnlyList<DeityFavoredWeapon> FavoredWeapons { get; }
    public IReadOnlyList<DivineFont> DivineFontOptions { get; }
    public IReadOnlyList<DivineSanctification> SanctificationOptions { get; }
    public DivineSanctification? RequiredSanctification { get; }
    public IReadOnlyList<string> PrimaryDomainIds { get; }
    public IReadOnlyList<DeityGrantedSpell> GrantedSpells { get; }
    public IReadOnlyList<ProficiencyGrant> ProficiencyGrants { get; }
    public string DivineSkillGrantId => $"{Id}.skill.divine";

    public Deity(
        string id,
        string name,
        SourceReference source,
        bool canGrantClericPowers,
        string? divineSkillId,
        IReadOnlyList<DeityFavoredWeapon> favoredWeapons,
        IReadOnlyList<DivineFont> divineFontOptions,
        IReadOnlyList<DivineSanctification> sanctificationOptions,
        DivineSanctification? requiredSanctification,
        IReadOnlyList<string> primaryDomainIds,
        IReadOnlyList<DeityGrantedSpell> grantedSpells )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "deity.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Deity id must use the 'deity.' prefix.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Deity name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( source );
        ArgumentNullException.ThrowIfNull( favoredWeapons );
        ArgumentNullException.ThrowIfNull( divineFontOptions );
        ArgumentNullException.ThrowIfNull( sanctificationOptions );
        ArgumentNullException.ThrowIfNull( primaryDomainIds );
        ArgumentNullException.ThrowIfNull( grantedSpells );

        if ( canGrantClericPowers &&
             ( String.IsNullOrWhiteSpace( divineSkillId ) ||
               favoredWeapons.Count == 0 ||
               divineFontOptions.Count == 0 ||
               primaryDomainIds.Count == 0 ) )
        {
            throw new ArgumentException( "A Cleric deity must define all devotee benefits." );
        }

        if ( !canGrantClericPowers &&
             ( !String.IsNullOrWhiteSpace( divineSkillId ) ||
               favoredWeapons.Count > 0 ||
               divineFontOptions.Count > 0 ||
               sanctificationOptions.Count > 0 ||
               requiredSanctification.HasValue ||
               primaryDomainIds.Count > 0 ||
               grantedSpells.Count > 0 ) )
        {
            throw new ArgumentException( "A non-Cleric faith cannot define devotee benefits." );
        }

        if ( requiredSanctification.HasValue &&
             !sanctificationOptions.Contains( requiredSanctification.Value ) )
        {
            throw new ArgumentException( "Required sanctification must be an allowed option." );
        }

        Id = id.Trim();
        Name = name.Trim();
        Source = source;
        CanGrantClericPowers = canGrantClericPowers;
        DivineSkillId = divineSkillId?.Trim();
        FavoredWeapons = favoredWeapons.ToArray();
        DivineFontOptions = divineFontOptions.Distinct().ToArray();
        SanctificationOptions = sanctificationOptions.Distinct().ToArray();
        RequiredSanctification = requiredSanctification;
        PrimaryDomainIds = primaryDomainIds.Distinct( StringComparer.Ordinal ).ToArray();
        GrantedSpells = grantedSpells.ToArray();
        ProficiencyGrants = FavoredWeapons
            .Select( weapon => new ProficiencyGrant(
                new ProficiencyTarget(
                    $"proficiency.attack.{weapon.Id}",
                    weapon.Name,
                    ProficiencyCategory.Attack ),
                ProficiencyRank.Trained,
                $"{Id}.proficiency.{weapon.Id}" ) )
            .ToArray();
    }
}
