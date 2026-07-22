using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pathfinder.CharacterManagement.Application;
using Pathfinder.CharacterManagement.Application.Equipment;
using Pathfinder.CharacterManagement.Infrastructure;
using Pathfinder.CampaignManagement.Application;
using Pathfinder.CampaignManagement.Infrastructure;
using Pathfinder.ItemCatalog.Application;
using Pathfinder.ItemCatalog.Infrastructure;
using Pathfinder.CharacterManagement.Infrastructure.Consumers;
using Pathfinder.Web.Extensions;
using Pathfinder.Web.Integration;
using Serilog;
using SerilogLogContext = Serilog.Context.LogContext;

namespace Pathfinder.Web;

public class Startup( IConfiguration configuration )
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices( IServiceCollection services )
    {
        services.ConfigureDbContext( Configuration );
        services.ConfigureAuthentication();
        services.ConfigureJwtTokenAuth( Configuration );
        services.ConfigureCors( Configuration );
        services.ConfigureDependencyInjection();
        services.AddCharacterManagementApplicationServices();
        services.AddCharacterManagementInfrastructureServices();
        services.AddCampaignManagementApplicationServices();
        services.AddCampaignManagementInfrastructureServices();
        services.AddItemCatalogApplicationServices();
        services.AddItemCatalogInfrastructureServices();
        services.AddScoped<CompletedCharacterInventoryMigrationService>();
        services.AddHostedService<CompletedCharacterInventoryMigrationHostedService>();
        services.AddScoped<ItemCatalogAllowedEquipmentReader>();
        services.AddScoped<IAllowedEquipmentReader, RuntimeInventoryAllowedEquipmentReader>();

        services.AddControllers()
            .AddJsonOptions(
                options =>
                {
                    options.JsonSerializerOptions.Converters.Add( new JsonStringEnumConverter() );
                } );

        services.AddSwaggerGen( c =>
        {
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Pathfinde API",
                    Version = "v1"
                } );
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                } );
            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        [ ]
                    }
                } );
        } );

        services.AddSpaStaticFiles( configuration => configuration.RootPath = "ClientApp/dist" );

        // services.AddStoreInfrastructure();
        // services.AddStoreApplication();
        services.AddMassTransit( busConfigurator =>
        {
            busConfigurator.AddConsumer<UserRegisteredEventConsumer>();
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingInMemory( ( context, configurator ) =>
            {
                configurator.UseInMemoryOutbox( context );
                configurator.ConfigureEndpoints( context );
            } );
        } );
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        IServiceProvider serviceProvider )
    {
        if ( env.IsDevelopment() )
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseSwagger();
        app.UseSwaggerUI( c => c.SwaggerEndpoint( "/swagger/v1/swagger.json", "Pathfinder API V1" ) );
        app.UseCors( Configuration[ "App:CorsOriginPolicyName" ] );

        app.UseRouting();

        app.Use( async ( context, next ) =>
        {
            string traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
            using IDisposable traceIdContext = SerilogLogContext.PushProperty( "TraceId", traceId );
            using IDisposable requestIdContext = SerilogLogContext.PushProperty( "RequestId", context.TraceIdentifier );
            await next();
        } );

        app.UseSerilogRequestLogging();

        app.UseForwardedHeaders( new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto } );
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints( endpoints => endpoints.MapControllers() );

        //PathfinderSeed.SeedAsync(serviceProvider).Wait();
    }
}
