using Microsoft.Extensions.DependencyInjection;
using Pathfinder.CampaignManagement.Application.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Campaigns;
using Pathfinder.CharacterManagement.Application.Access;
using Pathfinder.Inventory.Application.Transfers;

namespace Pathfinder.CampaignManagement.Infrastructure;

public static class DependencyInjection
{
    public static void AddCampaignManagementInfrastructureServices( this IServiceCollection services )
    {
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<ICharacterCampaignAccessPolicy, CampaignCharacterAccessPolicy>();
        services.AddScoped<IPartyTransferAccessPolicy, PartyTransferAccessPolicy>();
    }
}
