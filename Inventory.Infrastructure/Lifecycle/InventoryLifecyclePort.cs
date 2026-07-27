using Pathfinder.Inventory.Application.Lifecycle;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;

namespace Pathfinder.Inventory.Infrastructure.Lifecycle;

public sealed class InventoryLifecyclePort : IInventoryLifecyclePort
{
    private readonly IInventoryTransferRepository _repository;
    private readonly TimeProvider _timeProvider;

    public InventoryLifecyclePort(
        IInventoryTransferRepository repository,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<ItemLifecycleDto> ConsumeChargesAsync(
        InventoryLifecycleMutation mutation,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await LoadItem(
            mutation.CampaignId,
            mutation.ItemInstanceKey,
            cancellationToken );
        item.ConsumeCharges(
            mutation.Quantity,
            mutation.ExpectedVersion,
            mutation.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( item, cancellationToken );
    }

    public async Task<ItemLifecycleDto> RecoverChargesAsync(
        InventoryLifecycleMutation mutation,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await LoadItem(
            mutation.CampaignId,
            mutation.ItemInstanceKey,
            cancellationToken );
        item.RecoverCharges(
            mutation.Quantity,
            mutation.ExpectedVersion,
            mutation.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( item, cancellationToken );
    }

    public async Task<ItemLifecycleDto> ConsumeItemAsync(
        InventoryLifecycleMutation mutation,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await LoadItem(
            mutation.CampaignId,
            mutation.ItemInstanceKey,
            cancellationToken );
        item.Consume(
            mutation.Quantity,
            mutation.ExpectedVersion,
            mutation.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( item, cancellationToken );
    }

    public async Task<ItemLifecycleDto> DamageItemAsync(
        InventoryLifecycleMutation mutation,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await LoadItem(
            mutation.CampaignId,
            mutation.ItemInstanceKey,
            cancellationToken );
        item.ApplyDamage(
            mutation.Quantity,
            mutation.ExpectedVersion,
            mutation.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( item, cancellationToken );
    }

    public async Task<ItemLifecycleDto> RepairItemAsync(
        InventoryLifecycleMutation mutation,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await LoadItem(
            mutation.CampaignId,
            mutation.ItemInstanceKey,
            cancellationToken );
        item.Repair(
            mutation.Quantity,
            mutation.ExpectedVersion,
            mutation.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( item, cancellationToken );
    }

    public async Task<ItemLifecycleDto> AttachRuneAsync(
        AttachRuneMutation mutation,
        CancellationToken cancellationToken )
    {
        ItemInstance rune = await LoadItem(
            mutation.CampaignId,
            mutation.RuneInstanceKey,
            cancellationToken );
        ItemInstance target = await LoadItem(
            mutation.CampaignId,
            mutation.TargetInstanceKey,
            cancellationToken );
        rune.AttachRuneTo(
            target,
            mutation.ExpectedRuneVersion,
            mutation.ExpectedTargetVersion,
            mutation.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( rune, cancellationToken );
    }

    public async Task<ItemLifecycleDto> TransferRuneAsync(
        TransferRuneMutation mutation,
        CancellationToken cancellationToken )
    {
        ItemInstance rune = await LoadItem(
            mutation.CampaignId,
            mutation.RuneInstanceKey,
            cancellationToken );
        ItemInstance source = await LoadItem(
            mutation.CampaignId,
            mutation.SourceInstanceKey,
            cancellationToken );
        ItemInstance destination = await LoadItem(
            mutation.CampaignId,
            mutation.DestinationInstanceKey,
            cancellationToken );
        rune.TransferRuneTo(
            source,
            destination,
            mutation.ExpectedRuneVersion,
            mutation.ExpectedSourceVersion,
            mutation.ExpectedDestinationVersion,
            mutation.OperationId,
            _timeProvider.GetUtcNow() );
        return await Save( rune, cancellationToken );
    }

    private async Task<ItemInstance> LoadItem(
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
