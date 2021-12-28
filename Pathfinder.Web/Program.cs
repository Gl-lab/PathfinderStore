using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Pathfinder.Web;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync().ConfigureAwait(false);
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .UseDefaultServiceProvider((_, options) => { options.ValidateScopes = true; });
}