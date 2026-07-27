using MediatR;

namespace Pathfinder.Inventory.Application.Lifecycle;

public sealed record ConsumeItemChargesCommand(
    int ActingUserId,
    int CampaignId,
    Guid ItemInstanceKey,
    int ChargeCost,
    int ExpectedVersion,
    Guid OperationId ) : IRequest<ItemLifecycleDto>;

public sealed record RecoverItemChargesCommand(
    int ActingUserId,
    int CampaignId,
    Guid ItemInstanceKey,
    int Quantity,
    int ExpectedVersion,
    Guid OperationId ) : IRequest<ItemLifecycleDto>;

public sealed record ConsumeInventoryItemCommand(
    int ActingUserId,
    int CampaignId,
    Guid ItemInstanceKey,
    int UseCount,
    int ExpectedVersion,
    Guid OperationId ) : IRequest<ItemLifecycleDto>;

public sealed record DamageInventoryItemCommand(
    int ActingUserId,
    int CampaignId,
    Guid ItemInstanceKey,
    int Damage,
    int ExpectedVersion,
    Guid OperationId ) : IRequest<ItemLifecycleDto>;

public sealed record RepairInventoryItemCommand(
    int ActingUserId,
    int CampaignId,
    Guid ItemInstanceKey,
    int HitPoints,
    int ExpectedVersion,
    Guid OperationId ) : IRequest<ItemLifecycleDto>;

public sealed record AttachInventoryRuneCommand(
    int ActingUserId,
    int CampaignId,
    Guid RuneInstanceKey,
    Guid TargetInstanceKey,
    int ExpectedRuneVersion,
    int ExpectedTargetVersion,
    Guid OperationId ) : IRequest<ItemLifecycleDto>;

public sealed record TransferInventoryRuneCommand(
    int ActingUserId,
    int CampaignId,
    Guid RuneInstanceKey,
    Guid SourceInstanceKey,
    Guid DestinationInstanceKey,
    int ExpectedRuneVersion,
    int ExpectedSourceVersion,
    int ExpectedDestinationVersion,
    Guid OperationId ) : IRequest<ItemLifecycleDto>;
