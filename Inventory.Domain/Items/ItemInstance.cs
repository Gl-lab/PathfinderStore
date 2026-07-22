using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Movements;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Inventory.Domain.Items;

public sealed class ItemInstance : Entity, IAggregateRoot
{
    public const int CustomNameMaxLength = 200;
    public const int MovementReasonMaxLength = 200;
    public const int PerformedByMaxLength = 200;

    private readonly List<InventoryMovement> _movements = [];

    private ItemInstance()
    {
    }

    public Guid InstanceKey { get; private set; }
    public int CampaignId { get; private set; }
    public int ItemConfigurationId { get; private set; }
    public string? CustomName { get; private set; }
    public bool IsStackable { get; private set; }
    public int Quantity { get; private set; }
    public bool IsDepleted => Quantity == 0;
    public Guid CurrentContainerKey { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public IReadOnlyList<InventoryMovement> Movements { get => _movements.AsReadOnly(); }

    public static ItemInstance Create(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        InventoryContainer initialContainer,
        string? customName,
        DateTimeOffset createdAtUtc )
    {
        return CreateCore(
            instanceKey,
            campaignId,
            itemConfigurationId,
            initialContainer,
            customName,
            false,
            1,
            createdAtUtc );
    }

    public static ItemInstance CreateStack(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        int quantity,
        InventoryContainer initialContainer,
        string? customName,
        DateTimeOffset createdAtUtc )
    {
        if ( quantity <= 0 )
        {
            throw new InventoryException( "Item stack quantity must be greater than zero." );
        }

        return CreateCore(
            instanceKey,
            campaignId,
            itemConfigurationId,
            initialContainer,
            customName,
            true,
            quantity,
            createdAtUtc );
    }

    public ItemInstance Split(
        Guid newInstanceKey,
        int splitQuantity,
        DateTimeOffset createdAtUtc )
    {
        EnsureActiveStack();
        if ( ( splitQuantity <= 0 ) || ( splitQuantity >= Quantity ) )
        {
            throw new InventoryException(
                "Split quantity must be greater than zero and less than the current quantity." );
        }

        if ( newInstanceKey == InstanceKey )
        {
            throw new InventoryException(
                "A split item stack must have a different instance key." );
        }

        if ( createdAtUtc.Offset != TimeSpan.Zero )
        {
            throw new InventoryException( "Split timestamp must use UTC." );
        }

        if ( createdAtUtc < CreatedAtUtc )
        {
            throw new InventoryException(
                "Split timestamp cannot precede item instance creation." );
        }

        ItemInstance split = CreateCore(
            newInstanceKey,
            CampaignId,
            ItemConfigurationId,
            CurrentContainerKey,
            CustomName,
            true,
            splitQuantity,
            createdAtUtc );
        Quantity -= splitQuantity;
        return split;
    }

    public void MergeFrom( ItemInstance source )
    {
        ArgumentNullException.ThrowIfNull( source );
        EnsureActiveStack();
        source.EnsureActiveStack();

        if ( InstanceKey == source.InstanceKey )
        {
            throw new InventoryException( "An item stack cannot be merged with itself." );
        }

        if ( ( CampaignId != source.CampaignId ) ||
             ( ItemConfigurationId != source.ItemConfigurationId ) ||
             ( CurrentContainerKey != source.CurrentContainerKey ) ||
             !String.Equals( CustomName, source.CustomName, StringComparison.Ordinal ) )
        {
            throw new InventoryException(
                "Item stacks must have the same campaign, configuration, location, and custom name." );
        }

        if ( Quantity > ( Int32.MaxValue - source.Quantity ) )
        {
            throw new InventoryException( "Merged item stack quantity is too large." );
        }

        Quantity += source.Quantity;
        source.Quantity = 0;
    }

    public InventoryMovement MoveTo(
        InventoryContainer destination,
        string reason,
        Guid operationId,
        string performedBy,
        DateTimeOffset occurredAtUtc )
    {
        ArgumentNullException.ThrowIfNull( destination );
        if ( IsDepleted )
        {
            throw new InventoryException( "A depleted item instance cannot be moved." );
        }

        if ( destination.CampaignId != CampaignId )
        {
            throw new InventoryException(
                "An item instance cannot move to a container in another campaign." );
        }

        if ( destination.ContainerKey == CurrentContainerKey )
        {
            throw new InventoryException( "Item instance is already in the destination container." );
        }

        if ( operationId == Guid.Empty )
        {
            throw new InventoryException( "Movement operation id cannot be empty." );
        }

        string normalizedReason = NormalizeRequiredText(
            reason,
            MovementReasonMaxLength,
            "Movement reason" );
        string normalizedPerformedBy = NormalizeRequiredText(
            performedBy,
            PerformedByMaxLength,
            "Movement performer" );
        EnsureMovementTimestamp( occurredAtUtc );

        InventoryMovement movement = InventoryMovement.Create(
            InstanceKey,
            CurrentContainerKey,
            destination.ContainerKey,
            Quantity,
            normalizedReason,
            operationId,
            normalizedPerformedBy,
            occurredAtUtc );
        CurrentContainerKey = destination.ContainerKey;
        _movements.Add( movement );
        return movement;
    }

    private static ItemInstance CreateCore(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        InventoryContainer initialContainer,
        string? customName,
        bool isStackable,
        int quantity,
        DateTimeOffset createdAtUtc )
    {
        ArgumentNullException.ThrowIfNull( initialContainer );
        if ( initialContainer.CampaignId != campaignId )
        {
            throw new InventoryException(
                "Item instance and initial container must belong to the same campaign." );
        }

        return CreateCore(
            instanceKey,
            campaignId,
            itemConfigurationId,
            initialContainer.ContainerKey,
            customName,
            isStackable,
            quantity,
            createdAtUtc );
    }

    private static ItemInstance CreateCore(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        Guid currentContainerKey,
        string? customName,
        bool isStackable,
        int quantity,
        DateTimeOffset createdAtUtc )
    {
        if ( instanceKey == Guid.Empty )
        {
            throw new InventoryException( "Item instance key cannot be empty." );
        }

        if ( campaignId <= 0 )
        {
            throw new InventoryException( "Campaign id must be greater than zero." );
        }

        if ( itemConfigurationId <= 0 )
        {
            throw new InventoryException( "Item configuration id must be greater than zero." );
        }

        if ( createdAtUtc.Offset != TimeSpan.Zero )
        {
            throw new InventoryException( "Item instance creation timestamp must use UTC." );
        }

        string? normalizedCustomName = NormalizeCustomName( customName );
        return new ItemInstance
        {
            InstanceKey = instanceKey,
            CampaignId = campaignId,
            ItemConfigurationId = itemConfigurationId,
            CustomName = normalizedCustomName,
            IsStackable = isStackable,
            Quantity = quantity,
            CurrentContainerKey = currentContainerKey,
            CreatedAtUtc = createdAtUtc,
        };
    }

    private void EnsureMovementTimestamp( DateTimeOffset occurredAtUtc )
    {
        if ( occurredAtUtc.Offset != TimeSpan.Zero )
        {
            throw new InventoryException( "Movement timestamp must use UTC." );
        }

        DateTimeOffset earliestTimestamp = _movements.Count == 0
            ? CreatedAtUtc
            : _movements[ _movements.Count - 1 ].OccurredAtUtc;
        if ( occurredAtUtc < earliestTimestamp )
        {
            throw new InventoryException(
                "Movement timestamp cannot precede the item history." );
        }
    }

    private static string NormalizeRequiredText(
        string value,
        int maximumLength,
        string fieldName )
    {
        if ( String.IsNullOrWhiteSpace( value ) )
        {
            throw new InventoryException( $"{fieldName} cannot be empty." );
        }

        string normalizedValue = value.Trim();
        if ( normalizedValue.Length > maximumLength )
        {
            throw new InventoryException(
                $"{fieldName} cannot exceed {maximumLength} characters." );
        }

        return normalizedValue;
    }

    private void EnsureActiveStack()
    {
        if ( !IsStackable )
        {
            throw new InventoryException( "This item instance cannot be stacked." );
        }

        if ( IsDepleted )
        {
            throw new InventoryException( "A depleted item stack cannot be changed." );
        }
    }

    private static string? NormalizeCustomName( string? customName )
    {
        if ( String.IsNullOrWhiteSpace( customName ) )
        {
            return null;
        }

        string normalizedCustomName = customName.Trim();
        if ( normalizedCustomName.Length > CustomNameMaxLength )
        {
            throw new InventoryException(
                $"Item instance custom name cannot exceed {CustomNameMaxLength} characters." );
        }

        return normalizedCustomName;
    }
}
