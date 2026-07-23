using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Transactions;

public sealed class ShopSale : Entity, IAggregateRoot
{
    private ShopSale()
    {
    }

    public Guid SaleKey { get; private set; }
    public Guid OperationId { get; private set; }
    public int CampaignId { get; private set; }
    public int ShopId { get; private set; }
    public int SellerCharacterId { get; private set; }
    public Guid ItemInstanceKey { get; private set; }
    public int ItemConfigurationId { get; private set; }
    public int Quantity { get; private set; }
    public long UnitPriceCopper { get; private set; }
    public long TotalPriceCopper { get; private set; }
    public DateTimeOffset CompletedAtUtc { get; private set; }

    public static ShopSale Create(
        Guid operationId,
        int campaignId,
        int shopId,
        int sellerCharacterId,
        Guid itemInstanceKey,
        int itemConfigurationId,
        int quantity,
        long unitPriceCopper,
        DateTimeOffset completedAtUtc )
    {
        if ( ( operationId == Guid.Empty ) || ( itemInstanceKey == Guid.Empty ) )
        {
            throw new CommerceException( "Sale operation and item keys cannot be empty." );
        }

        if ( ( campaignId <= 0 ) ||
             ( shopId <= 0 ) ||
             ( sellerCharacterId <= 0 ) ||
             ( itemConfigurationId <= 0 ) ||
             ( quantity <= 0 ) ||
             ( unitPriceCopper < 0 ) )
        {
            throw new CommerceException( "Sale identifiers, quantity or price are invalid." );
        }

        return new ShopSale
        {
            SaleKey = Guid.NewGuid(),
            OperationId = operationId,
            CampaignId = campaignId,
            ShopId = shopId,
            SellerCharacterId = sellerCharacterId,
            ItemInstanceKey = itemInstanceKey,
            ItemConfigurationId = itemConfigurationId,
            Quantity = quantity,
            UnitPriceCopper = unitPriceCopper,
            TotalPriceCopper = checked( unitPriceCopper * quantity ),
            CompletedAtUtc = completedAtUtc,
        };
    }
}
