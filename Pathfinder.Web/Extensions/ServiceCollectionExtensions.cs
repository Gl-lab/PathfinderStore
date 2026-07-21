using System;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using Pathfinder.Secure.Application;
using Pathfinder.Secure.Application.Configuration;
using Pathfinder.Secure.Domain.Authentication.Permissions;
using Pathfinder.Secure.Domain.Authentication.Role;
using Pathfinder.Secure.Domain.Authentication.User;
using Pathfinder.Secure.Infrastructure;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.CampaignManagement.Application.Campaigns;
using Pathfinder.Secure.Infrastructure.Data;
using Pathfinder.Utils.UnitOfWork;
using Pathfinder.Web.Authentication;
using Pathfinder.Web.Utils;
using Pathfinder.Web.Integration;

namespace Pathfinder.Web.Extensions;

public static class ServiceCollection
{
    private static string GetSecureConnectionString( IConfiguration configuration ) =>
        configuration[ "DB:Secure" ]
        ?? configuration[ "DB_CONNECTION" ]
        ?? throw new InvalidOperationException( "Connection string 'DB:Secure' was not found. Configure 'DB:Secure' or legacy 'DB_CONNECTION'." );

    private static string GetCharacterManagementConnectionString( IConfiguration configuration ) =>
        configuration[ "DB:CharacterManagement" ]
        ?? configuration[ "DB_CONNECTION" ]
        ?? throw new InvalidOperationException( "Connection string 'DB:CharacterManagement' was not found. Configure 'DB:CharacterManagement' or legacy 'DB_CONNECTION'." );

    private static string GetCampaignManagementConnectionString( IConfiguration configuration ) =>
        configuration[ "DB:CampaignManagement" ]
        ?? configuration[ "DB:CharacterManagement" ]
        ?? configuration[ "DB_CONNECTION" ]
        ?? throw new InvalidOperationException( "Connection string 'DB:CampaignManagement' was not found. Configure 'DB:CampaignManagement'." );

    private static string GetSecurityKey( IConfiguration configuration ) =>
        configuration[ "Authentication:SecurityKey" ]
        ?? configuration[ "SECURITY_KEY" ]
        ?? throw new InvalidOperationException( "Authentication key 'Authentication:SecurityKey' was not found. Configure 'Authentication:SecurityKey' or legacy 'SECURITY_KEY'." );

    public static void ConfigureCors( this IServiceCollection services, IConfiguration configuration )
    {
        services.AddCors( options =>
        {
            options.AddPolicy( configuration[ "App:CorsOriginPolicyName" ] ?? String.Empty,
                builder =>
                    builder.WithOrigins( configuration[ "App:CorsOrigins" ]
                              ?.Split( ",", StringSplitOptions.RemoveEmptyEntries ) ?? [] )
                           .AllowAnyHeader()
                           .AllowAnyMethod() );
        } );
    }

    public static void ConfigureAuthentication( this IServiceCollection services )
    {
        services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<SecureDbContext>()
                .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>( options =>
        {
            // Password settings.
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredUniqueChars = 2;
            options.Lockout.AllowedForNewUsers = false;
            options.Lockout.MaxFailedAccessAttempts = 5;
        } );

        services.AddAuthorization( options =>
        {
            foreach ( Permission permission in DefaultPermissions.All() )
            {
                options.AddPolicy( permission.Name,
                    policy => policy.Requirements.Add( new PermissionRequirement( permission ) ) );
            }
        } );
        
        services.AddSecureInfrastructureServices();
        services.AddSecureApplicationServices();
    }

    public static void ConfigureDbContext( this IServiceCollection services, IConfiguration configuration )
    {
        // services.AddDbContext<StoreDbContext>( options =>
        //     options
        //        .UseNpgsql( connectionString: configuration[ "DB:Main" ] ?? throw new InvalidOperationException("DB_CONNECTION for PathfinderDbContext not found") )
        //        .UseLazyLoadingProxies() );
        
        services.AddDbContext<SecureDbContext>( options =>
            options
               .UseNpgsql( connectionString: GetSecureConnectionString( configuration ) ) );

        services.AddDbContext<CharacterManagementDbContext>( options =>
            options
               .UseNpgsql( connectionString: GetCharacterManagementConnectionString( configuration ) ) );

        services.AddDbContext<CampaignManagementDbContext>( options =>
            options
               .UseNpgsql( connectionString: GetCampaignManagementConnectionString( configuration ) ) );

        services.AddScoped<IUnitOfWork>( context =>
        {
            UnitOfWork unitOfWork = new();
            // unitOfWork.AddDbContext( context.GetService<StoreDbContext>() );
            unitOfWork.AddDbContext( context.GetRequiredService<SecureDbContext>() );
            unitOfWork.AddDbContext( context.GetRequiredService<CharacterManagementDbContext>() );
            unitOfWork.AddDbContext( context.GetRequiredService<CampaignManagementDbContext>() );
            return unitOfWork;
        } );
    }

    public static void ConfigureDependencyInjection( this IServiceCollection services )
    {
        services.AddTransient<IAuthorizationHandler, PermissionHandler>();
        services.AddScoped<ICampaignUserDirectory, CampaignUserDirectory>();
    }

    public static void ConfigureJwtTokenAuth( this IServiceCollection services, IConfiguration configuration )
    {
        SymmetricSecurityKey signingKey =
            new(
                Encoding.ASCII.GetBytes( GetSecurityKey( configuration ) ) );

        JwtTokenConfiguration jwtTokenConfiguration = new JwtTokenConfiguration
        {
            Issuer = configuration[ "Authentication:JwtBearer:Issuer" ] ?? throw new InvalidOperationException("Authentication:JwtBearer:Issuer not found"),
            Audience = configuration[ "Authentication:JwtBearer:Audience" ] ?? throw new InvalidOperationException("Authentication:JwtBearer:Audience not found"),
            SigningCredentials = new SigningCredentials( signingKey, SecurityAlgorithms.HmacSha256 ),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays( 14 ),
        };

        services.Configure<JwtTokenConfiguration>( config =>
        {
            config.Audience = jwtTokenConfiguration.Audience;
            config.EndDate = jwtTokenConfiguration.EndDate;
            config.Issuer = jwtTokenConfiguration.Issuer;
            config.StartDate = jwtTokenConfiguration.StartDate;
            config.SigningCredentials = jwtTokenConfiguration.SigningCredentials;
        } );

        services.AddAuthentication( options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        } ).AddJwtBearer( jwtBearerOptions =>
        {
            string environmentName = Environment.GetEnvironmentVariable( "ASPNETCORE_ENVIRONMENT" ) ?? String.Empty;
            bool isDevelopment = String.Equals( environmentName, "Development", StringComparison.OrdinalIgnoreCase );

            jwtBearerOptions.MapInboundClaims = false;
            jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateActor = false,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtTokenConfiguration.Issuer,
                ValidAudience = jwtTokenConfiguration.Audience,
                IssuerSigningKey = signingKey,
                NameClaimType = JwtRegisteredClaimNames.Sub
            };

            jwtBearerOptions.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    if ( !isDevelopment )
                    {
                        return;
                    }

                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json; charset=utf-8";

                    string payload = JsonSerializer.Serialize( new
                    {
                        error = context.Error,
                        errorDescription = context.ErrorDescription,
                    } );

                    await context.Response.WriteAsync( payload );
                }
            };
        } );
    }
}
