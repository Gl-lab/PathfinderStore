using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;

namespace Pathfinder.Inventory.Infrastructure.Tests;

public sealed class InventoryPersistenceTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 17, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task ContextPersistsLocationMovementVersionAndOperationHistory()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        Guid instanceKey = Guid.NewGuid();
        Guid destinationKey;
        await using ( InventoryDbContext writeContext = new InventoryDbContext( options ) )
        {
            InventoryContainer source = CreateContainer( 31 );
            InventoryContainer destination = CreateContainer( 32 );
            destinationKey = destination.ContainerKey;
            ItemInstance instance = ItemInstance.Create(
                instanceKey,
                17,
                23,
                source,
                null,
                _createdAtUtc );
            writeContext.Containers.AddRange( source, destination );
            writeContext.ItemInstances.Add( instance );
            await writeContext.SaveChangesAsync();

            instance.MoveTo(
                destination,
                "transfer",
                0,
                Guid.NewGuid(),
                "user:31",
                _createdAtUtc.AddMinutes( 1 ) );
            await writeContext.SaveChangesAsync();
        }

        await using ( InventoryDbContext readContext = new InventoryDbContext( options ) )
        {
            ItemInstance instance = await readContext.ItemInstances
                .Include( item => item.Movements )
                .Include( item => item.Operations )
                .SingleAsync( item => item.InstanceKey == instanceKey );

            Assert.Equal( destinationKey, instance.CurrentContainerKey );
            Assert.Equal( 1, instance.Version );
            Assert.Single( instance.Movements );
            Assert.Single( instance.Operations );
        }
    }

    private static InventoryContainer CreateContainer( int ownerId )
    {
        return InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            17,
            InventoryContainerOwnerKind.Character,
            ownerId,
            _createdAtUtc );
    }
}
