using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Access;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Movements;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Rules;

namespace Pathfinder.Web.Integration;

public sealed class CharacterInventoryProjectionService
{
    private readonly CharacterManagementDbContext _characterDbContext;
    private readonly InventoryDbContext _inventoryDbContext;
    private readonly InventoryItemCatalogProjectionReader _catalogReader;
    private readonly ICharacterCampaignAccessPolicy _accessPolicy;

    public CharacterInventoryProjectionService(
        CharacterManagementDbContext characterDbContext,
        InventoryDbContext inventoryDbContext,
        InventoryItemCatalogProjectionReader catalogReader,
        ICharacterCampaignAccessPolicy accessPolicy )
    {
        _characterDbContext = characterDbContext;
        _inventoryDbContext = inventoryDbContext;
        _catalogReader = catalogReader;
        _accessPolicy = accessPolicy;
    }

    public async Task<CharacterInventoryDto> GetAsync(
        int campaignId,
        int characterId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        CharacterCampaignAccess access = await _accessPolicy.GetAccessAsync(
            campaignId,
            actingUserId,
            characterId,
            cancellationToken );
        if ( !access.CanView )
        {
            throw new CharacterInventoryAccessDeniedException();
        }

        DraftCharacter character = await _characterDbContext.Character
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.Id == characterId,
                cancellationToken )
            ?? throw new CharacterInventoryNotFoundException();
        InventoryContainer[] containers = await _inventoryDbContext.Containers
            .AsNoTracking()
            .Where( container =>
                container.CampaignId == campaignId &&
                container.OwnerKind == InventoryContainerOwnerKind.Character &&
                container.OwnerId == characterId )
            .OrderBy( container => container.CreatedAtUtc )
            .ThenBy( container => container.ContainerKey )
            .ToArrayAsync( cancellationToken );
        Guid[] containerKeys = containers
            .Select( container => container.ContainerKey )
            .ToArray();
        ItemInstance[] instances = await _inventoryDbContext.ItemInstances
            .AsNoTracking()
            .Include( instance => instance.Movements )
            .Where( instance =>
                instance.CampaignId == campaignId &&
                instance.Quantity > 0 &&
                containerKeys.Contains( instance.CurrentContainerKey ) )
            .OrderBy( instance => instance.CreatedAtUtc )
            .ThenBy( instance => instance.InstanceKey )
            .ToArrayAsync( cancellationToken );
        Dictionary<int, InventoryItemCatalogProjection> catalog =
            await _catalogReader.ReadAsync( campaignId, instances, cancellationToken );
        HashSet<Guid> equippedInstanceKeys = character.RuntimeEquipmentItems
            .Where( item => item.IsEquipped )
            .Select( item => item.ItemInstanceKey )
            .ToHashSet();
        HashSet<Guid> migratedInstanceKeys = character.RuntimeEquipmentItems
            .Select( item => item.ItemInstanceKey )
            .ToHashSet();
        CharacterInventoryItemDto[] items = instances
            .Select( instance => ToDto(
                instance,
                catalog[ instance.ItemConfigurationId ],
                equippedInstanceKeys.Contains( instance.InstanceKey ),
                migratedInstanceKeys.Contains( instance.InstanceKey ) ) )
            .ToArray();
        int totalBulkTenths = items.Sum( item =>
            checked(item.Revision.BulkTenths * item.Quantity) );
        int strengthModifier = character.AbilityScores.Strength.Modifier;
        CharacterInventoryBulkDto bulk = new CharacterInventoryBulkDto(
            totalBulkTenths,
            Math.Max( 0, 5 + strengthModifier ) * 10,
            Math.Max( 0, 10 + strengthModifier ) * 10 );
        CharacterInventoryContainerDto[] containerDtos = containers
            .Select( container => new CharacterInventoryContainerDto(
                container.ContainerKey,
                container.OwnerKind ) )
            .ToArray();
        return new CharacterInventoryDto(
            campaignId,
            characterId,
            !access.CanAct,
            containerDtos,
            items,
            bulk );
    }

    private static CharacterInventoryItemDto ToDto(
        ItemInstance instance,
        InventoryItemCatalogProjection catalog,
        bool isEquipped,
        bool isMigratedStartingEquipment )
    {
        InventoryMovement? latestMovement = instance.Movements
            .OrderByDescending( movement => movement.OccurredAtUtc )
            .ThenByDescending( movement => movement.Id )
            .FirstOrDefault();
        CharacterInventoryProvenanceDto provenance = latestMovement is null
            ? new CharacterInventoryProvenanceDto(
                isMigratedStartingEquipment ? "StartingEquipment" : "Created",
                instance.CreatedAtUtc )
            : new CharacterInventoryProvenanceDto(
                latestMovement.Reason,
                latestMovement.OccurredAtUtc );
        CharacterInventoryRevisionDto revision = new CharacterInventoryRevisionDto(
            instance.CustomName ?? catalog.Name,
            catalog.RevisionNumber,
            catalog.Level,
            catalog.PrimaryCategory,
            catalog.PriceInCopperPieces,
            catalog.BulkTenths,
            catalog.Description );
        return new CharacterInventoryItemDto(
            instance.InstanceKey,
            instance.Version,
            instance.CurrentContainerKey,
            instance.Quantity,
            isEquipped,
            revision,
            provenance );
    }

}

public sealed record CharacterInventoryDto(
    int CampaignId,
    int CharacterId,
    bool IsReadOnly,
    IReadOnlyCollection<CharacterInventoryContainerDto> Containers,
    IReadOnlyCollection<CharacterInventoryItemDto> Items,
    CharacterInventoryBulkDto Bulk );

public sealed record CharacterInventoryContainerDto(
    Guid ContainerKey,
    InventoryContainerOwnerKind Kind );

public sealed record CharacterInventoryItemDto(
    Guid ItemInstanceKey,
    int Version,
    Guid ContainerKey,
    int Quantity,
    bool IsEquipped,
    CharacterInventoryRevisionDto Revision,
    CharacterInventoryProvenanceDto Provenance );

public sealed record CharacterInventoryRevisionDto(
    string Name,
    int RevisionNumber,
    int Level,
    ItemCategory PrimaryCategory,
    int PriceInCopperPieces,
    int BulkTenths,
    string Description );

public sealed record CharacterInventoryProvenanceDto(
    string Kind,
    DateTimeOffset OccurredAtUtc );

public sealed record CharacterInventoryBulkDto(
    int TotalTenths,
    int EncumberedAtTenths,
    int MaximumTenths );

public sealed class CharacterInventoryAccessDeniedException : Exception
{
    public CharacterInventoryAccessDeniedException()
        : base( "Character inventory access is denied." )
    {
    }
}

public sealed class CharacterInventoryNotFoundException : Exception
{
    public CharacterInventoryNotFoundException()
        : base( "Character inventory was not found." )
    {
    }
}