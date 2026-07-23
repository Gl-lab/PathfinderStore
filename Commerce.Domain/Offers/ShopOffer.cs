using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Offers;

public sealed class ShopOffer : Entity, IAggregateRoot
{
    private ShopOffer()
    {
    }

    public Guid OfferKey { get; private set; }
    public int CampaignId { get; private set; }
    public int ShopId { get; private set; }
    public ShopOfferKind Kind { get; private set; }
    public int? ItemConfigurationId { get; private set; }
    public Guid? ItemInstanceKey { get; private set; }
    public int AvailableQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public long UnitPriceCopper { get; private set; }
    public ShopOfferStatus Status { get; private set; }
    public int Version { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static ShopOffer CreateCatalog(
        int campaignId,
        int shopId,
        int itemConfigurationId,
        int availableQuantity,
        long unitPriceCopper,
        DateTimeOffset createdAtUtc ) => Create(
        campaignId,
        shopId,
        ShopOfferKind.Catalog,
        itemConfigurationId,
        null,
        availableQuantity,
        unitPriceCopper,
        createdAtUtc );

    public static ShopOffer CreateStockInstance(
        int campaignId,
        int shopId,
        Guid itemInstanceKey,
        int availableQuantity,
        long unitPriceCopper,
        DateTimeOffset createdAtUtc ) => Create(
        campaignId,
        shopId,
        ShopOfferKind.StockInstance,
        null,
        itemInstanceKey,
        availableQuantity,
        unitPriceCopper,
        createdAtUtc );

    private static ShopOffer Create(
        int campaignId,
        int shopId,
        ShopOfferKind kind,
        int? itemConfigurationId,
        Guid? itemInstanceKey,
        int availableQuantity,
        long unitPriceCopper,
        DateTimeOffset createdAtUtc )
    {
        if ( ( campaignId <= 0 ) || ( shopId <= 0 ) )
        {
            throw new CommerceException( "Campaign and shop ids must be greater than zero." );
        }

        if ( availableQuantity <= 0 )
        {
            throw new CommerceException( "Offer quantity must be greater than zero." );
        }

        if ( unitPriceCopper < 0 )
        {
            throw new CommerceException( "Offer price cannot be negative." );
        }

        if ( createdAtUtc.Offset != TimeSpan.Zero )
        {
            throw new CommerceException( "Offer creation timestamp must use UTC." );
        }

        return new ShopOffer
        {
            OfferKey = Guid.NewGuid(),
            CampaignId = campaignId,
            ShopId = shopId,
            Kind = kind,
            ItemConfigurationId = itemConfigurationId,
            ItemInstanceKey = itemInstanceKey,
            AvailableQuantity = availableQuantity,
            ReservedQuantity = 0,
            UnitPriceCopper = unitPriceCopper,
            Status = ShopOfferStatus.Active,
            Version = 0,
            CreatedAtUtc = createdAtUtc,
        };
    }

    public void Reserve( int quantity )
    {
        if ( Status != ShopOfferStatus.Active )
        {
            throw new CommerceException( "Only an active offer can be reserved." );
        }

        if ( ( quantity <= 0 ) || ( quantity > ( AvailableQuantity - ReservedQuantity ) ) )
        {
            throw new CommerceException( "Offer has insufficient unreserved quantity." );
        }

        ReservedQuantity += quantity;
        Version++;
    }

    public void Release( int quantity )
    {
        if ( ( quantity <= 0 ) || ( quantity > ReservedQuantity ) )
        {
            throw new CommerceException( "Offer reservation quantity is invalid." );
        }

        ReservedQuantity -= quantity;
        Version++;
    }
}
