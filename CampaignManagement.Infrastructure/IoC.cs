using Microsoft.Extensions.DependencyInjection;
using Pathfinder.CampaignManagement.Application.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Campaigns;
using Pathfinder.CharacterManagement.Application.Access;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Application.Storage;
using Pathfinder.Inventory.Application.Administration;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Application.Transactions;

namespace Pathfinder.CampaignManagement.Infrastructure;

public static class DependencyInjection
{
    public static void AddCampaignManagementInfrastructureServices( this IServiceCollection services )
    {
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<ICharacterCampaignAccessPolicy, CampaignCharacterAccessPolicy>();
        services.AddScoped<IPartyTransferAccessPolicy, PartyTransferAccessPolicy>();
        services.AddScoped<IPartyStorageAccessPolicy, PartyStorageAccessPolicy>();
        services.AddScoped<IInventoryGameMasterAccessPolicy, InventoryGameMasterAccessPolicy>();
        services.AddScoped<ICommerceCampaignAccessPolicy, InventoryGameMasterAccessPolicy>();
        services.AddScoped<ICommerceBuyerAccessPolicy, CommerceBuyerAccessPolicy>();
    }
}
