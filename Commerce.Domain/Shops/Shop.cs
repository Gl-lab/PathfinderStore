using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Shops;

public sealed class Shop : Entity
{
    public const int NameMaxLength = 200;
    public const int SpecializationMaxLength = 200;

    private Shop()
    {
    }

    public int SettlementId { get; private set; }
    public int CampaignId { get; private set; }
    public string Name { get; private set; } = String.Empty;
    public string Specialization { get; private set; } = String.Empty;
    public int ShopLevel { get; private set; }
    public int CatalogPricePercent { get; private set; }
    public int BuybackPricePercent { get; private set; }
    public int PricingPolicyVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    internal static Shop Create(
        int campaignId,
        string name,
        string specialization,
        int shopLevel,
        DateTimeOffset createdAtUtc )
    {
        string normalizedSpecialization = specialization?.Trim() ?? String.Empty;
        if ( normalizedSpecialization.Length > SpecializationMaxLength )
        {
            throw new CommerceException(
                $"Shop specialization cannot exceed {SpecializationMaxLength} characters." );
        }

        return new Shop
        {
            CampaignId = campaignId,
            Name = name,
            Specialization = normalizedSpecialization,
            ShopLevel = shopLevel,
            CatalogPricePercent = 100,
            BuybackPricePercent = 50,
            PricingPolicyVersion = 1,
            CreatedAtUtc = createdAtUtc,
        };
    }

    public void SetPricingPolicy( int catalogPricePercent, int buybackPricePercent )
    {
        if ( ( catalogPricePercent < 1 ) || ( catalogPricePercent > 1000 ) )
        {
            throw new CommerceException(
                "Catalog price percent must be between 1 and 1000." );
        }

        if ( ( buybackPricePercent < 0 ) || ( buybackPricePercent > 100 ) )
        {
            throw new CommerceException(
                "Buyback price percent must be between 0 and 100." );
        }

        CatalogPricePercent = catalogPricePercent;
        BuybackPricePercent = buybackPricePercent;
        PricingPolicyVersion++;
    }

    public long CalculateCatalogPrice( long basePriceCopper ) =>
        CalculatePrice( basePriceCopper, CatalogPricePercent );

    public long CalculateBuybackPrice( long basePriceCopper ) =>
        CalculatePrice( basePriceCopper, BuybackPricePercent );

    private static long CalculatePrice( long basePriceCopper, int percent )
    {
        if ( basePriceCopper < 0 )
        {
            throw new CommerceException( "Base price cannot be negative." );
        }

        return checked( basePriceCopper * percent ) / 100;
    }
}
