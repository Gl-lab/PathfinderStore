using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.ItemCatalog.Application.Administration;
using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;

namespace Pathfinder.Web.Integration;

public sealed class ItemCatalogAdministrationProjectionService
{
    private const int DefaultTake = 50;
    private const int MaximumTake = 200;

    private readonly ItemCatalogDbContext _itemCatalogDbContext;
    private readonly IItemCatalogAdministrativeAccess _administrativeAccess;

    public ItemCatalogAdministrationProjectionService(
        ItemCatalogDbContext itemCatalogDbContext,
        IItemCatalogAdministrativeAccess administrativeAccess )
    {
        _itemCatalogDbContext = itemCatalogDbContext;
        _administrativeAccess = administrativeAccess;
    }

    public async Task<ItemDefinitionAdministrationListDto> SearchDefinitionsAsync(
        ItemCatalogAdministrationSearchRequest request,
        CancellationToken cancellationToken )
    {
        if ( !Enum.IsDefined( request.Scope ) )
        {
            throw new ItemCatalogApplicationException( "Item catalog scope filter is invalid." );
        }

        if ( request.Status is ItemRevisionStatus requestedStatus &&
             !Enum.IsDefined( requestedStatus ) )
        {
            throw new ItemCatalogApplicationException( "Item revision status filter is invalid." );
        }

        bool canManageGlobal = await _administrativeAccess.CanManageGlobalCatalogAsync(
            request.ActingUserName,
            cancellationToken );
        bool canManageCampaign = ( request.CampaignId is int campaignId ) &&
            ( campaignId > 0 ) &&
            await _administrativeAccess.CanManageCampaignCatalogAsync(
                request.ActingUserId,
                campaignId,
                cancellationToken );
        if ( !canManageGlobal && !canManageCampaign )
        {
            throw new ItemCatalogAccessDeniedException(
                "Current user cannot browse the item catalog administration list." );
        }

        int requestedCampaignId = request.CampaignId ?? 0;
        IQueryable<ItemDefinition> query = _itemCatalogDbContext.ItemDefinitions
            .AsNoTracking()
            .Include( definition => definition.Revisions )
            .Where( definition =>
                ( ( definition.Scope == ItemCatalogScope.Global ) &&
                  ( canManageGlobal ||
                    definition.Revisions.Any( revision =>
                        revision.Status != ItemRevisionStatus.Draft ) ) ) ||
                ( canManageCampaign &&
                  ( definition.Scope == ItemCatalogScope.Campaign ) &&
                  ( definition.CampaignId == requestedCampaignId ) ) );
        query = request.Scope switch
        {
            ItemCatalogScopeFilter.Global => query.Where( definition =>
                definition.Scope == ItemCatalogScope.Global ),
            ItemCatalogScopeFilter.Campaign => query.Where( definition =>
                definition.Scope == ItemCatalogScope.Campaign ),
            ItemCatalogScopeFilter.All => query,
            _ => throw new ItemCatalogApplicationException( "Item catalog scope filter is invalid." ),
        };
        if ( request.Status is ItemRevisionStatus status )
        {
            query = query.Where( definition =>
                definition.Revisions.Any( revision =>
                    ( revision.Status == status ) &&
                    ( canManageGlobal ||
                      ( definition.Scope == ItemCatalogScope.Campaign ) ||
                      ( revision.Status != ItemRevisionStatus.Draft ) ) ) );
        }

        string normalizedSearch = request.Search?.Trim().ToLowerInvariant() ?? String.Empty;
        if ( normalizedSearch.Length > 0 )
        {
            query = query.Where( definition =>
                definition.Key.ToLower().Contains( normalizedSearch ) ||
                definition.Revisions.Any( revision =>
                    ( canManageGlobal ||
                      ( definition.Scope == ItemCatalogScope.Campaign ) ||
                      ( revision.Status != ItemRevisionStatus.Draft ) ) &&
                    revision.Name.ToLower().Contains( normalizedSearch ) ) );
        }

        int totalCount = await query.CountAsync( cancellationToken );
        int skip = Math.Max( 0, request.Skip );
        int take = request.Take <= 0
            ? DefaultTake
            : Math.Min( request.Take, MaximumTake );
        ItemDefinition[] definitions = await query
            .OrderBy( definition => definition.Key )
            .Skip( skip )
            .Take( take )
            .ToArrayAsync( cancellationToken );
        ItemDefinitionAdministrationDto[] items = definitions
            .Select( definition => new ItemDefinitionAdministrationDto(
                definition.Id,
                definition.Key,
                definition.Scope,
                definition.CampaignId,
                definition.CreatedAtUtc,
                definition.Revisions
                    .Where( revision =>
                        canManageGlobal ||
                        ( definition.Scope == ItemCatalogScope.Campaign ) ||
                        ( revision.Status != ItemRevisionStatus.Draft ) )
                    .OrderBy( revision => revision.RevisionNumber )
                    .Select( revision => new ItemRevisionSummaryDto(
                        revision.Id,
                        revision.RevisionNumber,
                        revision.Name,
                        revision.Description,
                        revision.Level,
                        revision.PriceInCopperPieces,
                        revision.Bulk,
                        revision.PrimaryCategory,
                        revision.Rarity,
                        revision.Status,
                        revision.CreatedAtUtc,
                        revision.PublishedAtUtc,
                        revision.RetiredAtUtc ) )
                    .ToArray() ) )
            .ToArray();
        return new ItemDefinitionAdministrationListDto( totalCount, items );
    }
}

public sealed record ItemCatalogAdministrationSearchRequest(
    ItemCatalogScopeFilter Scope,
    int? CampaignId,
    ItemRevisionStatus? Status,
    string? Search,
    int Skip,
    int Take,
    int ActingUserId,
    string ActingUserName );

public sealed record ItemDefinitionAdministrationListDto(
    int TotalCount,
    IReadOnlyCollection<ItemDefinitionAdministrationDto> Items );

public sealed record ItemDefinitionAdministrationDto(
    int ItemDefinitionId,
    string Key,
    ItemCatalogScope Scope,
    int? CampaignId,
    DateTimeOffset CreatedAtUtc,
    IReadOnlyCollection<ItemRevisionSummaryDto> Revisions );

public sealed record ItemRevisionSummaryDto(
    int ItemRevisionId,
    int RevisionNumber,
    string Name,
    string Description,
    int Level,
    int PriceInCopperPieces,
    decimal Bulk,
    ItemCategory PrimaryCategory,
    ItemRarity Rarity,
    ItemRevisionStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? PublishedAtUtc,
    DateTimeOffset? RetiredAtUtc );