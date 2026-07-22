using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Inventory.Domain.Audit;

public sealed class InventoryAuditEntry : Entity, IAggregateRoot
{
    public const int ReasonMaxLength = 500;

    private InventoryAuditEntry()
    {
    }

    public Guid AuditKey { get; private set; }
    public int CampaignId { get; private set; }
    public Guid OperationId { get; private set; }
    public InventoryAuditActionKind ActionKind { get; private set; }
    public int ActorUserId { get; private set; }
    public bool IsForced { get; private set; }
    public string Reason { get; private set; } = String.Empty;
    public Guid? ItemInstanceKey { get; private set; }
    public Guid? RelatedKey { get; private set; }
    public DateTimeOffset OccurredAtUtc { get; private set; }

    public static InventoryAuditEntry Create(
        Guid auditKey,
        int campaignId,
        Guid operationId,
        InventoryAuditActionKind actionKind,
        int actorUserId,
        bool isForced,
        string reason,
        Guid? itemInstanceKey,
        Guid? relatedKey,
        DateTimeOffset occurredAtUtc )
    {
        if ( ( auditKey == Guid.Empty ) || ( operationId == Guid.Empty ) )
        {
            throw new InventoryException( "Audit and operation keys cannot be empty." );
        }

        if ( ( campaignId <= 0 ) || ( actorUserId <= 0 ) || !Enum.IsDefined( actionKind ) )
        {
            throw new InventoryException( "Inventory audit identity is invalid." );
        }

        if ( String.IsNullOrWhiteSpace( reason ) )
        {
            throw new InventoryException( "Inventory audit reason cannot be empty." );
        }

        string normalizedReason = reason?.Trim() ?? String.Empty;
        if ( normalizedReason.Length > ReasonMaxLength )
        {
            throw new InventoryException(
                $"Inventory audit reason cannot exceed {ReasonMaxLength} characters." );
        }

        if ( occurredAtUtc.Offset != TimeSpan.Zero )
        {
            throw new InventoryException( "Inventory audit timestamp must use UTC." );
        }

        return new InventoryAuditEntry
        {
            AuditKey = auditKey,
            CampaignId = campaignId,
            OperationId = operationId,
            ActionKind = actionKind,
            ActorUserId = actorUserId,
            IsForced = isForced,
            Reason = normalizedReason,
            ItemInstanceKey = itemInstanceKey,
            RelatedKey = relatedKey,
            OccurredAtUtc = occurredAtUtc,
        };
    }

    public void EnsureMatches(
        int campaignId,
        Guid operationId,
        InventoryAuditActionKind actionKind,
        int actorUserId,
        bool isForced,
        string reason,
        Guid? itemInstanceKey,
        Guid? relatedKey )
    {
        string normalizedReason = reason?.Trim() ?? String.Empty;
        if ( ( CampaignId != campaignId ) ||
             ( OperationId != operationId ) ||
             ( ActionKind != actionKind ) ||
             ( ActorUserId != actorUserId ) ||
             ( IsForced != isForced ) ||
             !String.Equals( Reason, normalizedReason, StringComparison.Ordinal ) ||
             ( ItemInstanceKey != itemInstanceKey ) ||
             ( RelatedKey != relatedKey ) )
        {
            throw new InventoryException( "Audit operation was already used for another action." );
        }
    }
}
