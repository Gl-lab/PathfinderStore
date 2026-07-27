using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Restocking;

public sealed class RestockPolicy : Entity, IAggregateRoot
{
    public const int NameMaxLength = 200;

    private readonly List<RestockPolicyRevision> _revisions = [];

    private RestockPolicy()
    {
    }

    public int CampaignId { get; private set; }
    public int ShopId { get; private set; }
    public string Name { get; private set; } = String.Empty;
    public int CurrentVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public IReadOnlyCollection<RestockPolicyRevision> Revisions { get => _revisions; }
    public RestockPolicyRevision CurrentRevision { get => _revisions.Single(
        revision => revision.Version == CurrentVersion ); }

    public static RestockPolicy Create(
        int campaignId,
        int shopId,
        string name,
        int targetOfferCount,
        RestockPolicyConstraints constraints,
        int createdByUserId,
        DateTimeOffset createdAtUtc )
    {
        if ( campaignId <= 0 || shopId <= 0 )
        {
            throw new CommerceException( "Restock policy campaign and shop ids must be greater than zero." );
        }

        string normalizedName = name?.Trim() ?? String.Empty;
        if ( String.IsNullOrWhiteSpace( normalizedName ) )
        {
            throw new CommerceException( "Restock policy name cannot be empty." );
        }

        if ( normalizedName.Length > NameMaxLength )
        {
            throw new CommerceException(
                $"Restock policy name cannot exceed {NameMaxLength} characters." );
        }

        RestockPolicy policy = new RestockPolicy
        {
            CampaignId = campaignId,
            ShopId = shopId,
            Name = normalizedName,
            CurrentVersion = 0,
            CreatedAtUtc = createdAtUtc,
        };
        policy.AddRevision( targetOfferCount, constraints, createdByUserId, createdAtUtc );
        return policy;
    }

    public RestockPolicyRevision Revise(
        int expectedVersion,
        int targetOfferCount,
        RestockPolicyConstraints constraints,
        int createdByUserId,
        DateTimeOffset createdAtUtc )
    {
        if ( expectedVersion != CurrentVersion )
        {
            throw new CommerceException( "Restock policy version conflict." );
        }

        return AddRevision( targetOfferCount, constraints, createdByUserId, createdAtUtc );
    }

    private RestockPolicyRevision AddRevision(
        int targetOfferCount,
        RestockPolicyConstraints constraints,
        int createdByUserId,
        DateTimeOffset createdAtUtc )
    {
        RestockPolicyRevision revision = RestockPolicyRevision.Create(
            CurrentVersion + 1,
            targetOfferCount,
            constraints,
            createdByUserId,
            createdAtUtc );
        _revisions.Add( revision );
        CurrentVersion = revision.Version;
        return revision;
    }
}
