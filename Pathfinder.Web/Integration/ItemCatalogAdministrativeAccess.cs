using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.ItemCatalog.Application.Administration;
using Pathfinder.Secure.Application.Services.Authentication;
using Pathfinder.Secure.Domain.Authentication.Permissions;

namespace Pathfinder.Web.Integration;

public sealed class ItemCatalogAdministrativeAccess : IItemCatalogAdministrativeAccess
{
    private readonly IPermissionService _permissionService;
    private readonly CampaignManagementDbContext _campaignDbContext;

    public ItemCatalogAdministrativeAccess(
        IPermissionService permissionService,
        CampaignManagementDbContext campaignDbContext )
    {
        _permissionService = permissionService;
        _campaignDbContext = campaignDbContext;
    }

    public async Task<bool> CanManageGlobalCatalogAsync(
        string userName,
        CancellationToken cancellationToken )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _permissionService.IsUserGrantedToPermissionAsync(
            userName,
            DefaultPermissions.PermissionNameForAdministration );
    }

    public async Task<bool> CanManageCampaignCatalogAsync(
        int userId,
        int campaignId,
        CancellationToken cancellationToken )
    {
        return await _campaignDbContext.Campaigns
            .AsNoTracking()
            .AnyAsync(
                campaign =>
                    ( campaign.Id == campaignId ) &&
                    ( campaign.Status == CampaignStatus.Active ) &&
                    campaign.Memberships.Any( membership =>
                        ( membership.UserId == userId ) &&
                        ( membership.Role == CampaignMembershipRole.GameMaster ) &&
                        ( membership.Status == CampaignMembershipStatus.Active ) ),
                cancellationToken );
    }
}