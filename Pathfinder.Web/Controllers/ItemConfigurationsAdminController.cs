using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.ItemCatalog.Application.Configurations;
using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/item-catalog-admin/campaigns/{campaignId:int}/configurations" )]
public sealed class ItemConfigurationsAdminController : AuthorizedController
{
    private readonly ItemConfigurationAdministrationService _administrationService;

    public ItemConfigurationsAdminController(
        ItemConfigurationAdministrationService administrationService )
    {
        _administrationService = administrationService;
    }

    [HttpPost]
    public async Task<ActionResult<ItemConfigurationDto>> Create(
        int campaignId,
        [FromBody] CreateItemConfigurationApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            PermanentUpgrade[] upgrades = ( request.PermanentUpgrades ?? [] )
                .Select( item => item.ToDomain() )
                .ToArray();
            ItemConfigurationDto result = await _administrationService.CreateAsync(
                new CreateItemConfigurationRequest(
                    campaignId,
                    request.ItemDefinitionId,
                    request.RevisionNumber,
                    request.Size,
                    request.MaterialType,
                    request.MaterialGrade,
                    upgrades,
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
            exception is ItemCatalogApplicationException )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }
}

public sealed record CreateItemConfigurationApiRequest(
    int ItemDefinitionId,
    int RevisionNumber,
    ItemSize Size,
    ItemMaterialType MaterialType,
    ItemMaterialGrade MaterialGrade,
    IReadOnlyCollection<PermanentUpgradeApiRequest>? PermanentUpgrades );