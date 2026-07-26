using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.ItemCatalog.Domain.Knowledge;
using Pathfinder.Web.Controllers.Base;
using Pathfinder.Web.Integration;

namespace Pathfinder.Web.Controllers;

[Route( "api/item-catalog-admin/campaigns/{campaignId:int}/items/{instanceKey:guid}/knowledge" )]
public sealed class ItemKnowledgeAdminController : AuthorizedController
{
    private readonly ItemKnowledgeAdministrationService _knowledgeService;

    public ItemKnowledgeAdminController(
        ItemKnowledgeAdministrationService knowledgeService )
    {
        _knowledgeService = knowledgeService;
    }

    [HttpPost( "reveal" )]
    public async Task<ActionResult<ItemPropertyKnowledgeDto>> Reveal(
        int campaignId,
        Guid instanceKey,
        [FromBody] RevealItemPropertyApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            ItemPropertyKnowledgeDto result = await _knowledgeService.RevealAsync(
                new RevealItemPropertyRequest(
                    campaignId,
                    instanceKey,
                    request.SubjectKind,
                    request.SubjectId,
                    request.UpgradeCode,
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
            exception is InventoryException ||
            exception is InvalidOperationException )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }
}

public sealed record RevealItemPropertyApiRequest(
    ItemKnowledgeSubjectKind SubjectKind,
    int SubjectId,
    string UpgradeCode );