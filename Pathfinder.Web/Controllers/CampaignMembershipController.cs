using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Pathfinder.CampaignManagement.Application.Campaigns;
using Pathfinder.CampaignManagement.Application.Exceptions;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Domain.Exceptions;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/campaign-membership" )]
public sealed class CampaignMembershipController : AuthorizedController
{
    private readonly IMediator _mediator;
    private readonly ILogger<CampaignMembershipController> _logger;

    public CampaignMembershipController(
        IMediator mediator,
        ILogger<CampaignMembershipController> logger )
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet( "invitations" )]
    public async Task<ActionResult<IReadOnlyCollection<CampaignInvitationDto>>> GetInvitations()
    {
        try
        {
            IReadOnlyCollection<CampaignInvitationDto> invitations = await _mediator.Send(
                new GetPendingCampaignInvitationsQuery( CurrentUserId() ) );
            return Ok( invitations );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "read campaign invitations" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "read campaign invitations" );
        }
    }

    [HttpPost( "campaigns/{campaignId:int}/invitations" )]
    public async Task<ActionResult> Invite(
        int campaignId,
        [FromBody] InviteCampaignMemberRequest request )
    {
        try
        {
            await _mediator.Send( new InviteCampaignMemberCommand(
                CurrentUserId(),
                campaignId,
                request.UserName ) );
            return NoContent();
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( ValidationException exception )
        {
            return BadRequest( MapValidation( exception ) );
        }
        catch ( CampaignManagementApplicationException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( CampaignManagementException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "invite a campaign member" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "invite a campaign member" );
        }
    }

    [HttpPost( "invitations/{invitationId:int}/accept" )]
    public Task<ActionResult<CampaignDto?>> Accept( int invitationId ) =>
        Respond( invitationId, true );

    [HttpPost( "invitations/{invitationId:int}/decline" )]
    public Task<ActionResult<CampaignDto?>> Decline( int invitationId ) =>
        Respond( invitationId, false );

    [HttpPost( "campaigns/{campaignId:int}/leave" )]
    public async Task<ActionResult> Leave( int campaignId )
    {
        try
        {
            await _mediator.Send( new LeaveCampaignCommand( CurrentUserId(), campaignId ) );
            return NoContent();
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( CampaignManagementApplicationException exception )
        {
            return NotFound( MapError( exception.Message ) );
        }
        catch ( CampaignManagementException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "leave a campaign" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "leave a campaign" );
        }
    }

    [HttpPut( "campaigns/{campaignId:int}/members/{memberUserId:int}/roles/{role}" )]
    public Task<ActionResult<CampaignDto>> AssignRole(
        int campaignId,
        int memberUserId,
        CampaignMembershipRole role ) => ChangeRole( campaignId, memberUserId, role, true );

    [HttpDelete( "campaigns/{campaignId:int}/members/{memberUserId:int}/roles/{role}" )]
    public Task<ActionResult<CampaignDto>> RevokeRole(
        int campaignId,
        int memberUserId,
        CampaignMembershipRole role ) => ChangeRole( campaignId, memberUserId, role, false );

    private async Task<ActionResult<CampaignDto?>> Respond( int invitationId, bool accept )
    {
        try
        {
            CampaignDto? campaign = await _mediator.Send(
                new RespondToCampaignInvitationCommand( CurrentUserId(), invitationId, accept ) );
            return campaign is null ? NoContent() : Ok( campaign );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( CampaignManagementApplicationException exception )
        {
            return NotFound( MapError( exception.Message ) );
        }
        catch ( CampaignManagementException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "respond to a campaign invitation" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "respond to a campaign invitation" );
        }
    }

    private async Task<ActionResult<CampaignDto>> ChangeRole(
        int campaignId,
        int memberUserId,
        CampaignMembershipRole role,
        bool assign )
    {
        try
        {
            CampaignDto campaign = await _mediator.Send( new ChangeCampaignRoleCommand(
                CurrentUserId(),
                campaignId,
                memberUserId,
                role,
                assign ) );
            return Ok( campaign );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( CampaignManagementApplicationException exception )
        {
            return NotFound( MapError( exception.Message ) );
        }
        catch ( CampaignManagementException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "change a campaign role" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "change a campaign role" );
        }
    }

    private ObjectResult DatabaseUnavailable( Exception exception, string operation )
    {
        _logger.LogError( exception, "Failed to {Operation} in the database.", operation );
        return StatusCode(
            StatusCodes.Status503ServiceUnavailable,
            MapError( "Campaign membership data is temporarily unavailable." ) );
    }
}

public sealed record InviteCampaignMemberRequest( string UserName );
