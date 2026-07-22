using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Movements;
using Pathfinder.Inventory.Domain.Operations;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Inventory.Domain.Items;

public sealed class ItemInstance : Entity, IAggregateRoot
{
    public const int CustomNameMaxLength = 200;
    public const int MovementReasonMaxLength = 200;
    public const int PerformedByMaxLength = 200;

    private readonly List<InventoryMovement> _movements = [];
    private readonly List<InventoryOperation> _operations = [];

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
    public int Version { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public IReadOnlyList<InventoryMovement> Movements { get => _movements.AsReadOnly(); }
    public IReadOnlyList<InventoryOperation> Operations { get => _operations.AsReadOnly(); }

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

    public ItemSplitResult Split(
        Guid newInstanceKey,
        int splitQuantity,
        int expectedVersion,
        Guid operationId,
        DateTimeOffset createdAtUtc )
    {
        EnsureOperationId( operationId );
        InventoryOperation? replay = FindOperation( operationId );
        if ( replay is not null )
        {
            replay.EnsureMatches(
                InventoryOperationKind.Split,
                newInstanceKey,
                splitQuantity );
            return ItemSplitResult.Replay( newInstanceKey );
        }

        EnsureExpectedVersion( expectedVersion );
        EnsureOperationTimestamp( createdAtUtc );
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
        Version++;
        _operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.Split,
            newInstanceKey,
            splitQuantity,
            Version,
            createdAtUtc ) );
        return ItemSplitResult.Applied( split );
    }

    public bool MergeFrom(
        ItemInstance source,
        int expectedVersion,
        int sourceExpectedVersion,
        Guid operationId,
        DateTimeOffset occurredAtUtc )
    {
        ArgumentNullException.ThrowIfNull( source );
        EnsureOperationId( operationId );
        InventoryOperation? replay = FindOperation( operationId );
        InventoryOperation? sourceReplay = source.FindOperation( operationId );
        if ( ( replay is not null ) || ( sourceReplay is not null ) )
        {
            if ( ( replay is null ) || ( sourceReplay is null ) )
            {
                throw new InventoryException(
                    "Merge operation history is inconsistent between item stacks." );
            }

            replay.EnsureMatches(
                InventoryOperationKind.Merge,
                source.InstanceKey,
                sourceReplay.Quantity );
            sourceReplay.EnsureMatches(
                InventoryOperationKind.Merge,
                InstanceKey,
                replay.Quantity );
            return false;
        }

        EnsureExpectedVersion( expectedVersion );
        source.EnsureExpectedVersion( sourceExpectedVersion );
        EnsureOperationTimestamp( occurredAtUtc );
        source.EnsureOperationTimestamp( occurredAtUtc );
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

        int transferredQuantity = source.Quantity;
        Quantity += transferredQuantity;
        source.Quantity = 0;
        Version++;
        source.Version++;
        _operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.Merge,
            source.InstanceKey,
            transferredQuantity,
            Version,
            occurredAtUtc ) );
        source._operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.Merge,
            InstanceKey,
            transferredQuantity,
            source.Version,
            occurredAtUtc ) );
        return true;
    }

    public InventoryMovement MoveTo(
        InventoryContainer destination,
        string reason,
        int expectedVersion,
        Guid operationId,
        string performedBy,
        DateTimeOffset occurredAtUtc )
    {
        ArgumentNullException.ThrowIfNull( destination );
        EnsureOperationId( operationId );
        InventoryMovement? replay = _movements.SingleOrDefault(
            movement => movement.OperationId == operationId );
        if ( replay is not null )
        {
            string replayReason = NormalizeRequiredText(
                reason,
                MovementReasonMaxLength,
                "Movement reason" );
            string replayPerformedBy = NormalizeRequiredText(
                performedBy,
                PerformedByMaxLength,
                "Movement performer" );
            if ( ( replay.ToContainerKey != destination.ContainerKey ) ||
                 !String.Equals( replay.Reason, replayReason, StringComparison.Ordinal ) ||
                 !String.Equals( replay.PerformedBy, replayPerformedBy, StringComparison.Ordinal ) ||
                 ( replay.OccurredAtUtc != occurredAtUtc ) )
            {
                throw new InventoryException(
                    "Operation id was already used for a different inventory movement." );
            }

            return replay;
        }

        if ( FindOperation( operationId ) is not null )
        {
            throw new InventoryException(
                "Operation id was already used for a different inventory change." );
        }

        EnsureExpectedVersion( expectedVersion );
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

        string normalizedReason = NormalizeRequiredText(
            reason,
            MovementReasonMaxLength,
            "Movement reason" );
        string normalizedPerformedBy = NormalizeRequiredText(
            performedBy,
            PerformedByMaxLength,
            "Movement performer" );
        EnsureOperationTimestamp( occurredAtUtc );
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
        Version++;
        _movements.Add( movement );
        _operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.Move,
            destination.ContainerKey,
            Quantity,
            Version,
            occurredAtUtc ) );
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
            Version = 0,
            CreatedAtUtc = createdAtUtc,
        };
    }

    private void EnsureExpectedVersion( int expectedVersion )
    {
        if ( expectedVersion != Version )
        {
            throw new InventoryException(
                $"Item instance version mismatch. Expected {expectedVersion}, current {Version}." );
        }
    }

    private static void EnsureOperationId( Guid operationId )
    {
        if ( operationId == Guid.Empty )
        {
            throw new InventoryException( "Inventory operation id cannot be empty." );
        }
    }

    private InventoryOperation? FindOperation( Guid operationId )
    {
        return _operations.SingleOrDefault(
            operation => operation.OperationId == operationId );
    }

    private void EnsureOperationTimestamp( DateTimeOffset appliedAtUtc )
    {
        if ( appliedAtUtc.Offset != TimeSpan.Zero )
        {
            throw new InventoryException( "Inventory operation timestamp must use UTC." );
        }

        DateTimeOffset earliestTimestamp = _operations.Count == 0
            ? CreatedAtUtc
            : _operations[ _operations.Count - 1 ].AppliedAtUtc;
        if ( appliedAtUtc < earliestTimestamp )
        {
            throw new InventoryException(
                "Inventory operation timestamp cannot precede the item history." );
        }
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
