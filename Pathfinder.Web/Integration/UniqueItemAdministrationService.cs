using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Application.Administration;
using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Infrastructure.Data;

namespace Pathfinder.Web.Integration;

public sealed class UniqueItemAdministrationService
{
    private readonly ItemCatalogDbContext _itemCatalogDbContext;
    private readonly InventoryDbContext _inventoryDbContext;
    private readonly IItemCatalogAdministrativeAccess _administrativeAccess;
    private readonly ItemEffectRestrictionPolicy _effectRestrictionPolicy;
    private readonly TimeProvider _timeProvider;

    public UniqueItemAdministrationService(
        ItemCatalogDbContext itemCatalogDbContext,
        InventoryDbContext inventoryDbContext,
        IItemCatalogAdministrativeAccess administrativeAccess,
        ItemEffectRestrictionPolicy effectRestrictionPolicy,
        TimeProvider timeProvider )
    {
        _itemCatalogDbContext = itemCatalogDbContext;
        _inventoryDbContext = inventoryDbContext;
        _administrativeAccess = administrativeAccess;
        _effectRestrictionPolicy = effectRestrictionPolicy;
        _timeProvider = timeProvider;
    }

    public async Task<UniqueItemDto> CreateAsync(
        CreateUniqueItemRequest request,
        CancellationToken cancellationToken )
    {
        bool canManage = await _administrativeAccess.CanManageCampaignCatalogAsync(
            request.ActingUserId,
            request.CampaignId,
            cancellationToken );
        if ( !canManage )
        {
            throw new ItemCatalogAccessDeniedException(
                "Current user cannot create unique items for this campaign." );
        }

        ItemDefinition definition = await _itemCatalogDbContext.ItemDefinitions
            .Include( item => item.Revisions )
            .SingleOrDefaultAsync(
                item =>
                    item.Id == request.ItemDefinitionId &&
                    (item.Scope == ItemCatalogScope.Global ||
                     (item.Scope == ItemCatalogScope.Campaign &&
                      item.CampaignId == request.CampaignId)),
                cancellationToken )
            ?? throw new ItemCatalogApplicationException(
                "Published item definition was not found in this campaign." );
        ItemRevision revision = definition.Revisions
            .SingleOrDefault( item =>
                item.RevisionNumber == request.RevisionNumber &&
                item.Status == ItemRevisionStatus.Published )
            ?? throw new ItemCatalogApplicationException(
                "Published item revision was not found." );
        InventoryContainer container = await _inventoryDbContext.Containers
            .SingleOrDefaultAsync(
                item =>
                    item.CampaignId == request.CampaignId &&
                    item.ContainerKey == request.ContainerKey,
                cancellationToken )
            ?? throw new InventoryException(
                "Destination inventory container was not found in this campaign." );

        PermanentUpgrade[] upgrades = request.PermanentUpgrades
            .Select( item => PermanentUpgrade.Create(
                item.Code,
                item.Kind,
                item.Rank,
                item.Visibility ) )
            .ToArray();
        ItemConfiguration candidate = ItemConfiguration.Create(
            request.CampaignId,
            revision.Id,
            request.Size,
            request.MaterialType,
            request.MaterialGrade,
            upgrades,
            _timeProvider.GetUtcNow() );
        ItemConfiguration? configuration = await _itemCatalogDbContext.ItemConfigurations
            .Include( item => item.PermanentUpgrades )
            .SingleOrDefaultAsync(
                item => item.ConfigurationKey == candidate.ConfigurationKey,
                cancellationToken );
        if ( configuration is null )
        {
            configuration = candidate;
            _itemCatalogDbContext.ItemConfigurations.Add( configuration );
            await _itemCatalogDbContext.SaveChangesAsync( cancellationToken );
        }

        ItemInstance expected = ItemInstance.Create(
            request.InstanceKey,
            request.CampaignId,
            configuration.Id,
            container,
            request.CustomName,
            _timeProvider.GetUtcNow() );
        if ( _effectRestrictionPolicy.RequiresTransferRestriction(
                 configuration.PermanentUpgrades ) )
        {
            expected.SetTransferRestriction(
                true,
                expected.Version,
                request.InstanceKey,
                _timeProvider.GetUtcNow() );
        }

        ItemInstance? instance = await _inventoryDbContext.ItemInstances
            .SingleOrDefaultAsync(
                item => item.InstanceKey == request.InstanceKey,
                cancellationToken );
        if ( instance is null )
        {
            instance = expected;
            _inventoryDbContext.ItemInstances.Add( instance );
            await _inventoryDbContext.SaveChangesAsync( cancellationToken );
        }
        else if ( instance.CampaignId != expected.CampaignId ||
                  instance.ItemConfigurationId != expected.ItemConfigurationId ||
                  instance.CurrentContainerKey != expected.CurrentContainerKey ||
                  instance.IsTransferRestricted != expected.IsTransferRestricted ||
                  !String.Equals(
                      instance.CustomName,
                      expected.CustomName,
                      StringComparison.Ordinal ) )
        {
            throw new InventoryException(
                "Item instance key was already used for different unique item parameters." );
        }

        return new UniqueItemDto(
            instance.InstanceKey,
            instance.CampaignId,
            configuration.Id,
            configuration.ConfigurationKey,
            instance.CurrentContainerKey,
            instance.CustomName,
            instance.Version );
    }
}

public sealed record CreateUniqueItemRequest(
    int CampaignId,
    int ItemDefinitionId,
    int RevisionNumber,
    ItemSize Size,
    ItemMaterialType MaterialType,
    ItemMaterialGrade MaterialGrade,
    IReadOnlyCollection<PermanentUpgrade> PermanentUpgrades,
    Guid InstanceKey,
    Guid ContainerKey,
    string? CustomName,
    int ActingUserId );

public sealed record UniqueItemDto(
    Guid InstanceKey,
    int CampaignId,
    int ItemConfigurationId,
    string ConfigurationKey,
    Guid ContainerKey,
    string? CustomName,
    int Version );