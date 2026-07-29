using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Infrastructure.Data;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;

namespace Pathfinder.Web.Integration;

public sealed class CommerceAdministrationProjectionService
{
    private readonly CampaignManagementDbContext _campaignDbContext;
    private readonly CharacterManagementDbContext _characterDbContext;
    private readonly CommerceDbContext _commerceDbContext;
    private readonly InventoryDbContext _inventoryDbContext;
    private readonly ItemCatalogDbContext _itemCatalogDbContext;
    private readonly InventoryItemCatalogProjectionReader _catalogReader;
    private readonly ICommerceCampaignAccessPolicy _accessPolicy;

    public CommerceAdministrationProjectionService(
        CampaignManagementDbContext campaignDbContext,
        CharacterManagementDbContext characterDbContext,
        CommerceDbContext commerceDbContext,
        InventoryDbContext inventoryDbContext,
        ItemCatalogDbContext itemCatalogDbContext,
        InventoryItemCatalogProjectionReader catalogReader,
        ICommerceCampaignAccessPolicy accessPolicy )
    {
        _campaignDbContext = campaignDbContext;
        _characterDbContext = characterDbContext;
        _commerceDbContext = commerceDbContext;
        _inventoryDbContext = inventoryDbContext;
        _itemCatalogDbContext = itemCatalogDbContext;
        _catalogReader = catalogReader;
        _accessPolicy = accessPolicy;
    }

    public async Task<IReadOnlyCollection<CommerceWalletSummaryDto>> GetWalletsAsync(
        int campaignId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        Campaign campaign = await GetCampaignAsGameMasterAsync(
            campaignId,
            actingUserId,
            cancellationToken );
        int[] characterIds = campaign.Parties
            .SelectMany( party => party.Characters )
            .Select( character => character.CharacterId )
            .Distinct()
            .ToArray();
        Dictionary<int, string> characterNames = await ReadCharacterNamesAsync(
            characterIds,
            cancellationToken );
        Dictionary<int, Wallet> wallets = await _commerceDbContext.Wallets
            .AsNoTracking()
            .Where( wallet =>
                wallet.CampaignId == campaignId &&
                characterIds.Contains( wallet.CharacterId ) )
            .ToDictionaryAsync(
                wallet => wallet.CharacterId,
                cancellationToken );
        return characterIds
            .OrderBy( characterId => characterNames[ characterId ] )
            .ThenBy( characterId => characterId )
            .Select( characterId =>
            {
                wallets.TryGetValue( characterId, out Wallet? wallet );
                return new CommerceWalletSummaryDto(
                    campaignId,
                    characterId,
                    characterNames[ characterId ],
                    wallet?.BalanceCopper ?? 0,
                    wallet?.ReservedCopper ?? 0,
                    wallet?.AvailableCopper ?? 0,
                    wallet?.Version ?? 0 );
            } )
            .ToArray();
    }

    public async Task<WalletDto> GetWalletAsync(
        int campaignId,
        int characterId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        Campaign campaign = await GetCampaignAsGameMasterAsync(
            campaignId,
            actingUserId,
            cancellationToken );
        bool belongsToCampaign = campaign.Parties.Any( party =>
            party.Characters.Any( character =>
                character.CharacterId == characterId ) );
        if ( !belongsToCampaign )
        {
            throw new CommerceAdministrationProjectionNotFoundException();
        }

        Wallet? wallet = await _commerceDbContext.Wallets
            .AsNoTracking()
            .Include( item => item.Entries )
            .SingleOrDefaultAsync(
                item =>
                    item.CampaignId == campaignId &&
                    item.CharacterId == characterId,
                cancellationToken );
        return wallet is null
            ? new WalletDto(
                campaignId,
                characterId,
                0,
                0,
                0,
                0,
                [] )
            : ToDto( wallet );
    }

    public async Task<IReadOnlyCollection<InventoryContainerAdministrationDto>>
        GetContainersAsync(
            int campaignId,
            int actingUserId,
            CancellationToken cancellationToken )
    {
        Campaign campaign = await GetCampaignAsGameMasterAsync(
            campaignId,
            actingUserId,
            cancellationToken );
        InventoryContainer[] containers = await _inventoryDbContext.Containers
            .AsNoTracking()
            .Where( container => container.CampaignId == campaignId )
            .OrderBy( container => container.OwnerKind )
            .ThenBy( container => container.OwnerId )
            .ThenBy( container => container.ContainerKey )
            .ToArrayAsync( cancellationToken );
        Guid[] containerKeys = containers
            .Select( container => container.ContainerKey )
            .ToArray();
        ItemInstance[] instances = await _inventoryDbContext.ItemInstances
            .AsNoTracking()
            .Where( item =>
                item.CampaignId == campaignId &&
                item.Quantity > 0 &&
                containerKeys.Contains( item.CurrentContainerKey ) )
            .OrderBy( item => item.CreatedAtUtc )
            .ThenBy( item => item.InstanceKey )
            .ToArrayAsync( cancellationToken );
        Dictionary<Guid, InventoryOperationItemDto> items = await ReadItemsAsync(
            campaignId,
            instances,
            cancellationToken );
        Dictionary<int, string> characterNames = await ReadCharacterNamesAsync(
            containers
                .Where( container =>
                    container.OwnerKind == InventoryContainerOwnerKind.Character )
                .Select( container => container.OwnerId ),
            cancellationToken );
        Dictionary<int, string> partyNames = campaign.Parties
            .ToDictionary( party => party.Id, party => party.Name );
        Dictionary<int, string> shopNames = await _commerceDbContext.Shops
            .AsNoTracking()
            .Where( shop => shop.CampaignId == campaignId )
            .ToDictionaryAsync( shop => shop.Id, shop => shop.Name, cancellationToken );
        return containers
            .Select( container => new InventoryContainerAdministrationDto(
                container.ContainerKey,
                container.OwnerKind,
                container.OwnerId,
                ResolveOwnerName(
                    container,
                    characterNames,
                    partyNames,
                    shopNames ),
                instances
                    .Where( item =>
                        item.CurrentContainerKey == container.ContainerKey )
                    .Select( item => items[ item.InstanceKey ] )
                    .ToArray() ) )
            .ToArray();
    }

    public async Task<IReadOnlyCollection<PublishedItemRevisionAdministrationDto>>
        SearchPublishedRevisionsAsync(
            int campaignId,
            string? search,
            ItemCatalogScopeFilter scope,
            int actingUserId,
            CancellationToken cancellationToken )
    {
        await GetCampaignAsGameMasterAsync(
            campaignId,
            actingUserId,
            cancellationToken );
        string normalizedSearch = search?.Trim().ToLowerInvariant() ?? String.Empty;
        IQueryable<ItemDefinition> query = _itemCatalogDbContext.ItemDefinitions
            .AsNoTracking()
            .Include( definition => definition.Revisions )
            .Where( definition =>
                ( definition.Scope == ItemCatalogScope.Global ||
                  ( definition.Scope == ItemCatalogScope.Campaign &&
                    definition.CampaignId == campaignId ) ) &&
                definition.Revisions.Any( revision =>
                    revision.Status == ItemRevisionStatus.Published ) );
        query = scope switch
        {
            ItemCatalogScopeFilter.Global => query.Where( definition =>
                definition.Scope == ItemCatalogScope.Global ),
            ItemCatalogScopeFilter.Campaign => query.Where( definition =>
                definition.Scope == ItemCatalogScope.Campaign ),
            ItemCatalogScopeFilter.All => query,
            _ => throw new ArgumentOutOfRangeException( nameof( scope ) ),
        };
        if ( normalizedSearch.Length > 0 )
        {
            query = query.Where( definition =>
                definition.Key.ToLower().Contains( normalizedSearch ) ||
                definition.Revisions.Any( revision =>
                    revision.Status == ItemRevisionStatus.Published &&
                    revision.Name.ToLower().Contains( normalizedSearch ) ) );
        }

        ItemDefinition[] definitions = await query
            .OrderBy( definition => definition.Key )
            .Take( 50 )
            .ToArrayAsync( cancellationToken );
        ItemRevision[] revisions = definitions
            .SelectMany( definition => definition.Revisions )
            .Where( revision => revision.Status == ItemRevisionStatus.Published )
            .ToArray();
        int[] revisionIds = revisions
            .Select( revision => revision.Id )
            .ToArray();
        ItemConfiguration[] configurations = await _itemCatalogDbContext.ItemConfigurations
            .AsNoTracking()
            .Where( configuration =>
                ( configuration.CampaignId == null ||
                  configuration.CampaignId == campaignId ) &&
                revisionIds.Contains( configuration.ItemRevisionId ) )
            .OrderBy( configuration => configuration.Id )
            .ToArrayAsync( cancellationToken );
        return definitions
            .SelectMany( definition => definition.Revisions
                .Where( revision =>
                    revision.Status == ItemRevisionStatus.Published )
                .Select( revision =>
                    new PublishedItemRevisionAdministrationDto(
                        definition.Id,
                        definition.Key,
                        definition.Scope,
                        definition.CampaignId,
                        revision.Id,
                        revision.RevisionNumber,
                        revision.Name,
                        revision.Description,
                        revision.Level,
                        revision.PriceInCopperPieces,
                        revision.Bulk,
                        revision.PrimaryCategory,
                        revision.Rarity,
                        configurations
                            .Where( configuration =>
                                configuration.ItemRevisionId == revision.Id )
                            .Select( configuration =>
                                new ItemConfigurationAdministrationDto(
                                    configuration.Id,
                                    configuration.Size,
                                    configuration.MaterialType,
                                    configuration.MaterialGrade ) )
                            .ToArray() ) ) )
            .OrderBy( revision => revision.Name )
            .ThenBy( revision => revision.Key )
            .ToArray();
    }

    private async Task<Campaign> GetCampaignAsGameMasterAsync(
        int campaignId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        bool isGameMaster = await _accessPolicy.IsGameMasterAsync(
            campaignId,
            actingUserId,
            cancellationToken );
        if ( !isGameMaster )
        {
            throw new UnauthorizedAccessException();
        }

        return await _campaignDbContext.Campaigns
            .AsNoTracking()
            .Include( campaign => campaign.Parties )
                .ThenInclude( party => party.Characters )
            .SingleOrDefaultAsync(
                campaign =>
                    campaign.Id == campaignId &&
                    campaign.Status == CampaignStatus.Active,
                cancellationToken )
            ?? throw new UnauthorizedAccessException();
    }

    private async Task<Dictionary<int, string>> ReadCharacterNamesAsync(
        IEnumerable<int> characterIds,
        CancellationToken cancellationToken )
    {
        int[] ids = characterIds
            .Distinct()
            .ToArray();
        return await _characterDbContext.Character
            .AsNoTracking()
            .Where( character => ids.Contains( character.Id ) )
            .ToDictionaryAsync(
                character => character.Id,
                character => character.Name,
                cancellationToken );
    }

    private async Task<Dictionary<Guid, InventoryOperationItemDto>> ReadItemsAsync(
        int campaignId,
        IReadOnlyCollection<ItemInstance> instances,
        CancellationToken cancellationToken )
    {
        Dictionary<int, InventoryItemCatalogProjection> catalog =
            await _catalogReader.ReadAsync( campaignId, instances, cancellationToken );
        return instances.ToDictionary(
            item => item.InstanceKey,
            item =>
            {
                InventoryItemCatalogProjection revision =
                    catalog[ item.ItemConfigurationId ];
                return new InventoryOperationItemDto(
                    item.InstanceKey,
                    item.Version,
                    item.Quantity,
                    item.CustomName ?? revision.Name,
                    revision.PrimaryCategory,
                    revision.BulkTenths );
            } );
    }

    private static string? ResolveOwnerName(
        InventoryContainer container,
        IReadOnlyDictionary<int, string> characterNames,
        IReadOnlyDictionary<int, string> partyNames,
        IReadOnlyDictionary<int, string> shopNames )
    {
        return container.OwnerKind switch
        {
            InventoryContainerOwnerKind.Character =>
                characterNames.GetValueOrDefault( container.OwnerId ),
            InventoryContainerOwnerKind.Party =>
                partyNames.GetValueOrDefault( container.OwnerId ),
            InventoryContainerOwnerKind.Shop =>
                shopNames.GetValueOrDefault( container.OwnerId ),
            InventoryContainerOwnerKind.World => null,
            _ => null,
        };
    }

    private static WalletDto ToDto( Wallet wallet )
    {
        WalletLedgerEntryDto[] entries = wallet.Entries
            .OrderByDescending( entry => entry.OccurredAtUtc )
            .ThenBy( entry => entry.OperationId )
            .Select( entry => new WalletLedgerEntryDto(
                entry.OperationId,
                entry.Kind,
                entry.AmountCopper,
                entry.BalanceAfterCopper,
                entry.Description,
                entry.PerformedByUserId,
                entry.OccurredAtUtc ) )
            .ToArray();
        return new WalletDto(
            wallet.CampaignId,
            wallet.CharacterId,
            wallet.BalanceCopper,
            wallet.ReservedCopper,
            wallet.AvailableCopper,
            wallet.Version,
            entries );
    }
}

public enum ItemCatalogScopeFilter
{
    All = 0,
    Global = 1,
    Campaign = 2,
}

public sealed record CommerceWalletSummaryDto(
    int CampaignId,
    int CharacterId,
    string CharacterName,
    long BalanceCopper,
    long ReservedCopper,
    long AvailableCopper,
    int Version );

public sealed record InventoryContainerAdministrationDto(
    Guid ContainerKey,
    InventoryContainerOwnerKind OwnerKind,
    int OwnerId,
    string? OwnerName,
    IReadOnlyCollection<InventoryOperationItemDto> Items );

public sealed record PublishedItemRevisionAdministrationDto(
    int ItemDefinitionId,
    string Key,
    ItemCatalogScope Scope,
    int? CampaignId,
    int ItemRevisionId,
    int RevisionNumber,
    string Name,
    string Description,
    int Level,
    int PriceInCopperPieces,
    decimal Bulk,
    ItemCategory PrimaryCategory,
    ItemRarity Rarity,
    IReadOnlyCollection<ItemConfigurationAdministrationDto> Configurations );

public sealed record ItemConfigurationAdministrationDto(
    int ItemConfigurationId,
    ItemSize Size,
    ItemMaterialType MaterialType,
    ItemMaterialGrade MaterialGrade );

public sealed class CommerceAdministrationProjectionNotFoundException : Exception
{
    public CommerceAdministrationProjectionNotFoundException()
        : base( "The requested commerce administration projection was not found." )
    {
    }
}
