using Microsoft.Extensions.DependencyInjection;
using Pathfinder.CampaignManagement.Application.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Campaigns;

namespace Pathfinder.CampaignManagement.Infrastructure;

public static class DependencyInjection
{
    public static void AddCampaignManagementInfrastructureServices( this IServiceCollection services )
    {
        services.AddScoped<ICampaignRepository, CampaignRepository>();
    }
}
