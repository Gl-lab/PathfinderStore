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
    public Guid? ReservationKey { get; private set; }
    public bool IsTransferRestricted { get; private set; }
    public int? MaximumCharges { get; private set; }
    public int? CurrentCharges { get; private set; }
    public int? DefaultActivationCost { get; private set; }
    public ItemChargeRecoveryRule? ChargeRecoveryRule { get; private set; }
    public ItemConsumptionMode? ConsumptionMode { get; private set; }
    public int? ConsumptionQuantity { get; private set; }
    public int? Hardness { get; private set; }
    public int? MaximumHitPoints { get; private set; }
    public int? CurrentHitPoints { get; private set; }
    public int? BrokenThreshold { get; private set; }
    public ItemRuneTargetKind? RuneTargetKind { get; private set; }
    public string? AttachableRuneCode { get; private set; }
    public Guid? AttachedToInstanceKey { get; private set; }
    public bool IsBroken =>
        CurrentHitPoints > 0 && CurrentHitPoints <= BrokenThreshold;
    public bool IsDestroyed =>
        CurrentHitPoints is not null && CurrentHitPoints == 0;
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

    public static ItemInstance CreateConsumable(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        ItemConsumptionMode consumptionMode,
        int consumptionQuantity,
        InventoryContainer initialContainer,
        string? customName,
        DateTimeOffset createdAtUtc )
    {
        EnsureConsumptionProfile( consumptionMode, consumptionQuantity, false );
        ItemInstance instance = CreateCore(
            instanceKey,
            campaignId,
            itemConfigurationId,
            initialContainer,
            customName,
            false,
            1,
            createdAtUtc );
        instance.ConfigureConsumption( consumptionMode, consumptionQuantity );
        return instance;
    }

    public static ItemInstance CreateConsumableStack(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        int initialQuantity,
        ItemConsumptionMode consumptionMode,
        int consumptionQuantity,
        InventoryContainer initialContainer,
        string? customName,
        DateTimeOffset createdAtUtc )
    {
        EnsureConsumptionProfile( consumptionMode, consumptionQuantity, true );
        if ( initialQuantity <= 0 )
        {
            throw new InventoryException( "Item stack quantity must be greater than zero." );
        }

        ItemInstance instance = CreateCore(
            instanceKey,
            campaignId,
            itemConfigurationId,
            initialContainer,
            customName,
            true,
            initialQuantity,
            createdAtUtc );
        instance.ConfigureConsumption( consumptionMode, consumptionQuantity );
        return instance;
    }

    public static ItemInstance CreateCharged(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        int maximumCharges,
        int defaultActivationCost,
        ItemChargeRecoveryRule recoveryRule,
        InventoryContainer initialContainer,
        string? customName,
        DateTimeOffset createdAtUtc )
    {
        EnsureChargeProfile( maximumCharges, defaultActivationCost, recoveryRule );
        ItemInstance instance = CreateCore(
            instanceKey,
            campaignId,
            itemConfigurationId,
            initialContainer,
            customName,
            false,
            1,
            createdAtUtc );
        instance.MaximumCharges = maximumCharges;
        instance.CurrentCharges = maximumCharges;
        instance.DefaultActivationCost = defaultActivationCost;
        instance.ChargeRecoveryRule = recoveryRule;
        return instance;
    }

    public static ItemInstance CreateDurable(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        int hardness,
        int maximumHitPoints,
        int brokenThreshold,
        InventoryContainer initialContainer,
        string? customName,
        DateTimeOffset createdAtUtc )
    {
        EnsureDurabilityProfile( hardness, maximumHitPoints, brokenThreshold );
        ItemInstance instance = CreateCore(
            instanceKey,
            campaignId,
            itemConfigurationId,
            initialContainer,
            customName,
            false,
            1,
            createdAtUtc );
        instance.Hardness = hardness;
        instance.MaximumHitPoints = maximumHitPoints;
        instance.CurrentHitPoints = maximumHitPoints;
        instance.BrokenThreshold = brokenThreshold;
        return instance;
    }

    public static ItemInstance CreateRuneCompatible(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        ItemRuneTargetKind runeTargetKind,
        InventoryContainer initialContainer,
        string? customName,
        DateTimeOffset createdAtUtc )
    {
        EnsureRuneTargetKind( runeTargetKind );
        ItemInstance instance = CreateCore(
            instanceKey,
            campaignId,
            itemConfigurationId,
            initialContainer,
            customName,
            false,
            1,
            createdAtUtc );
        instance.RuneTargetKind = runeTargetKind;
        return instance;
    }

    public static ItemInstance CreateAttachableRune(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        string runeCode,
        ItemRuneTargetKind runeTargetKind,
        InventoryContainer initialContainer,
        string? customName,
        DateTimeOffset createdAtUtc )
    {
        EnsureRuneTargetKind( runeTargetKind );
        string normalizedRuneCode = NormalizeRequiredText(
            runeCode,
            CustomNameMaxLength,
            "Attachable rune code" );
        ItemInstance instance = CreateCore(
            instanceKey,
            campaignId,
            itemConfigurationId,
            initialContainer,
            customName,
            false,
            1,
            createdAtUtc );
        instance.AttachableRuneCode = normalizedRuneCode;
        instance.RuneTargetKind = runeTargetKind;
        return instance;
    }

    public bool ConsumeCharges(
        int chargeCost,
        int expectedVersion,
        Guid operationId,
        DateTimeOffset consumedAtUtc )
    {
        EnsureOperationId( operationId );
        InventoryOperation? replay = FindOperation( operationId );
        if ( replay is not null )
        {
            replay.EnsureMatches(
                InventoryOperationKind.ConsumeCharges,
                InstanceKey,
                chargeCost );
            return false;
        }

        EnsureExpectedVersion( expectedVersion );
        EnsureOperationTimestamp( consumedAtUtc );
        EnsureNotReserved();
        EnsureCharged();
        if ( chargeCost <= 0 )
        {
            throw new InventoryException( "Charge cost must be greater than zero." );
        }

        if ( chargeCost > CurrentCharges!.Value )
        {
            throw new InventoryException( "Item instance does not have enough charges." );
        }

        CurrentCharges -= chargeCost;
        Version++;
        _operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.ConsumeCharges,
            InstanceKey,
            chargeCost,
            Version,
            consumedAtUtc ) );
        return true;
    }

    public bool ConsumeDefaultCharges(
        int expectedVersion,
        Guid operationId,
        DateTimeOffset consumedAtUtc )
    {
        EnsureCharged();
        return ConsumeCharges(
            DefaultActivationCost!.Value,
            expectedVersion,
            operationId,
            consumedAtUtc );
    }

    public bool RecoverCharges(
        int quantity,
        int expectedVersion,
        Guid operationId,
        DateTimeOffset recoveredAtUtc )
    {
        EnsureOperationId( operationId );
        InventoryOperation? replay = FindOperation( operationId );
        if ( replay is not null )
        {
            replay.EnsureMatches(
                InventoryOperationKind.RecoverCharges,
                InstanceKey,
                quantity );
            return false;
        }

        EnsureExpectedVersion( expectedVersion );
        EnsureOperationTimestamp( recoveredAtUtc );
        EnsureNotReserved();
        EnsureCharged();
        if ( ChargeRecoveryRule == ItemChargeRecoveryRule.None )
        {
            throw new InventoryException( "Item instance charges cannot be recovered." );
        }

        if ( quantity <= 0 )
        {
            throw new InventoryException( "Recovered charge quantity must be greater than zero." );
        }

        if ( quantity > MaximumCharges!.Value - CurrentCharges!.Value )
        {
            throw new InventoryException(
                "Recovered charges cannot exceed the item instance maximum." );
        }

        CurrentCharges += quantity;
        Version++;
        _operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.RecoverCharges,
            InstanceKey,
            quantity,
            Version,
            recoveredAtUtc ) );
        return true;
    }

    public bool Consume(
        int useCount,
        int expectedVersion,
        Guid operationId,
        DateTimeOffset consumedAtUtc )
    {
        EnsureOperationId( operationId );
        EnsureConsumable();
        if ( useCount <= 0 )
        {
            throw new InventoryException( "Consumption use count must be greater than zero." );
        }

        if ( useCount > Int32.MaxValue / ConsumptionQuantity!.Value )
        {
            throw new InventoryException( "Consumed item quantity is too large." );
        }

        int consumedQuantity = useCount * ConsumptionQuantity.Value;
        InventoryOperation? replay = FindOperation( operationId );
        if ( replay is not null )
        {
            replay.EnsureMatches(
                InventoryOperationKind.ConsumeItem,
                InstanceKey,
                consumedQuantity );
            return false;
        }

        EnsureExpectedVersion( expectedVersion );
        EnsureOperationTimestamp( consumedAtUtc );
        EnsureNotReserved();
        if ( IsDepleted )
        {
            throw new InventoryException( "A depleted item instance cannot be consumed." );
        }

        if ( consumedQuantity > Quantity )
        {
            throw new InventoryException(
                "Consumption cannot exceed the item instance quantity." );
        }

        Quantity -= consumedQuantity;
        Version++;
        _operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.ConsumeItem,
            InstanceKey,
            consumedQuantity,
            Version,
            consumedAtUtc ) );
        return true;
    }

    public bool ApplyDamage(
        int damage,
        int expectedVersion,
        Guid operationId,
        DateTimeOffset damagedAtUtc )
    {
        EnsureOperationId( operationId );
        InventoryOperation? replay = FindOperation( operationId );
        if ( replay is not null )
        {
            replay.EnsureMatches(
                InventoryOperationKind.DamageItem,
                InstanceKey,
                damage );
            return false;
        }

        EnsureExpectedVersion( expectedVersion );
        EnsureOperationTimestamp( damagedAtUtc );
        EnsureNotReserved();
        EnsureDurable();
        if ( damage <= 0 )
        {
            throw new InventoryException( "Item damage must be greater than zero." );
        }

        if ( IsDestroyed )
        {
            throw new InventoryException( "A destroyed item instance cannot take damage." );
        }

        int appliedDamage = Math.Max( 0, damage - Hardness!.Value );
        CurrentHitPoints = Math.Max( 0, CurrentHitPoints!.Value - appliedDamage );
        if ( CurrentHitPoints == 0 )
        {
            Quantity = 0;
        }

        Version++;
        _operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.DamageItem,
            InstanceKey,
            damage,
            Version,
            damagedAtUtc ) );
        return true;
    }

    public bool Repair(
        int hitPoints,
        int expectedVersion,
        Guid operationId,
        DateTimeOffset repairedAtUtc )
    {
        EnsureOperationId( operationId );
        InventoryOperation? replay = FindOperation( operationId );
        if ( replay is not null )
        {
            replay.EnsureMatches(
                InventoryOperationKind.RepairItem,
                InstanceKey,
                hitPoints );
            return false;
        }

        EnsureExpectedVersion( expectedVersion );
        EnsureOperationTimestamp( repairedAtUtc );
        EnsureNotReserved();
        EnsureDurable();
        if ( hitPoints <= 0 )
        {
            throw new InventoryException( "Repaired Hit Points must be greater than zero." );
        }

        if ( IsDestroyed )
        {
            throw new InventoryException( "A destroyed item instance cannot be repaired." );
        }

        if ( hitPoints > MaximumHitPoints!.Value - CurrentHitPoints!.Value )
        {
            throw new InventoryException(
                "Repair cannot exceed the item instance maximum Hit Points." );
        }

        CurrentHitPoints += hitPoints;
        Version++;
        _operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.RepairItem,
            InstanceKey,
            hitPoints,
            Version,
            repairedAtUtc ) );
        return true;
    }

    public bool AttachRuneTo(
        ItemInstance target,
        int expectedVersion,
        int targetExpectedVersion,
        Guid operationId,
        DateTimeOffset attachedAtUtc )
    {
        ArgumentNullException.ThrowIfNull( target );
        EnsureOperationId( operationId );
        InventoryOperation? replay = FindOperation( operationId );
        InventoryOperation? targetReplay = target.FindOperation( operationId );
        if ( replay is not null || targetReplay is not null )
        {
            EnsurePairedRuneReplay(
                replay,
                targetReplay,
                InventoryOperationKind.AttachRune,
                target );
            return false;
        }

        EnsureExpectedVersion( expectedVersion );
        target.EnsureExpectedVersion( targetExpectedVersion );
        EnsureOperationTimestamp( attachedAtUtc );
        target.EnsureOperationTimestamp( attachedAtUtc );
        EnsureRuneCanAttachTo( target );
        ApplyRuneAttachment(
            target,
            InventoryOperationKind.AttachRune,
            operationId,
            attachedAtUtc );
        return true;
    }

    public bool TransferRuneTo(
        ItemInstance sourceTarget,
        ItemInstance destinationTarget,
        int expectedVersion,
        int sourceExpectedVersion,
        int destinationExpectedVersion,
        Guid operationId,
        DateTimeOffset transferredAtUtc )
    {
        ArgumentNullException.ThrowIfNull( sourceTarget );
        ArgumentNullException.ThrowIfNull( destinationTarget );
        EnsureOperationId( operationId );
        InventoryOperation? replay = FindOperation( operationId );
        InventoryOperation? sourceReplay = sourceTarget.FindOperation( operationId );
        InventoryOperation? destinationReplay = destinationTarget.FindOperation( operationId );
        if ( replay is not null || sourceReplay is not null || destinationReplay is not null )
        {
            if ( replay is null || sourceReplay is null || destinationReplay is null )
            {
                throw new InventoryException(
                    "Rune transfer operation history is inconsistent." );
            }

            replay.EnsureMatches(
                InventoryOperationKind.TransferRune,
                destinationTarget.InstanceKey,
                1 );
            sourceReplay.EnsureMatches(
                InventoryOperationKind.TransferRune,
                InstanceKey,
                1 );
            destinationReplay.EnsureMatches(
                InventoryOperationKind.TransferRune,
                InstanceKey,
                1 );
            return false;
        }

        EnsureExpectedVersion( expectedVersion );
        sourceTarget.EnsureExpectedVersion( sourceExpectedVersion );
        destinationTarget.EnsureExpectedVersion( destinationExpectedVersion );
        EnsureOperationTimestamp( transferredAtUtc );
        sourceTarget.EnsureOperationTimestamp( transferredAtUtc );
        destinationTarget.EnsureOperationTimestamp( transferredAtUtc );
        if ( AttachedToInstanceKey != sourceTarget.InstanceKey )
        {
            throw new InventoryException( "Rune is not attached to the source item instance." );
        }

        EnsureRuneCanAttachTo( destinationTarget, true );
        AttachedToInstanceKey = destinationTarget.InstanceKey;
        Version++;
        sourceTarget.Version++;
        destinationTarget.Version++;
        _operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.TransferRune,
            destinationTarget.InstanceKey,
            1,
            Version,
            transferredAtUtc ) );
        sourceTarget._operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.TransferRune,
            InstanceKey,
            1,
            sourceTarget.Version,
            transferredAtUtc ) );
        destinationTarget._operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.TransferRune,
            InstanceKey,
            1,
            destinationTarget.Version,
            transferredAtUtc ) );
        return true;
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
        EnsureNotReserved();
        if ( splitQuantity <= 0 || splitQuantity >= Quantity )
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
        if ( ConsumptionMode is not null && ConsumptionQuantity is not null )
        {
            split.ConfigureConsumption(
                ConsumptionMode.Value,
                ConsumptionQuantity.Value );
        }

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
        if ( replay is not null || sourceReplay is not null )
        {
            if ( replay is null || sourceReplay is null )
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
        EnsureNotReserved();
        source.EnsureNotReserved();

        if ( InstanceKey == source.InstanceKey )
        {
            throw new InventoryException( "An item stack cannot be merged with itself." );
        }

        if ( CampaignId != source.CampaignId ||
             ItemConfigurationId != source.ItemConfigurationId ||
             CurrentContainerKey != source.CurrentContainerKey ||
             !String.Equals( CustomName, source.CustomName, StringComparison.Ordinal ) ||
             ConsumptionMode != source.ConsumptionMode ||
             ConsumptionQuantity != source.ConsumptionQuantity )
        {
            throw new InventoryException(
                "Item stacks must have the same campaign, configuration, location, and custom name." );
        }

        if ( Quantity > Int32.MaxValue - source.Quantity )
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
            if ( replay.ToContainerKey != destination.ContainerKey ||
                 !String.Equals( replay.Reason, replayReason, StringComparison.Ordinal ) ||
                 !String.Equals( replay.PerformedBy, replayPerformedBy, StringComparison.Ordinal ) ||
                 replay.OccurredAtUtc != occurredAtUtc )
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

        EnsureNotReserved();
        EnsureNotAttached();
        EnsureTransferAllowed();

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

    public bool Reserve(
        Guid reservationKey,
        int expectedVersion,
        Guid operationId,
        DateTimeOffset reservedAtUtc )
    {
        if ( reservationKey == Guid.Empty )
        {
            throw new InventoryException( "Reservation key cannot be empty." );
        }

        EnsureOperationId( operationId );
        InventoryOperation? replay = FindOperation( operationId );
        if ( replay is not null )
        {
            replay.EnsureMatches( InventoryOperationKind.Reserve, reservationKey, Quantity );
            return false;
        }

        EnsureExpectedVersion( expectedVersion );
        EnsureOperationTimestamp( reservedAtUtc );
        if ( IsDepleted )
        {
            throw new InventoryException( "A depleted item cannot be reserved." );
        }

        EnsureTransferAllowed();
        EnsureNotAttached();

        if ( ReservationKey is not null )
        {
            throw new InventoryException( "Item is already reserved." );
        }

        ReservationKey = reservationKey;
        Version++;
        _operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.Reserve,
            reservationKey,
            Quantity,
            Version,
            reservedAtUtc ) );
        return true;
    }

    public InventoryMovement MoveReservedTo(
        InventoryContainer destination,
        Guid reservationKey,
        int expectedVersion,
        Guid operationId,
        string performedBy,
        DateTimeOffset occurredAtUtc )
    {
        ArgumentNullException.ThrowIfNull( destination );
        EnsureOperationId( operationId );
        if ( FindOperation( operationId ) is not null )
        {
            throw new InventoryException(
                "Operation id was already used for another inventory change." );
        }

        EnsureExpectedVersion( expectedVersion );
        if ( ReservationKey != reservationKey )
        {
            throw new InventoryException( "Item is not reserved for this transfer." );
        }

        EnsureTransferAllowed();

        if ( IsDepleted || destination.CampaignId != CampaignId ||
             destination.ContainerKey == CurrentContainerKey )
        {
            throw new InventoryException( "Reserved item cannot move to the requested container." );
        }

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
            "party-exchange",
            operationId,
            normalizedPerformedBy,
            occurredAtUtc );
        CurrentContainerKey = destination.ContainerKey;
        ReservationKey = null;
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

    public bool ReleaseReservation(
        Guid reservationKey,
        int expectedVersion,
        Guid operationId,
        DateTimeOffset releasedAtUtc )
    {
        EnsureOperationId( operationId );
        InventoryOperation? replay = FindOperation( operationId );
        if ( replay is not null )
        {
            replay.EnsureMatches(
                InventoryOperationKind.ReleaseReservation,
                reservationKey,
                Quantity );
            return false;
        }

        EnsureExpectedVersion( expectedVersion );
        EnsureOperationTimestamp( releasedAtUtc );
        if ( ReservationKey != reservationKey )
        {
            throw new InventoryException( "Item is not reserved for this exchange." );
        }

        ReservationKey = null;
        Version++;
        _operations.Add( InventoryOperation.Create(
            operationId,
            InventoryOperationKind.ReleaseReservation,
            reservationKey,
            Quantity,
            Version,
            releasedAtUtc ) );
        return true;
    }

    public InventoryMovement ForceMoveTo(
        InventoryContainer destination,
        string reason,
        int expectedVersion,
        Guid operationId,
        string performedBy,
        DateTimeOffset occurredAtUtc )
    {
        ArgumentNullException.ThrowIfNull( destination );
        EnsureOperationId( operationId );
        if ( FindOperation( operationId ) is not null )
        {
            throw new InventoryException(
                "Operation id was already used for another inventory change." );
        }

        EnsureExpectedVersion( expectedVersion );
        EnsureNotAttached();
        if ( IsDepleted || destination.CampaignId != CampaignId ||
             destination.ContainerKey == CurrentContainerKey )
        {
            throw new InventoryException( "Item cannot be force-moved to the requested container." );
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
        ReservationKey = null;
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

    public bool SetTransferRestriction(
        bool isRestricted,
        int expectedVersion,
        Guid operationId,
        DateTimeOffset appliedAtUtc )
    {
        EnsureOperationId( operationId );
        InventoryOperationKind kind = isRestricted
            ? InventoryOperationKind.RestrictTransfer
            : InventoryOperationKind.AllowTransfer;
        InventoryOperation? replay = FindOperation( operationId );
        if ( replay is not null )
        {
            replay.EnsureMatches( kind, InstanceKey, Quantity );
            return false;
        }

        EnsureExpectedVersion( expectedVersion );
        EnsureOperationTimestamp( appliedAtUtc );
        if ( IsTransferRestricted == isRestricted )
        {
            throw new InventoryException( "Item transfer restriction already has the requested state." );
        }

        IsTransferRestricted = isRestricted;
        Version++;
        _operations.Add( InventoryOperation.Create(
            operationId,
            kind,
            InstanceKey,
            Quantity,
            Version,
            appliedAtUtc ) );
        return true;
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

    private static void EnsureChargeProfile(
        int maximumCharges,
        int defaultActivationCost,
        ItemChargeRecoveryRule recoveryRule )
    {
        if ( maximumCharges <= 0 )
        {
            throw new InventoryException( "Maximum charges must be greater than zero." );
        }

        if ( defaultActivationCost <= 0 || defaultActivationCost > maximumCharges )
        {
            throw new InventoryException(
                "Default activation cost must be positive and cannot exceed maximum charges." );
        }

        if ( !Enum.IsDefined( recoveryRule ) )
        {
            throw new InventoryException( "Charge recovery rule is invalid." );
        }
    }

    private static void EnsureConsumptionProfile(
        ItemConsumptionMode consumptionMode,
        int consumptionQuantity,
        bool isStackable )
    {
        if ( !Enum.IsDefined( consumptionMode ) )
        {
            throw new InventoryException( "Item consumption mode is invalid." );
        }

        if ( consumptionQuantity <= 0 )
        {
            throw new InventoryException(
                "Item consumption quantity must be greater than zero." );
        }

        if ( isStackable && consumptionMode == ItemConsumptionMode.DestroyInstance )
        {
            throw new InventoryException(
                "A stackable item cannot use destroy-instance consumption." );
        }

        if ( !isStackable &&
             (consumptionMode != ItemConsumptionMode.DestroyInstance ||
              consumptionQuantity != 1) )
        {
            throw new InventoryException(
                "A non-stackable consumable must destroy exactly one instance." );
        }
    }

    private static void EnsureDurabilityProfile(
        int hardness,
        int maximumHitPoints,
        int brokenThreshold )
    {
        if ( hardness < 0 )
        {
            throw new InventoryException( "Item Hardness cannot be negative." );
        }

        if ( maximumHitPoints <= 0 )
        {
            throw new InventoryException(
                "Maximum item Hit Points must be greater than zero." );
        }

        if ( brokenThreshold <= 0 || brokenThreshold > maximumHitPoints )
        {
            throw new InventoryException(
                "Broken threshold must be positive and cannot exceed maximum Hit Points." );
        }
    }

    private static void EnsureRuneTargetKind( ItemRuneTargetKind runeTargetKind )
    {
        if ( !Enum.IsDefined( runeTargetKind ) )
        {
            throw new InventoryException( "Rune target kind is invalid." );
        }
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

    private void EnsureNotReserved()
    {
        if ( ReservationKey is not null )
        {
            throw new InventoryException( "A reserved item instance cannot be changed." );
        }
    }

    private void EnsureTransferAllowed()
    {
        if ( IsTransferRestricted )
        {
            throw new InventoryException( "Item instance is prohibited from transfer." );
        }
    }

    private void EnsureNotAttached()
    {
        if ( AttachedToInstanceKey is not null )
        {
            throw new InventoryException(
                "An attached rune cannot be moved or reserved independently." );
        }
    }

    private void EnsureCharged()
    {
        if ( MaximumCharges is null ||
             CurrentCharges is null ||
             DefaultActivationCost is null ||
             ChargeRecoveryRule is null )
        {
            throw new InventoryException( "Item instance does not have charges." );
        }
    }

    private void ConfigureConsumption(
        ItemConsumptionMode consumptionMode,
        int consumptionQuantity )
    {
        ConsumptionMode = consumptionMode;
        ConsumptionQuantity = consumptionQuantity;
    }

    private void EnsureConsumable()
    {
        if ( ConsumptionMode is null || ConsumptionQuantity is null )
        {
            throw new InventoryException( "Item instance is not consumable." );
        }
    }

    private void EnsureDurable()
    {
        if ( Hardness is null ||
             MaximumHitPoints is null ||
             CurrentHitPoints is null ||
             BrokenThreshold is null )
        {
            throw new InventoryException( "Item instance does not have durability." );
        }
    }

    private void EnsureRuneCanAttachTo(
        ItemInstance target,
        bool allowExistingAttachment = false )
    {
        if ( AttachableRuneCode is null || RuneTargetKind is null )
        {
            throw new InventoryException( "Item instance is not an attachable rune." );
        }

        if ( !allowExistingAttachment && AttachedToInstanceKey is not null )
        {
            throw new InventoryException( "Rune is already attached to an item instance." );
        }

        EnsureNotReserved();
        target.EnsureNotReserved();
        if ( IsDepleted || target.IsDepleted ||
             CampaignId != target.CampaignId ||
             target.InstanceKey == InstanceKey )
        {
            throw new InventoryException( "Rune cannot attach to the requested item instance." );
        }

        if ( target.AttachableRuneCode is not null ||
             target.RuneTargetKind != RuneTargetKind )
        {
            throw new InventoryException( "Rune is incompatible with the target item instance." );
        }
    }

    private void ApplyRuneAttachment(
        ItemInstance target,
        InventoryOperationKind kind,
        Guid operationId,
        DateTimeOffset appliedAtUtc )
    {
        AttachedToInstanceKey = target.InstanceKey;
        Version++;
        target.Version++;
        _operations.Add( InventoryOperation.Create(
            operationId,
            kind,
            target.InstanceKey,
            1,
            Version,
            appliedAtUtc ) );
        target._operations.Add( InventoryOperation.Create(
            operationId,
            kind,
            InstanceKey,
            1,
            target.Version,
            appliedAtUtc ) );
    }

    private void EnsurePairedRuneReplay(
        InventoryOperation? runeReplay,
        InventoryOperation? targetReplay,
        InventoryOperationKind kind,
        ItemInstance target )
    {
        if ( runeReplay is null || targetReplay is null )
        {
            throw new InventoryException( "Rune operation history is inconsistent." );
        }

        runeReplay.EnsureMatches( kind, target.InstanceKey, 1 );
        targetReplay.EnsureMatches( kind, InstanceKey, 1 );
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
