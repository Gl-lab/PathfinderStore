using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Rules;

public sealed class ShieldComponent : Entity
{
    private ShieldComponent()
    {
    }

    public int ItemRevisionId { get; private set; }
    public int RaisedArmorClassBonus { get; private set; }

    public static ShieldComponent Create( int raisedArmorClassBonus )
    {
        if ( raisedArmorClassBonus <= 0 )
        {
            throw new ItemCatalogException( "Raised shield Armor Class bonus must be greater than zero." );
        }

        return new ShieldComponent
        {
            RaisedArmorClassBonus = raisedArmorClassBonus,
        };
    }
}