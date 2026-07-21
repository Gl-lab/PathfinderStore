using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pathfinder.CampaignManagement.Application.Campaigns;

namespace Pathfinder.CampaignManagement.Application;

public static class DependencyInjection
{
    public static void AddCampaignManagementApplicationServices( this IServiceCollection services )
    {
        services.AddSingleton( TimeProvider.System );
        services.AddTransient<IValidator<CreateCampaignCommand>, CreateCampaignCommandValidator>();
        services.AddTransient<IValidator<InviteCampaignMemberCommand>, InviteCampaignMemberCommandValidator>();
        services.AddTransient<IValidator<CreateCampaignPartyCommand>, CreateCampaignPartyCommandValidator>();
        services.AddMediatR( configuration =>
            configuration.RegisterServicesFromAssembly( typeof( DependencyInjection ).Assembly ) );
    }
}
