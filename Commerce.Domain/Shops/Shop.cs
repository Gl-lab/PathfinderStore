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
            CreatedAtUtc = createdAtUtc,
        };
    }
}
