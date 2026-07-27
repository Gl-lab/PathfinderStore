namespace Pathfinder.Inventory.Application.Lifecycle;

public interface IInventoryLifecyclePort
{
    Task<ItemLifecycleDto> ConsumeChargesAsync(
        InventoryLifecycleMutation mutation,
        CancellationToken cancellationToken );
    Task<ItemLifecycleDto> RecoverChargesAsync(
        InventoryLifecycleMutation mutation,
        CancellationToken cancellationToken );
    Task<ItemLifecycleDto> ConsumeItemAsync(
        InventoryLifecycleMutation mutation,
        CancellationToken cancellationToken );
    Task<ItemLifecycleDto> DamageItemAsync(
        InventoryLifecycleMutation mutation,
        CancellationToken cancellationToken );
    Task<ItemLifecycleDto> RepairItemAsync(
        InventoryLifecycleMutation mutation,
        CancellationToken cancellationToken );
    Task<ItemLifecycleDto> AttachRuneAsync(
        AttachRuneMutation mutation,
        CancellationToken cancellationToken );
    Task<ItemLifecycleDto> TransferRuneAsync(
        TransferRuneMutation mutation,
        CancellationToken cancellationToken );
}

public sealed record InventoryLifecycleMutation(
    int CampaignId,
    Guid ItemInstanceKey,
    int Quantity,
    int ExpectedVersion,
    Guid OperationId );

public sealed record AttachRuneMutation(
    int CampaignId,
    Guid RuneInstanceKey,
    Guid TargetInstanceKey,
    int ExpectedRuneVersion,
    int ExpectedTargetVersion,
    Guid OperationId );

public sealed record TransferRuneMutation(
    int CampaignId,
    Guid RuneInstanceKey,
    Guid SourceInstanceKey,
    Guid DestinationInstanceKey,
    int ExpectedRuneVersion,
    int ExpectedSourceVersion,
    int ExpectedDestinationVersion,
    Guid OperationId );
