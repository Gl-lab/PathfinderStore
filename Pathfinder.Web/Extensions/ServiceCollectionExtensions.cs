using System;
using System.Text;
using Authorization.Authentication.Permissions;
using Authorization.Authentication.Role;
using Authorization.Authentication.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Utils.UnitOfWork;
using Pathfinder.Web.Authentication;
using Pathfinder.Web.Utils;
using Secure.Application;
using Secure.Application.Configuration;
using Secure.Infrastructure;
using Secure.Infrastructure.Data;

namespace Pathfinder.Web.Extensions;

public static class ServiceCollection
{
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
        services.AddDbContext<PathfinderDbContext>( options =>
            options
               .UseNpgsql( connectionString: configuration[ "DB:Main" ] ?? throw new InvalidOperationException("DB_CONNECTION for PathfinderDbContext not found") )
               .UseLazyLoadingProxies() );
        
        services.AddDbContext<SecureDbContext>( options =>
            options
               .UseNpgsql( connectionString: configuration[ "DB:Secure" ] ?? throw new InvalidOperationException("DB_CONNECTION for SecureDbContext not found") )
               .UseLazyLoadingProxies() );

        services.AddScoped<IUnitOfWork>( context =>
        {
            UnitOfWork unitOfWork = new();
            unitOfWork.AddDbContext( context.GetService<PathfinderDbContext>() );
            unitOfWork.AddDbContext( context.GetService<SecureDbContext>() );
            return unitOfWork;
        } );
    }

    public static void ConfigureDependencyInjection( this IServiceCollection services )
    {
        services.AddTransient<IAuthorizationHandler, PermissionHandler>();
    }

    public static void ConfigureJwtTokenAuth( this IServiceCollection services, IConfiguration configuration )
    {
        SymmetricSecurityKey signingKey =
            new(
                Encoding.ASCII.GetBytes( configuration[ "Authentication:SecurityKey" ] ?? throw new InvalidOperationException( "Authentication.SecurityKey for JwtTokenConfiguration not found") ) );

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
            jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateActor = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtTokenConfiguration.Issuer,
                ValidAudience = jwtTokenConfiguration.Audience,
                IssuerSigningKey = signingKey
            };
        } );
    }
}