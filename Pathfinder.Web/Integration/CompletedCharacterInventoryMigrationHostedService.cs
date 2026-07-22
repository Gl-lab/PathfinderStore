using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pathfinder.Web.Integration;

public sealed class CompletedCharacterInventoryMigrationHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CompletedCharacterInventoryMigrationHostedService> _logger;

    public CompletedCharacterInventoryMigrationHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<CompletedCharacterInventoryMigrationHostedService> logger )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync( CancellationToken cancellationToken )
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        CompletedCharacterInventoryMigrationService migrationService =
            scope.ServiceProvider.GetRequiredService<CompletedCharacterInventoryMigrationService>();
        IReadOnlyCollection<CompletedCharacterInventoryMigrationResult> results =
            await migrationService.MigratePendingAsync(
                DateTimeOffset.UtcNow,
                cancellationToken );
        int migratedCount = results.Count( result => result.Status == "Migrated" );
        int skippedCount = results.Count - migratedCount;
        _logger.LogInformation(
            "Completed-character inventory migration finished: {MigratedCount} migrated, {SkippedCount} skipped.",
            migratedCount,
            skippedCount );
    }

    public Task StopAsync( CancellationToken cancellationToken ) => Task.CompletedTask;
}
