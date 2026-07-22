using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Inventory.Domain.Containers;

public sealed class InventoryContainer : Entity, IAggregateRoot
{
    private InventoryContainer()
    {
    }

    public Guid ContainerKey { get; private set; }
    public int CampaignId { get; private set; }
    public InventoryContainerOwnerKind OwnerKind { get; private set; }
    public int OwnerId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static InventoryContainer CreateRoot(
        Guid containerKey,
        int campaignId,
        InventoryContainerOwnerKind ownerKind,
        int ownerId,
        DateTimeOffset createdAtUtc )
    {
        if ( containerKey == Guid.Empty )
        {
            throw new InventoryException( "Inventory container key cannot be empty." );
        }

        if ( campaignId <= 0 )
        {
            throw new InventoryException( "Campaign id must be greater than zero." );
        }

        if ( !Enum.IsDefined( ownerKind ) )
        {
            throw new InventoryException( "Inventory container owner kind is invalid." );
        }

        if ( ownerId <= 0 )
        {
            throw new InventoryException( "Inventory container owner id must be greater than zero." );
        }

        if ( createdAtUtc.Offset != TimeSpan.Zero )
        {
            throw new InventoryException( "Inventory container creation timestamp must use UTC." );
        }

        return new InventoryContainer
        {
            ContainerKey = containerKey,
            CampaignId = campaignId,
            OwnerKind = ownerKind,
            OwnerId = ownerId,
            CreatedAtUtc = createdAtUtc,
        };
    }
}
