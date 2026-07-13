namespace Pathfinder.CharacterManagement.Domain.Entity;

public sealed class SkillDefinition
{
    public string Id { get; }
    public string Name { get; }
    public AbilityType KeyAbility { get; }
    public SourceReference Source { get; }

    public SkillDefinition(
        string id,
        string name,
        AbilityType keyAbility,
        SourceReference source )
    {
        if ( String.IsNullOrWhiteSpace( id ) || !id.StartsWith( "skill.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Skill id must use the 'skill.' prefix.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Skill name cannot be empty.", nameof( name ) );
        }

        if ( !Enum.IsDefined( keyAbility ) )
        {
            throw new ArgumentOutOfRangeException( nameof( keyAbility ), keyAbility, null );
        }

        ArgumentNullException.ThrowIfNull( source );

        Id = id.Trim();
        Name = name.Trim();
        KeyAbility = keyAbility;
        Source = source;
    }
}
