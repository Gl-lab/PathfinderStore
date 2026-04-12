using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pathfinder.CharacterManagement.Infrastructure.Data;

public class CharacterManagementDbContextFactory : IDesignTimeDbContextFactory<CharacterManagementDbContext>
{
    public CharacterManagementDbContext CreateDbContext( string[] args )
    {
        string environmentName = Environment.GetEnvironmentVariable( "DOTNET_ENVIRONMENT" )
            ?? Environment.GetEnvironmentVariable( "ASPNETCORE_ENVIRONMENT" )
            ?? "Development";
        string configurationBasePath = ResolveConfigurationBasePath();
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath( configurationBasePath )
            .AddJsonFile( "appsettings.json", optional: false )
            .AddJsonFile( $"appsettings.{environmentName}.json", optional: true )
            .AddUserSecrets( "c3db048d-4a24-483f-9e9a-fe0cc5bbd832" )
            .AddEnvironmentVariables()
            .Build();
        string connectionString = configuration[ "DB:CharacterManagement" ]
            ?? throw new InvalidOperationException(
                $"Connection string 'DB:CharacterManagement' was not found in '{configurationBasePath}'.");

        DbContextOptionsBuilder<CharacterManagementDbContext> builder = new DbContextOptionsBuilder<CharacterManagementDbContext>();
        builder.UseNpgsql( connectionString );
        return new CharacterManagementDbContext( builder.Options );
    }

    private static string ResolveConfigurationBasePath()
    {
        DirectoryInfo? current = new DirectoryInfo( Directory.GetCurrentDirectory() );

        while ( current is not null )
        {
            string candidate = Path.Combine( current.FullName, "Pathfinder.Web" );
            if ( File.Exists( Path.Combine( candidate, "appsettings.json" ) ) )
            {
                return candidate;
            }

            if ( File.Exists( Path.Combine( current.FullName, "appsettings.json" ) )
                && String.Equals( current.Name, "Pathfinder.Web", StringComparison.OrdinalIgnoreCase ) )
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException(
            "Could not locate the Pathfinder.Web configuration directory for design-time CharacterManagementDbContext creation." );
    }
}
