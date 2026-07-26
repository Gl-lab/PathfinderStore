using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Web.Controllers.Base;
using Pathfinder.Web.Integration;

namespace Pathfinder.Web.Controllers;

[Route( "api/item-catalog-admin/campaigns/{campaignId:int}/unique-items" )]
public sealed class UniqueItemsAdminController : AuthorizedController
{
    private readonly UniqueItemAdministrationService _administrationService;

    public UniqueItemsAdminController(
        UniqueItemAdministrationService administrationService )
    {
        _administrationService = administrationService;
    }

    [HttpPost]
    public async Task<ActionResult<UniqueItemDto>> Create(
        int campaignId,
        [FromBody] CreateUniqueItemApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            PermanentUpgrade[] upgrades = request.PermanentUpgrades
                .Select( item => item.ToDomain() )
                .ToArray();
            UniqueItemDto result = await _administrationService.CreateAsync(
                new CreateUniqueItemRequest(
                    campaignId,
                    request.ItemDefinitionId,
                    request.RevisionNumber,
                    request.Size,
                    request.MaterialType,
                    request.MaterialGrade,
                    upgrades,
                    request.InstanceKey,
                    request.ContainerKey,
                    request.CustomName,
                    CurrentUserId() ),
                cancellationToken );
            return Ok( result );
        }
        catch ( ItemCatalogAccessDeniedException )
        {
            return Forbid();
        }
        catch ( Exception exception ) when (
            exception is ItemCatalogException ||
            exception is ItemCatalogApplicationException ||
            exception is InventoryException )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }
}

public sealed record CreateUniqueItemApiRequest(
    int ItemDefinitionId,
    int RevisionNumber,
    ItemSize Size,
    ItemMaterialType MaterialType,
    ItemMaterialGrade MaterialGrade,
    IReadOnlyCollection<PermanentUpgradeApiRequest> PermanentUpgrades,
    Guid InstanceKey,
    Guid ContainerKey,
    string? CustomName );

public sealed record PermanentUpgradeApiRequest(
    string Code,
    PermanentUpgradeKind Kind,
    int Rank,
    PermanentUpgradeVisibility Visibility )
{
    public PermanentUpgrade ToDomain() =>
        PermanentUpgrade.Create( Code, Kind, Rank, Visibility );
}