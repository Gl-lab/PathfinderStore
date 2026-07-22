namespace Pathfinder.Inventory.Application.Storage;

public sealed record PartyStorageItemDto(
    Guid ItemInstanceKey,
    Guid ContainerKey,
    int Version );
