using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pathfinder.CampaignManagement.Infrastructure.Data;

public sealed class CampaignManagementDbContextFactory
    : IDesignTimeDbContextFactory<CampaignManagementDbContext>
{
    public CampaignManagementDbContext CreateDbContext( string[] args )
    {
        string environmentName = Environment.GetEnvironmentVariable( "DOTNET_ENVIRONMENT" )
            ?? Environment.GetEnvironmentVariable( "ASPNETCORE_ENVIRONMENT" )
            ?? "Development";
        string configurationBasePath = ResolveConfigurationBasePath();
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath( configurationBasePath )
            .AddJsonFile( "appsettings.json", optional: true )
            .AddJsonFile( "appsettings.Sample.json", optional: true )
            .AddJsonFile( $"appsettings.{environmentName}.json", optional: true )
            .AddUserSecrets( "c3db048d-4a24-483f-9e9a-fe0cc5bbd832" )
            .AddEnvironmentVariables()
            .Build();
        string connectionString = configuration[ "DB:CampaignManagement" ]
            ?? configuration[ "DB:CharacterManagement" ]
            ?? configuration[ "DB_CONNECTION" ]
            ?? throw new InvalidOperationException(
                $"Connection string 'DB:CampaignManagement' was not found in '{configurationBasePath}'." );
        DbContextOptionsBuilder<CampaignManagementDbContext> builder =
            new DbContextOptionsBuilder<CampaignManagementDbContext>();
        builder.UseNpgsql( connectionString );
        return new CampaignManagementDbContext( builder.Options );
    }

    private static string ResolveConfigurationBasePath()
    {
        DirectoryInfo? current = new DirectoryInfo( Directory.GetCurrentDirectory() );
        while ( current is not null )
        {
            string candidate = Path.Combine( current.FullName, "Pathfinder.Web" );
            if ( HasConfigurationFile( candidate ) )
            {
                return candidate;
            }

            if ( HasConfigurationFile( current.FullName ) &&
                 String.Equals( current.Name, "Pathfinder.Web", StringComparison.OrdinalIgnoreCase ) )
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException(
            "Could not locate the Pathfinder.Web configuration directory for design-time CampaignManagementDbContext creation." );
    }

    private static bool HasConfigurationFile( string directoryPath ) =>
        File.Exists( Path.Combine( directoryPath, "appsettings.json" ) ) ||
        File.Exists( Path.Combine( directoryPath, "appsettings.Sample.json" ) );
}
