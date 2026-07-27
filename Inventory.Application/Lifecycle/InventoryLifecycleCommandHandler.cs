using MediatR;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;

namespace Pathfinder.Inventory.Application.Lifecycle;

public sealed class InventoryLifecycleCommandHandler :
    IRequestHandler<ConsumeItemChargesCommand, ItemLifecycleDto>,
    IRequestHandler<RecoverItemChargesCommand, ItemLifecycleDto>,
    IRequestHandler<ConsumeInventoryItemCommand, ItemLifecycleDto>,
    IRequestHandler<DamageInventoryItemCommand, ItemLifecycleDto>,
    IRequestHandler<RepairInventoryItemCommand, ItemLifecycleDto>,
    IRequestHandler<AttachInventoryRuneCommand, ItemLifecycleDto>,
    IRequestHandler<TransferInventoryRuneCommand, ItemLifecycleDto>
{
    private readonly IInventoryTransferRepository _repository;
    private readonly IInventoryLifecycleAccessPolicy _accessPolicy;
    private readonly TimeProvider _timeProvider;

    public InventoryLifecycleCommandHandler(
        IInventoryTransferRepository repository,
        IInventoryLifecycleAccessPolicy accessPolicy,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _accessPolicy = accessPolicy;
        _timeProvider = timeProvider;
    }

    public async Task<ItemLifecycleDto> Handle(
        ConsumeItemChargesCommand request,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await LoadAuthorizedItem(
            request.ActingUserId,
            request.CampaignId,
            request.ItemInstanceKey,
            cancellationToken );
        item.ConsumeCharges(
            request.ChargeCost,
            request.ExpectedVersion,
            request.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( item, cancellationToken );
    }

    public async Task<ItemLifecycleDto> Handle(
        RecoverItemChargesCommand request,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await LoadAuthorizedItem(
            request.ActingUserId,
            request.CampaignId,
            request.ItemInstanceKey,
            cancellationToken );
        item.RecoverCharges(
            request.Quantity,
            request.ExpectedVersion,
            request.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( item, cancellationToken );
    }

    public async Task<ItemLifecycleDto> Handle(
        ConsumeInventoryItemCommand request,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await LoadAuthorizedItem(
            request.ActingUserId,
            request.CampaignId,
            request.ItemInstanceKey,
            cancellationToken );
        item.Consume(
            request.UseCount,
            request.ExpectedVersion,
            request.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( item, cancellationToken );
    }

    public async Task<ItemLifecycleDto> Handle(
        DamageInventoryItemCommand request,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await LoadAuthorizedItem(
            request.ActingUserId,
            request.CampaignId,
            request.ItemInstanceKey,
            cancellationToken );
        item.ApplyDamage(
            request.Damage,
            request.ExpectedVersion,
            request.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( item, cancellationToken );
    }

    public async Task<ItemLifecycleDto> Handle(
        RepairInventoryItemCommand request,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await LoadAuthorizedItem(
            request.ActingUserId,
            request.CampaignId,
            request.ItemInstanceKey,
            cancellationToken );
        item.Repair(
            request.HitPoints,
            request.ExpectedVersion,
            request.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( item, cancellationToken );
    }

    public async Task<ItemLifecycleDto> Handle(
        AttachInventoryRuneCommand request,
        CancellationToken cancellationToken )
    {
        ItemInstance rune = await LoadAuthorizedItem(
            request.ActingUserId,
            request.CampaignId,
            request.RuneInstanceKey,
            cancellationToken );
        ItemInstance target = await LoadAuthorizedItem(
            request.ActingUserId,
            request.CampaignId,
            request.TargetInstanceKey,
            cancellationToken );
        rune.AttachRuneTo(
            target,
            request.ExpectedRuneVersion,
            request.ExpectedTargetVersion,
            request.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( rune, cancellationToken );
    }

    public async Task<ItemLifecycleDto> Handle(
        TransferInventoryRuneCommand request,
        CancellationToken cancellationToken )
    {
        ItemInstance rune = await LoadAuthorizedItem(
            request.ActingUserId,
            request.CampaignId,
            request.RuneInstanceKey,
            cancellationToken );
        ItemInstance source = await LoadAuthorizedItem(
            request.ActingUserId,
            request.CampaignId,
            request.SourceInstanceKey,
            cancellationToken );
        ItemInstance destination = await LoadAuthorizedItem(
            request.ActingUserId,
            request.CampaignId,
            request.DestinationInstanceKey,
            cancellationToken );
        rune.TransferRuneTo(
            source,
            destination,
            request.ExpectedRuneVersion,
            request.ExpectedSourceVersion,
            request.ExpectedDestinationVersion,
            request.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( rune, cancellationToken );
    }

    private async Task<ItemInstance> LoadAuthorizedItem(
        int actingUserId,
        int campaignId,
        Guid itemInstanceKey,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await _repository.GetItemAsync(
            itemInstanceKey,
            cancellationToken ) ?? throw new InventoryException( "Inventory item was not found." );
        if ( item.CampaignId != campaignId )
        {
            throw new InventoryException( "Inventory item does not belong to the campaign." );
        }

        ItemInstance authorizationItem = item;
        if ( item.AttachedToInstanceKey is not null )
        {
            authorizationItem = await _repository.GetItemAsync(
                item.AttachedToInstanceKey.Value,
                cancellationToken ) ?? throw new InventoryException(
                "Attached rune target was not found." );
            if ( authorizationItem.CampaignId != campaignId )
            {
                throw new InventoryException(
                    "Attached rune target does not belong to the campaign." );
            }
        }

        InventoryContainer container = await _repository.GetContainerByKeyAsync(
            campaignId,
            authorizationItem.CurrentContainerKey,
            cancellationToken ) ?? throw new InventoryException( "Inventory container was not found." );
        bool canMutate = await _accessPolicy.CanMutateAsync(
            campaignId,
            actingUserId,
            container.OwnerKind,
            container.OwnerId,
            cancellationToken );
        if ( !canMutate )
        {
            throw new InventoryException(
                "Actor cannot change lifecycle state for this inventory item." );
        }

        return item;
    }

    private async Task<ItemLifecycleDto> Save(
        ItemInstance item,
        CancellationToken cancellationToken )
    {
        await _repository.SaveChangesAsync( cancellationToken );
        return ItemLifecycleDto.FromDomain( item );
    }
}
