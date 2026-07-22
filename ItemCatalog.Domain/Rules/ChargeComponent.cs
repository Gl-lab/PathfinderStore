using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Rules;

public sealed class ChargeComponent : Entity
{
    private ChargeComponent()
    {
    }

    public int ItemRevisionId { get; private set; }
    public int MaximumCharges { get; private set; }
    public int DefaultActivationCost { get; private set; }
    public ChargeRecoveryRule RecoveryRule { get; private set; }

    public static ChargeComponent Create(
        int maximumCharges,
        int defaultActivationCost,
        ChargeRecoveryRule recoveryRule )
    {
        if ( maximumCharges <= 0 )
        {
            throw new ItemCatalogException( "Maximum charges must be greater than zero." );
        }

        if ( ( defaultActivationCost <= 0 ) || ( defaultActivationCost > maximumCharges ) )
        {
            throw new ItemCatalogException(
                "Default activation cost must be positive and cannot exceed maximum charges." );
        }

        if ( !Enum.IsDefined( recoveryRule ) )
        {
            throw new ItemCatalogException( "Charge recovery rule is invalid." );
        }

        return new ChargeComponent
        {
            MaximumCharges = maximumCharges,
            DefaultActivationCost = defaultActivationCost,
            RecoveryRule = recoveryRule,
        };
    }
}