using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Rules;

public sealed class AttackComponent : Entity
{
    public const int NameMaxLength = 100;

    private AttackComponent()
    {
    }

    public int ItemRevisionId { get; private set; }
    public string Name { get; private set; } = String.Empty;
    public int DamageDieCount { get; private set; }
    public DamageDieSize DamageDieSize { get; private set; }
    public ItemDamageType DamageType { get; private set; }
    public int Hands { get; private set; }
    public int? RangeIncrementFeet { get; private set; }

    public static AttackComponent Create(
        string name,
        int damageDieCount,
        DamageDieSize damageDieSize,
        ItemDamageType damageType,
        int hands,
        int? rangeIncrementFeet = null )
    {
        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ItemCatalogException( "Attack name cannot be empty." );
        }

        string normalizedName = name.Trim();
        if ( normalizedName.Length > NameMaxLength )
        {
            throw new ItemCatalogException( $"Attack name cannot exceed {NameMaxLength} characters." );
        }

        if ( damageDieCount <= 0 )
        {
            throw new ItemCatalogException( "Attack damage die count must be greater than zero." );
        }

        if ( !Enum.IsDefined( damageDieSize ) || !Enum.IsDefined( damageType ) )
        {
            throw new ItemCatalogException( "Attack damage definition is invalid." );
        }

        if ( ( hands < 0 ) || ( hands > 2 ) )
        {
            throw new ItemCatalogException( "Attack hands must be between zero and two." );
        }

        if ( rangeIncrementFeet is <= 0 )
        {
            throw new ItemCatalogException( "Attack range increment must be greater than zero." );
        }

        return new AttackComponent
        {
            Name = normalizedName,
            DamageDieCount = damageDieCount,
            DamageDieSize = damageDieSize,
            DamageType = damageType,
            Hands = hands,
            RangeIncrementFeet = rangeIncrementFeet,
        };
    }
}