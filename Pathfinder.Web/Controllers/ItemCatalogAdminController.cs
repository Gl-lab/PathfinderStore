using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.ItemCatalog.Application.Administration;
using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/item-catalog-admin" )]
public sealed class ItemCatalogAdminController : AuthorizedController
{
    private readonly ItemCatalogAdministrationService _administrationService;

    public ItemCatalogAdminController( ItemCatalogAdministrationService administrationService )
    {
        _administrationService = administrationService;
    }

    [HttpPost( "drafts" )]
    public async Task<ActionResult<ItemRevisionDto>> CreateDraft(
        [FromBody] CreateItemDraftApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            ItemRevisionRules rules = request.Rules.ToDomain();
            CreateItemDraftRequest command = new CreateItemDraftRequest(
                request.Scope,
                request.CampaignId,
                request.Key,
                request.Name,
                request.Description,
                request.Level,
                request.PriceInCopperPieces,
                request.Bulk,
                rules,
                CurrentUserId(),
                CurrentUserName() );
            ItemRevisionDto result = await _administrationService.CreateDraftAsync(
                command,
                cancellationToken );
            return Ok( result );
        }
        catch ( ItemCatalogAccessDeniedException )
        {
            return Forbid();
        }
        catch ( Exception exception ) when (
            ( exception is ItemCatalogException ) ||
            ( exception is ItemCatalogApplicationException ) )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }

    [HttpPost( "definitions/{itemDefinitionId:int}/revisions/{revisionNumber:int}/publish" )]
    public async Task<ActionResult<ItemRevisionDto>> Publish(
        int itemDefinitionId,
        int revisionNumber,
        CancellationToken cancellationToken )
    {
        return await ChangeLifecycle(
            itemDefinitionId,
            revisionNumber,
            true,
            cancellationToken );
    }

    [HttpPost( "definitions/{itemDefinitionId:int}/revisions/{revisionNumber:int}/retire" )]
    public async Task<ActionResult<ItemRevisionDto>> Retire(
        int itemDefinitionId,
        int revisionNumber,
        CancellationToken cancellationToken )
    {
        return await ChangeLifecycle(
            itemDefinitionId,
            revisionNumber,
            false,
            cancellationToken );
    }

    private async Task<ActionResult<ItemRevisionDto>> ChangeLifecycle(
        int itemDefinitionId,
        int revisionNumber,
        bool publish,
        CancellationToken cancellationToken )
    {
        try
        {
            ItemRevisionDto result = publish
                ? await _administrationService.PublishAsync(
                    itemDefinitionId,
                    revisionNumber,
                    CurrentUserId(),
                    CurrentUserName(),
                    cancellationToken )
                : await _administrationService.RetireAsync(
                    itemDefinitionId,
                    revisionNumber,
                    CurrentUserId(),
                    CurrentUserName(),
                    cancellationToken );
            return Ok( result );
        }
        catch ( ItemCatalogAccessDeniedException )
        {
            return Forbid();
        }
        catch ( Exception exception ) when (
            ( exception is ItemCatalogException ) ||
            ( exception is ItemCatalogApplicationException ) )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }

    private string CurrentUserName() => User.Identity?.Name
        ?? throw new InvalidOperationException( "Current user name claim is missing." );
}

public sealed record CreateItemDraftApiRequest(
    ItemCatalogScope Scope,
    int? CampaignId,
    string Key,
    string Name,
    string Description,
    int Level,
    int PriceInCopperPieces,
    decimal Bulk,
    ItemRevisionRulesApiRequest Rules );

public sealed record ItemRevisionRulesApiRequest(
    ItemCategory PrimaryCategory,
    IReadOnlyCollection<AttackComponentApiRequest>? Attacks,
    ArmorComponentApiRequest? Armor,
    ShieldComponentApiRequest? Shield,
    EquipmentComponentApiRequest? Equipment,
    ConsumptionComponentApiRequest? Consumption,
    ChargeComponentApiRequest? Charges,
    DurabilityComponentApiRequest? Durability )
{
    public ItemRevisionRules ToDomain() => ItemRevisionRules.Create(
        PrimaryCategory,
        Attacks?.Select( attack => attack.ToDomain() ).ToArray(),
        Armor?.ToDomain(),
        Shield?.ToDomain(),
        Equipment?.ToDomain(),
        Consumption?.ToDomain(),
        Charges?.ToDomain(),
        Durability?.ToDomain() );
}

public sealed record AttackComponentApiRequest(
    string Name,
    int DamageDieCount,
    DamageDieSize DamageDieSize,
    ItemDamageType DamageType,
    int Hands,
    int? RangeIncrementFeet )
{
    public AttackComponent ToDomain() => AttackComponent.Create(
        Name,
        DamageDieCount,
        DamageDieSize,
        DamageType,
        Hands,
        RangeIncrementFeet );
}

public sealed record ArmorComponentApiRequest(
    ArmorCategory Category,
    int ArmorClassBonus,
    int DexterityCap,
    int CheckPenalty,
    int SpeedPenaltyFeet,
    int StrengthRequirement )
{
    public ArmorComponent ToDomain() => ArmorComponent.Create(
        Category,
        ArmorClassBonus,
        DexterityCap,
        CheckPenalty,
        SpeedPenaltyFeet,
        StrengthRequirement );
}

public sealed record ShieldComponentApiRequest( int RaisedArmorClassBonus )
{
    public ShieldComponent ToDomain() => ShieldComponent.Create( RaisedArmorClassBonus );
}

public sealed record EquipmentComponentApiRequest( EquipmentUsage Usage, int RequiredHands )
{
    public EquipmentComponent ToDomain() => EquipmentComponent.Create( Usage, RequiredHands );
}

public sealed record ConsumptionComponentApiRequest( ConsumptionMode Mode, int Quantity )
{
    public ConsumptionComponent ToDomain() => ConsumptionComponent.Create( Mode, Quantity );
}

public sealed record ChargeComponentApiRequest(
    int MaximumCharges,
    int DefaultActivationCost,
    ChargeRecoveryRule RecoveryRule )
{
    public ChargeComponent ToDomain() => ChargeComponent.Create(
        MaximumCharges,
        DefaultActivationCost,
        RecoveryRule );
}

public sealed record DurabilityComponentApiRequest(
    int Hardness,
    int MaximumHitPoints,
    int BrokenThreshold )
{
    public DurabilityComponent ToDomain() => DurabilityComponent.Create(
        Hardness,
        MaximumHitPoints,
        BrokenThreshold );
}