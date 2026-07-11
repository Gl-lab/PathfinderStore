using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Pathfinder.Web;

public static class Program
{
    public static async Task Main( string[] args )
    {
        await CreateHostBuilder( args )
           .Build()
           .RunAsync()
           .ConfigureAwait( false );
    }

    private static IHostBuilder CreateHostBuilder( string[] args ) =>
        Host.CreateDefaultBuilder( args )
           .UseSerilog( ( context, services, loggerConfiguration ) => loggerConfiguration
               .ReadFrom
               .Configuration( context.Configuration )
               .ReadFrom
               .Services( services )
               .Enrich
               .FromLogContext()
               .Enrich
               .WithProperty( "Application", "Pathfinder.Web" )
               .Enrich
               .WithProperty( "Environment", context.HostingEnvironment.EnvironmentName ) )
           .ConfigureWebHostDefaults( webBuilder => webBuilder.UseStartup<Startup>() )
           .UseDefaultServiceProvider( ( _, options ) => { options.ValidateScopes = true; } );
}
