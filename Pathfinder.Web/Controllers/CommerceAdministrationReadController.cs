using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Web.Controllers.Base;
using Pathfinder.Web.Integration;

namespace Pathfinder.Web.Controllers;

[Route( "api/commerce-admin/campaigns/{campaignId:int}" )]
public sealed class CommerceAdministrationReadController : AuthorizedController
{
    private readonly CommerceAdministrationProjectionService _projectionService;

    public CommerceAdministrationReadController(
        CommerceAdministrationProjectionService projectionService )
    {
        _projectionService = projectionService;
    }

    [HttpGet( "wallets" )]
    public async Task<ActionResult<IReadOnlyCollection<CommerceWalletSummaryDto>>> GetWallets(
        int campaignId,
        CancellationToken cancellationToken )
    {
        return await Execute(
            () => _projectionService.GetWalletsAsync(
                campaignId,
                CurrentUserId(),
                cancellationToken ) );
    }

    [HttpGet( "wallets/{characterId:int}" )]
    public async Task<ActionResult<WalletDto>> GetWallet(
        int campaignId,
        int characterId,
        CancellationToken cancellationToken )
    {
        return await Execute(
            () => _projectionService.GetWalletAsync(
                campaignId,
                characterId,
                CurrentUserId(),
                cancellationToken ) );
    }

    [HttpGet( "~/api/campaigns/{campaignId:int}/inventory/containers" )]
    public async Task<ActionResult<IReadOnlyCollection<InventoryContainerAdministrationDto>>>
        GetContainers(
            int campaignId,
            CancellationToken cancellationToken )
    {
        return await Execute(
            () => _projectionService.GetContainersAsync(
                campaignId,
                CurrentUserId(),
                cancellationToken ) );
    }

    [HttpGet( "~/api/item-catalog/revisions" )]
    public async Task<
        ActionResult<IReadOnlyCollection<PublishedItemRevisionAdministrationDto>>>
        SearchPublishedRevisions(
            [FromQuery] int campaignId,
            [FromQuery] string? search,
            [FromQuery] ItemCatalogScopeFilter scope = ItemCatalogScopeFilter.All,
            CancellationToken cancellationToken = default )
    {
        return await Execute(
            () => _projectionService.SearchPublishedRevisionsAsync(
                campaignId,
                search,
                scope,
                CurrentUserId(),
                cancellationToken ) );
    }

    private static async Task<ActionResult<T>> Execute<T>(
        Func<Task<T>> action )
    {
        try
        {
            return new OkObjectResult( await action() );
        }
        catch ( UnauthorizedAccessException )
        {
            return new ForbidResult();
        }
        catch ( CommerceAdministrationProjectionNotFoundException )
        {
            return new NotFoundResult();
        }
    }
}
