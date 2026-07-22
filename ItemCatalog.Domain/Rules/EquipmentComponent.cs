using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Rules;

public sealed class EquipmentComponent : Entity
{
    private EquipmentComponent()
    {
    }

    public int ItemRevisionId { get; private set; }
    public EquipmentUsage Usage { get; private set; }
    public int RequiredHands { get; private set; }

    public static EquipmentComponent Create( EquipmentUsage usage, int requiredHands )
    {
        if ( !Enum.IsDefined( usage ) )
        {
            throw new ItemCatalogException( "Equipment usage is invalid." );
        }

        if ( ( requiredHands < 0 ) || ( requiredHands > 2 ) )
        {
            throw new ItemCatalogException( "Equipment required hands must be between zero and two." );
        }

        return new EquipmentComponent
        {
            Usage = usage,
            RequiredHands = requiredHands,
        };
    }
}