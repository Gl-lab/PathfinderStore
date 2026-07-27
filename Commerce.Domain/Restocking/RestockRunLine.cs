using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Restocking;

public sealed class RestockRunLine : Entity
{
    private RestockRunLine()
    {
    }

    public int RestockRunId { get; private set; }
    public int Sequence { get; private set; }
    public int ItemConfigurationId { get; private set; }
    public int Quantity { get; private set; }
    public long UnitPriceCopper { get; private set; }
    public RestockItemKind Kind { get; private set; }
    public Guid? PublishedOfferKey { get; private set; }
    public Guid? PublishedItemInstanceKey { get; private set; }

    internal static RestockRunLine Create(
        int sequence,
        RestockCandidate candidate )
    {
        ArgumentNullException.ThrowIfNull( candidate );
        if ( sequence <= 0 ||
             candidate.ItemConfigurationId <= 0 ||
             candidate.UnitPriceCopper < 0 ||
             !Enum.IsDefined( candidate.Kind ) )
        {
            throw new CommerceException( "Restock run line is invalid." );
        }

        return new RestockRunLine
        {
            Sequence = sequence,
            ItemConfigurationId = candidate.ItemConfigurationId,
            Quantity = 1,
            UnitPriceCopper = candidate.UnitPriceCopper,
            Kind = candidate.Kind,
        };
    }

    public void Publish( Guid offerKey, Guid? itemInstanceKey )
    {
        if ( PublishedOfferKey is not null )
        {
            if ( PublishedOfferKey != offerKey ||
                 PublishedItemInstanceKey != itemInstanceKey )
            {
                throw new CommerceException(
                    "Restock run line was already published with different identities." );
            }

            return;
        }

        if ( offerKey == Guid.Empty )
        {
            throw new CommerceException( "Published restock offer key cannot be empty." );
        }

        if ( Kind == RestockItemKind.Unique && itemInstanceKey is null )
        {
            throw new CommerceException( "A unique restock line requires an item instance." );
        }

        if ( Kind != RestockItemKind.Unique && itemInstanceKey is not null )
        {
            throw new CommerceException(
                "Only a unique restock line can publish an item instance." );
        }

        PublishedOfferKey = offerKey;
        PublishedItemInstanceKey = itemInstanceKey;
    }
}
