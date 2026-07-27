using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Application.Access;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Inventory.Application.Storage;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Audit;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Movements;
using Pathfinder.Inventory.Domain.Transfers;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Rules;

namespace Pathfinder.Web.Integration;

public sealed class InventoryOperationsProjectionService
{
    private readonly CampaignManagementDbContext _campaignDbContext;
    private readonly CharacterManagementDbContext _characterDbContext;
    private readonly InventoryDbContext _inventoryDbContext;
    private readonly InventoryItemCatalogProjectionReader _catalogReader;
    private readonly ICharacterCampaignAccessPolicy _characterAccessPolicy;

    public InventoryOperationsProjectionService(
        CampaignManagementDbContext campaignDbContext,
        CharacterManagementDbContext characterDbContext,
        InventoryDbContext inventoryDbContext,
        InventoryItemCatalogProjectionReader catalogReader,
        ICharacterCampaignAccessPolicy characterAccessPolicy )
    {
        _campaignDbContext = campaignDbContext;
        _characterDbContext = characterDbContext;
        _inventoryDbContext = inventoryDbContext;
        _catalogReader = catalogReader;
        _characterAccessPolicy = characterAccessPolicy;
    }

    public async Task<IReadOnlyCollection<PartyGiftProjectionDto>> GetGiftsAsync(
        int campaignId,
        int characterId,
        PartyGiftRole role,
        PartyGiftStatus status,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        await EnsureCharacterAccessAsync(
            campaignId,
            characterId,
            actingUserId,
            cancellationToken );
        IQueryable<PartyGift> query = _inventoryDbContext.PartyGifts
            .AsNoTracking()
            .Where( gift =>
                gift.CampaignId == campaignId &&
                gift.Status == status );
        query = role switch
        {
            PartyGiftRole.Incoming => query.Where( gift =>
                gift.DestinationCharacterId == characterId ),
            PartyGiftRole.Outgoing => query.Where( gift =>
                gift.SourceCharacterId == characterId ),
            _ => throw new ArgumentOutOfRangeException( nameof( role ) ),
        };
        PartyGift[] gifts = await query
            .OrderByDescending( gift => gift.CreatedAtUtc )
            .ThenBy( gift => gift.GiftKey )
            .ToArrayAsync( cancellationToken );
        Dictionary<Guid, InventoryOperationItemDto> items = await ReadItemsAsync(
            campaignId,
            gifts.Select( gift => gift.ItemInstanceKey ),
            cancellationToken );
        Dictionary<int, string> characterNames = await ReadCharacterNamesAsync(
            gifts.SelectMany( gift => new[]
            {
                gift.SourceCharacterId,
                gift.DestinationCharacterId,
            } ),
            cancellationToken );
        return gifts
            .Select( gift => new PartyGiftProjectionDto(
                ToDto( gift ),
                items[ gift.ItemInstanceKey ],
                new InventoryCharacterReferenceDto(
                    gift.SourceCharacterId,
                    characterNames[ gift.SourceCharacterId ] ),
                new InventoryCharacterReferenceDto(
                    gift.DestinationCharacterId,
                    characterNames[ gift.DestinationCharacterId ] ) ) )
            .ToArray();
    }

    public async Task<IReadOnlyCollection<PartyExchangeProjectionDto>> GetExchangesAsync(
        int campaignId,
        int participantCharacterId,
        PartyExchangeStatus status,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        await EnsureCharacterAccessAsync(
            campaignId,
            participantCharacterId,
            actingUserId,
            cancellationToken );
        PartyExchange[] exchanges = await _inventoryDbContext.PartyExchanges
            .AsNoTracking()
            .Include( exchange => exchange.Lines )
            .Where( exchange =>
                exchange.CampaignId == campaignId &&
                exchange.Status == status &&
                ( exchange.InitiatorCharacterId == participantCharacterId ||
                  exchange.CounterpartyCharacterId == participantCharacterId ) )
            .OrderByDescending( exchange => exchange.CreatedAtUtc )
            .ThenBy( exchange => exchange.ExchangeKey )
            .ToArrayAsync( cancellationToken );
        Dictionary<Guid, InventoryOperationItemDto> items = await ReadItemsAsync(
            campaignId,
            exchanges.SelectMany( exchange => exchange.Lines )
                .Select( line => line.ItemInstanceKey ),
            cancellationToken );
        Dictionary<int, string> characterNames = await ReadCharacterNamesAsync(
            exchanges.SelectMany( exchange => new[]
            {
                exchange.InitiatorCharacterId,
                exchange.CounterpartyCharacterId,
            } ),
            cancellationToken );
        return exchanges
            .Select( exchange => new PartyExchangeProjectionDto(
                ToDto( exchange ),
                new InventoryCharacterReferenceDto(
                    exchange.InitiatorCharacterId,
                    characterNames[ exchange.InitiatorCharacterId ] ),
                new InventoryCharacterReferenceDto(
                    exchange.CounterpartyCharacterId,
                    characterNames[ exchange.CounterpartyCharacterId ] ),
                exchange.Lines
                    .Select( line => new PartyExchangeItemProjectionDto(
                        line.FromCharacterId,
                        items[ line.ItemInstanceKey ] ) )
                    .ToArray() ) )
            .ToArray();
    }

    public async Task<PartyExchangeProjectionDto> GetExchangeAsync(
        int campaignId,
        Guid exchangeKey,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        PartyExchange exchange = await _inventoryDbContext.PartyExchanges
            .AsNoTracking()
            .Include( item => item.Lines )
            .SingleOrDefaultAsync(
                item =>
                    item.CampaignId == campaignId &&
                    item.ExchangeKey == exchangeKey,
                cancellationToken )
            ?? throw new InventoryOperationsNotFoundException();
        CharacterCampaignAccess initiatorAccess =
            await _characterAccessPolicy.GetAccessAsync(
                campaignId,
                actingUserId,
                exchange.InitiatorCharacterId,
                cancellationToken );
        CharacterCampaignAccess counterpartyAccess =
            await _characterAccessPolicy.GetAccessAsync(
                campaignId,
                actingUserId,
                exchange.CounterpartyCharacterId,
                cancellationToken );
        if ( !initiatorAccess.CanView && !counterpartyAccess.CanView )
        {
            throw new InventoryOperationsAccessDeniedException();
        }

        Dictionary<Guid, InventoryOperationItemDto> items = await ReadItemsAsync(
            campaignId,
            exchange.Lines.Select( line => line.ItemInstanceKey ),
            cancellationToken );
        Dictionary<int, string> characterNames = await ReadCharacterNamesAsync(
            [
                exchange.InitiatorCharacterId,
                exchange.CounterpartyCharacterId,
            ],
            cancellationToken );
        return new PartyExchangeProjectionDto(
            ToDto( exchange ),
            new InventoryCharacterReferenceDto(
                exchange.InitiatorCharacterId,
                characterNames[ exchange.InitiatorCharacterId ] ),
            new InventoryCharacterReferenceDto(
                exchange.CounterpartyCharacterId,
                characterNames[ exchange.CounterpartyCharacterId ] ),
            exchange.Lines
                .Select( line => new PartyExchangeItemProjectionDto(
                    line.FromCharacterId,
                    items[ line.ItemInstanceKey ] ) )
                .ToArray() );
    }

    public async Task<ExchangeInventoryProjectionDto> GetExchangeInventoryAsync(
        int campaignId,
        int participantCharacterId,
        int ownerCharacterId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        CharacterCampaignAccess access = await _characterAccessPolicy.GetAccessAsync(
            campaignId,
            actingUserId,
            participantCharacterId,
            cancellationToken );
        if ( !access.CanAct )
        {
            throw new InventoryOperationsAccessDeniedException();
        }

        Campaign campaign = await _campaignDbContext.Campaigns
            .AsNoTracking()
            .Include( item => item.Parties )
                .ThenInclude( party => party.Characters )
            .SingleOrDefaultAsync(
                item =>
                    item.Id == campaignId &&
                    item.Status == CampaignStatus.Active,
                cancellationToken )
            ?? throw new InventoryOperationsAccessDeniedException();
        bool belongsToSameActiveParty = campaign.Parties.Any( party =>
            party.Status == CampaignPartyStatus.Active &&
            party.Characters.Any( character =>
                character.CharacterId == participantCharacterId ) &&
            party.Characters.Any( character =>
                character.CharacterId == ownerCharacterId ) );
        if ( !belongsToSameActiveParty )
        {
            throw new InventoryOperationsAccessDeniedException();
        }

        Guid[] containerKeys = await _inventoryDbContext.Containers
            .AsNoTracking()
            .Where(
                item =>
                    item.CampaignId == campaignId &&
                    item.OwnerKind == InventoryContainerOwnerKind.Character &&
                    item.OwnerId == ownerCharacterId )
            .Select( item => item.ContainerKey )
            .ToArrayAsync( cancellationToken );
        DraftCharacter character = await _characterDbContext.Character
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.Id == ownerCharacterId,
                cancellationToken )
            ?? throw new InventoryOperationsNotFoundException();
        if ( containerKeys.Length == 0 )
        {
            throw new InventoryOperationsNotFoundException();
        }

        HashSet<Guid> equippedItemKeys = character.RuntimeEquipmentItems
            .Where( item => item.IsEquipped )
            .Select( item => item.ItemInstanceKey )
            .ToHashSet();
        ItemInstance[] instances = await _inventoryDbContext.ItemInstances
            .AsNoTracking()
            .Where( item =>
                item.CampaignId == campaignId &&
                containerKeys.Contains( item.CurrentContainerKey ) &&
                item.Quantity > 0 &&
                item.ReservationKey == null &&
                !equippedItemKeys.Contains( item.InstanceKey ) )
            .OrderBy( item => item.CreatedAtUtc )
            .ThenBy( item => item.InstanceKey )
            .ToArrayAsync( cancellationToken );
        Dictionary<Guid, InventoryOperationItemDto> items = await ReadItemsAsync(
            campaignId,
            instances,
            cancellationToken );
        return new ExchangeInventoryProjectionDto(
            new InventoryCharacterReferenceDto(
                character.Id,
                character.Name ),
            instances
                .Select( item => items[ item.InstanceKey ] )
                .ToArray() );
    }

    public async Task<PartyStorageProjectionDto> GetPartyStorageAsync(
        int campaignId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        Campaign campaign = await _campaignDbContext.Campaigns
            .AsNoTracking()
            .Include( item => item.Memberships )
            .Include( item => item.Parties )
                .ThenInclude( party => party.Characters )
            .Include( item => item.Parties )
                .ThenInclude( party => party.Storage )
            .SingleOrDefaultAsync(
                item =>
                    item.Id == campaignId &&
                    item.Status == CampaignStatus.Active,
                cancellationToken )
            ?? throw new InventoryOperationsAccessDeniedException();
        bool isMember =
            campaign.HasActiveRole( actingUserId, CampaignMembershipRole.Player ) ||
            campaign.HasActiveRole( actingUserId, CampaignMembershipRole.GameMaster );
        CampaignParty party = campaign.Parties.SingleOrDefault( item =>
            item.Status == CampaignPartyStatus.Active )
            ?? throw new InventoryOperationsAccessDeniedException();
        if ( !isMember )
        {
            throw new InventoryOperationsAccessDeniedException();
        }

        InventoryContainer? container = await _inventoryDbContext.Containers
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    item.CampaignId == campaignId &&
                    item.OwnerKind == InventoryContainerOwnerKind.Party &&
                    item.OwnerId == party.Id,
                cancellationToken );
        ItemInstance[] storageItems = container is null
            ? []
            : await _inventoryDbContext.ItemInstances
                .AsNoTracking()
                .Include( item => item.Movements )
                .Where( item =>
                    item.CampaignId == campaignId &&
                    item.CurrentContainerKey == container.ContainerKey &&
                    item.Quantity > 0 )
                .OrderBy( item => item.CreatedAtUtc )
                .ThenBy( item => item.InstanceKey )
                .ToArrayAsync( cancellationToken );
        InventoryAuditEntry[] audits = await _inventoryDbContext.AuditEntries
            .AsNoTracking()
            .Where( audit =>
                audit.CampaignId == campaignId &&
                ( audit.ActionKind == InventoryAuditActionKind.PartyStorageDeposited ||
                  audit.ActionKind == InventoryAuditActionKind.PartyStorageWithdrawn ) )
            .OrderByDescending( audit => audit.OccurredAtUtc )
            .Take( 100 )
            .ToArrayAsync( cancellationToken );
        Guid[] auditItemKeys = audits
            .Where( audit => audit.ItemInstanceKey.HasValue )
            .Select( audit => audit.ItemInstanceKey!.Value )
            .ToArray();
        ItemInstance[] auditItems = await _inventoryDbContext.ItemInstances
            .AsNoTracking()
            .Include( item => item.Movements )
            .Where( item =>
                item.CampaignId == campaignId &&
                auditItemKeys.Contains( item.InstanceKey ) )
            .ToArrayAsync( cancellationToken );
        ItemInstance[] allItems = storageItems
            .Concat( auditItems )
            .DistinctBy( item => item.InstanceKey )
            .ToArray();
        Dictionary<Guid, InventoryOperationItemDto> itemDtos = await ReadItemsAsync(
            campaignId,
            allItems,
            cancellationToken );
        Guid[] movementContainerKeys = allItems
            .SelectMany( item => item.Movements )
            .SelectMany( movement => new[]
            {
                movement.FromContainerKey,
                movement.ToContainerKey,
            } )
            .Distinct()
            .ToArray();
        Dictionary<Guid, InventoryContainer> movementContainers =
            await _inventoryDbContext.Containers
                .AsNoTracking()
                .Where( item => movementContainerKeys.Contains( item.ContainerKey ) )
                .ToDictionaryAsync( item => item.ContainerKey, cancellationToken );
        Dictionary<Guid, ItemInstance> itemsByKey = allItems
            .ToDictionary( item => item.InstanceKey );
        int[] relatedCharacterIds = allItems
            .SelectMany( item => item.Movements )
            .SelectMany( movement => CharacterIdsForMovement(
                movement,
                movementContainers ) )
            .Distinct()
            .ToArray();
        Dictionary<int, string> characterNames = await ReadCharacterNamesAsync(
            relatedCharacterIds,
            cancellationToken );
        PartyStorageItemProjectionDto[] storageItemDtos = storageItems
            .Select( item => ToStorageItem(
                item,
                itemDtos[ item.InstanceKey ],
                container!.ContainerKey,
                movementContainers,
                characterNames ) )
            .ToArray();
        PartyStorageOperationProjectionDto[] operationDtos = audits
            .Where( audit =>
                audit.ItemInstanceKey.HasValue &&
                itemsByKey.ContainsKey( audit.ItemInstanceKey.Value ) &&
                container is not null &&
                IsStorageOperationForContainer(
                    audit,
                    itemsByKey[ audit.ItemInstanceKey.Value ],
                    container.ContainerKey ) )
            .Select( audit => ToStorageOperation(
                audit,
                itemsByKey[ audit.ItemInstanceKey!.Value ],
                itemDtos[ audit.ItemInstanceKey.Value ],
                movementContainers,
                characterNames ) )
            .Take( 10 )
            .ToArray();
        PartyStorageWithdrawalPolicy accessPolicy = party.Storage.AccessPolicy switch
        {
            CampaignPartyStorageAccessPolicy.FreeForMembers =>
                PartyStorageWithdrawalPolicy.FreeForMembers,
            CampaignPartyStorageAccessPolicy.GameMasterOnly =>
                PartyStorageWithdrawalPolicy.GameMasterOnly,
            _ => PartyStorageWithdrawalPolicy.Unconfigured,
        };
        return new PartyStorageProjectionDto(
            party.Id,
            accessPolicy,
            storageItemDtos,
            operationDtos );
    }

    private async Task EnsureCharacterAccessAsync(
        int campaignId,
        int characterId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        CharacterCampaignAccess access = await _characterAccessPolicy.GetAccessAsync(
            campaignId,
            actingUserId,
            characterId,
            cancellationToken );
        if ( !access.CanView )
        {
            throw new InventoryOperationsAccessDeniedException();
        }
    }

    private async Task<Dictionary<Guid, InventoryOperationItemDto>> ReadItemsAsync(
        int campaignId,
        IEnumerable<Guid> itemInstanceKeys,
        CancellationToken cancellationToken )
    {
        Guid[] keys = itemInstanceKeys
            .Distinct()
            .ToArray();
        ItemInstance[] instances = await _inventoryDbContext.ItemInstances
            .AsNoTracking()
            .Where( item =>
                item.CampaignId == campaignId &&
                keys.Contains( item.InstanceKey ) )
            .ToArrayAsync( cancellationToken );
        return await ReadItemsAsync( campaignId, instances, cancellationToken );
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

    private static PartyGiftDto ToDto( PartyGift gift ) =>
        new PartyGiftDto(
            gift.GiftKey,
            gift.CampaignId,
            gift.PartyId,
            gift.SourceCharacterId,
            gift.DestinationCharacterId,
            gift.ItemInstanceKey,
            gift.ExpectedItemVersion,
            gift.Status,
            gift.CreatedAtUtc,
            gift.ExpiresAtUtc,
            gift.AcceptedAtUtc,
            gift.AcceptanceOperationId );

    private static PartyExchangeDto ToDto( PartyExchange exchange ) =>
        new PartyExchangeDto(
            exchange.ExchangeKey,
            exchange.CampaignId,
            exchange.PartyId,
            exchange.InitiatorCharacterId,
            exchange.CounterpartyCharacterId,
            exchange.Status,
            exchange.CreatedAtUtc,
            exchange.ExpiresAtUtc,
            exchange.Version,
            exchange.CompletedAtUtc,
            exchange.CancelledAtUtc,
            exchange.FinalOperationId,
            exchange.Lines
                .Select( line => new PartyExchangeLineDto(
                    line.FromCharacterId,
                    line.ItemInstanceKey,
                    line.ExpectedItemVersion ) )
                .ToArray() );

    private static PartyStorageItemProjectionDto ToStorageItem(
        ItemInstance item,
        InventoryOperationItemDto itemDto,
        Guid partyContainerKey,
        IReadOnlyDictionary<Guid, InventoryContainer> containers,
        IReadOnlyDictionary<int, string> characterNames )
    {
        InventoryMovement? deposit = item.Movements
            .Where( movement =>
                movement.Reason == "party-storage-deposit" &&
                movement.ToContainerKey == partyContainerKey )
            .OrderByDescending( movement => movement.OccurredAtUtc )
            .FirstOrDefault();
        InventoryCharacterReferenceDto? depositedBy = deposit is null
            ? null
            : CharacterForContainer(
                deposit.FromContainerKey,
                containers,
                characterNames );
        return new PartyStorageItemProjectionDto(
            itemDto,
            depositedBy,
            deposit?.OccurredAtUtc );
    }

    private static PartyStorageOperationProjectionDto ToStorageOperation(
        InventoryAuditEntry audit,
        ItemInstance item,
        InventoryOperationItemDto itemDto,
        IReadOnlyDictionary<Guid, InventoryContainer> containers,
        IReadOnlyDictionary<int, string> characterNames )
    {
        InventoryMovement? movement = item.Movements.SingleOrDefault(
            candidate => candidate.OperationId == audit.OperationId );
        Guid? characterContainerKey = audit.ActionKind switch
        {
            InventoryAuditActionKind.PartyStorageDeposited => movement?.FromContainerKey,
            InventoryAuditActionKind.PartyStorageWithdrawn => movement?.ToContainerKey,
            _ => null,
        };
        InventoryCharacterReferenceDto? character = characterContainerKey.HasValue
            ? CharacterForContainer(
                characterContainerKey.Value,
                containers,
                characterNames )
            : null;
        return new PartyStorageOperationProjectionDto(
            audit.ActionKind,
            character,
            itemDto,
            audit.OccurredAtUtc );
    }

    private static bool IsStorageOperationForContainer(
        InventoryAuditEntry audit,
        ItemInstance item,
        Guid partyContainerKey )
    {
        InventoryMovement? movement = item.Movements.SingleOrDefault(
            candidate => candidate.OperationId == audit.OperationId );
        return audit.ActionKind switch
        {
            InventoryAuditActionKind.PartyStorageDeposited =>
                movement?.ToContainerKey == partyContainerKey,
            InventoryAuditActionKind.PartyStorageWithdrawn =>
                movement?.FromContainerKey == partyContainerKey,
            _ => false,
        };
    }

    private static IEnumerable<int> CharacterIdsForMovement(
        InventoryMovement movement,
        IReadOnlyDictionary<Guid, InventoryContainer> containers )
    {
        if ( containers.TryGetValue(
                movement.FromContainerKey,
                out InventoryContainer? source ) &&
             source.OwnerKind == InventoryContainerOwnerKind.Character )
        {
            yield return source.OwnerId;
        }

        if ( containers.TryGetValue(
                movement.ToContainerKey,
                out InventoryContainer? destination ) &&
             destination.OwnerKind == InventoryContainerOwnerKind.Character )
        {
            yield return destination.OwnerId;
        }
    }

    private static InventoryCharacterReferenceDto? CharacterForContainer(
        Guid containerKey,
        IReadOnlyDictionary<Guid, InventoryContainer> containers,
        IReadOnlyDictionary<int, string> characterNames )
    {
        if ( !containers.TryGetValue( containerKey, out InventoryContainer? container ) ||
             container.OwnerKind != InventoryContainerOwnerKind.Character ||
             !characterNames.TryGetValue( container.OwnerId, out string? name ) )
        {
            return null;
        }

        return new InventoryCharacterReferenceDto( container.OwnerId, name );
    }
}

public enum PartyGiftRole
{
    Incoming = 0,
    Outgoing = 1,
}

public sealed record InventoryOperationItemDto(
    Guid ItemInstanceKey,
    int Version,
    int Quantity,
    string Name,
    ItemCategory PrimaryCategory,
    int BulkTenths );

public sealed record InventoryCharacterReferenceDto(
    int CharacterId,
    string Name );

public sealed record PartyGiftProjectionDto(
    PartyGiftDto Gift,
    InventoryOperationItemDto Item,
    InventoryCharacterReferenceDto SourceCharacter,
    InventoryCharacterReferenceDto DestinationCharacter );

public sealed record PartyExchangeItemProjectionDto(
    int FromCharacterId,
    InventoryOperationItemDto Item );

public sealed record PartyExchangeProjectionDto(
    PartyExchangeDto Exchange,
    InventoryCharacterReferenceDto InitiatorCharacter,
    InventoryCharacterReferenceDto CounterpartyCharacter,
    IReadOnlyCollection<PartyExchangeItemProjectionDto> Items );

public sealed record ExchangeInventoryProjectionDto(
    InventoryCharacterReferenceDto Character,
    IReadOnlyCollection<InventoryOperationItemDto> Items );

public sealed record PartyStorageProjectionDto(
    int PartyId,
    PartyStorageWithdrawalPolicy AccessPolicy,
    IReadOnlyCollection<PartyStorageItemProjectionDto> Items,
    IReadOnlyCollection<PartyStorageOperationProjectionDto> RecentOperations );

public sealed record PartyStorageItemProjectionDto(
    InventoryOperationItemDto Item,
    InventoryCharacterReferenceDto? DepositedBy,
    DateTimeOffset? DepositedAtUtc );

public sealed record PartyStorageOperationProjectionDto(
    InventoryAuditActionKind Kind,
    InventoryCharacterReferenceDto? Character,
    InventoryOperationItemDto Item,
    DateTimeOffset OccurredAtUtc );

public sealed class InventoryOperationsAccessDeniedException : Exception
{
    public InventoryOperationsAccessDeniedException()
        : base( "Inventory operations access is denied." )
    {
    }
}

public sealed class InventoryOperationsNotFoundException : Exception
{
    public InventoryOperationsNotFoundException()
        : base( "Inventory operation was not found." )
    {
    }
}
