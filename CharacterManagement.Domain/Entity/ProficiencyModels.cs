namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum ProficiencyRank
{
    Untrained,
    Trained,
    Expert,
    Master,
    Legendary
}

public enum ProficiencyCategory
{
    Perception,
    SavingThrow,
    Attack,
    Defense,
    ClassDc
}

public sealed record ProficiencyTarget
{
    public string Id { get; }
    public string Name { get; }
    public ProficiencyCategory Category { get; }

    public ProficiencyTarget(
        string id,
        string name,
        ProficiencyCategory category )
    {
        if ( String.IsNullOrWhiteSpace( id ) )
        {
            throw new ArgumentException( "Proficiency target id cannot be empty.", nameof( id ) );
        }

        if ( !id.StartsWith( "proficiency.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                "Proficiency target id must start with 'proficiency.'.",
                nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Proficiency target name cannot be empty.", nameof( name ) );
        }

        if ( !Enum.IsDefined( category ) )
        {
            throw new ArgumentOutOfRangeException( nameof( category ), category, null );
        }

        Id = id.Trim();
        Name = name.Trim();
        Category = category;
    }
}

public sealed record ProficiencyGrant
{
    public ProficiencyTarget Target { get; }
    public ProficiencyRank Rank { get; }
    public string SourceGrantId { get; }

    public ProficiencyGrant(
        ProficiencyTarget target,
        ProficiencyRank rank,
        string sourceGrantId )
    {
        ArgumentNullException.ThrowIfNull( target );

        if ( rank == ProficiencyRank.Untrained )
        {
            throw new ArgumentException(
                "Untrained proficiency is represented by the absence of a grant.",
                nameof( rank ) );
        }

        if ( !Enum.IsDefined( rank ) )
        {
            throw new ArgumentOutOfRangeException( nameof( rank ) );
        }

        if ( String.IsNullOrWhiteSpace( sourceGrantId ) )
        {
            throw new ArgumentException( "Proficiency source grant id cannot be empty.", nameof( sourceGrantId ) );
        }

        Target = target;
        Rank = rank;
        SourceGrantId = sourceGrantId.Trim();
    }
}

public static class ProficiencyTargets
{
    public static ProficiencyTarget Perception { get; } = new ProficiencyTarget(
        "proficiency.perception",
        "Perception",
        ProficiencyCategory.Perception );

    public static ProficiencyTarget Fortitude { get; } = new ProficiencyTarget(
        "proficiency.save.fortitude",
        "Fortitude",
        ProficiencyCategory.SavingThrow );

    public static ProficiencyTarget Reflex { get; } = new ProficiencyTarget(
        "proficiency.save.reflex",
        "Reflex",
        ProficiencyCategory.SavingThrow );

    public static ProficiencyTarget Will { get; } = new ProficiencyTarget(
        "proficiency.save.will",
        "Will",
        ProficiencyCategory.SavingThrow );

    public static ProficiencyTarget SimpleWeapons { get; } = new ProficiencyTarget(
        "proficiency.attack.simple_weapons",
        "Simple Weapons",
        ProficiencyCategory.Attack );

    public static ProficiencyTarget MartialWeapons { get; } = new ProficiencyTarget(
        "proficiency.attack.martial_weapons",
        "Martial Weapons",
        ProficiencyCategory.Attack );

    public static ProficiencyTarget AdvancedWeapons { get; } = new ProficiencyTarget(
        "proficiency.attack.advanced_weapons",
        "Advanced Weapons",
        ProficiencyCategory.Attack );

    public static ProficiencyTarget UnarmedAttacks { get; } = new ProficiencyTarget(
        "proficiency.attack.unarmed",
        "Unarmed Attacks",
        ProficiencyCategory.Attack );

    public static ProficiencyTarget UnarmoredDefense { get; } = new ProficiencyTarget(
        "proficiency.defense.unarmored",
        "Unarmored Defense",
        ProficiencyCategory.Defense );

    public static ProficiencyTarget LightArmor { get; } = new ProficiencyTarget(
        "proficiency.defense.light_armor",
        "Light Armor",
        ProficiencyCategory.Defense );

    public static ProficiencyTarget MediumArmor { get; } = new ProficiencyTarget(
        "proficiency.defense.medium_armor",
        "Medium Armor",
        ProficiencyCategory.Defense );

    public static ProficiencyTarget HeavyArmor { get; } = new ProficiencyTarget(
        "proficiency.defense.heavy_armor",
        "Heavy Armor",
        ProficiencyCategory.Defense );

    public static ProficiencyTarget ClassDc( string characterClassId, string characterClassName )
    {
        if ( String.IsNullOrWhiteSpace( characterClassId ) )
        {
            throw new ArgumentException( "Character class id cannot be empty.", nameof( characterClassId ) );
        }

        if ( !characterClassId.StartsWith( "class.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                "Character class id must start with 'class.'.",
                nameof( characterClassId ) );
        }

        if ( String.IsNullOrWhiteSpace( characterClassName ) )
        {
            throw new ArgumentException( "Character class name cannot be empty.", nameof( characterClassName ) );
        }

        string classSuffix = characterClassId[ "class.".Length.. ];

        if ( String.IsNullOrWhiteSpace( classSuffix ) )
        {
            throw new ArgumentException(
                "Character class id must contain a stable suffix.",
                nameof( characterClassId ) );
        }

        return new ProficiencyTarget(
            $"proficiency.class_dc.{classSuffix}",
            $"{characterClassName.Trim()} Class DC",
            ProficiencyCategory.ClassDc );
    }
}
