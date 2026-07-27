using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Operations;
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

    [Fact]
    public async Task ContextPersistsChargeStateAndOperationHistory()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        Guid instanceKey = Guid.NewGuid();
        await using ( InventoryDbContext writeContext = new InventoryDbContext( options ) )
        {
            InventoryContainer container = CreateContainer( 31 );
            ItemInstance instance = ItemInstance.CreateCharged(
                instanceKey,
                17,
                23,
                3,
                1,
                ItemChargeRecoveryRule.DailyPreparations,
                container,
                null,
                _createdAtUtc );
            writeContext.Containers.Add( container );
            writeContext.ItemInstances.Add( instance );
            await writeContext.SaveChangesAsync();

            instance.ConsumeDefaultCharges(
                0,
                Guid.NewGuid(),
                _createdAtUtc.AddMinutes( 1 ) );
            await writeContext.SaveChangesAsync();
        }

        await using ( InventoryDbContext readContext = new InventoryDbContext( options ) )
        {
            ItemInstance instance = await readContext.ItemInstances
                .Include( item => item.Operations )
                .SingleAsync( item => item.InstanceKey == instanceKey );

            Assert.Equal( 3, instance.MaximumCharges );
            Assert.Equal( 2, instance.CurrentCharges );
            Assert.Equal( 1, instance.DefaultActivationCost );
            Assert.Equal(
                ItemChargeRecoveryRule.DailyPreparations,
                instance.ChargeRecoveryRule );
            InventoryOperation operation = Assert.Single( instance.Operations );
            Assert.Equal( InventoryOperationKind.ConsumeCharges, operation.Kind );
        }
    }

    [Fact]
    public async Task ContextPersistsConsumableStackState()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        Guid instanceKey = Guid.NewGuid();
        await using ( InventoryDbContext writeContext = new InventoryDbContext( options ) )
        {
            InventoryContainer container = CreateContainer( 31 );
            ItemInstance instance = ItemInstance.CreateConsumableStack(
                instanceKey,
                17,
                23,
                10,
                ItemConsumptionMode.ConsumeAmmunition,
                1,
                container,
                null,
                _createdAtUtc );
            writeContext.Containers.Add( container );
            writeContext.ItemInstances.Add( instance );
            await writeContext.SaveChangesAsync();

            instance.Consume(
                2,
                0,
                Guid.NewGuid(),
                _createdAtUtc.AddMinutes( 1 ) );
            await writeContext.SaveChangesAsync();
        }

        await using ( InventoryDbContext readContext = new InventoryDbContext( options ) )
        {
            ItemInstance instance = await readContext.ItemInstances
                .Include( item => item.Operations )
                .SingleAsync( item => item.InstanceKey == instanceKey );

            Assert.Equal( ItemConsumptionMode.ConsumeAmmunition, instance.ConsumptionMode );
            Assert.Equal( 1, instance.ConsumptionQuantity );
            Assert.Equal( 8, instance.Quantity );
            InventoryOperation operation = Assert.Single( instance.Operations );
            Assert.Equal( InventoryOperationKind.ConsumeItem, operation.Kind );
        }
    }

    [Fact]
    public async Task ContextPersistsDurabilityState()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        Guid instanceKey = Guid.NewGuid();
        await using ( InventoryDbContext writeContext = new InventoryDbContext( options ) )
        {
            InventoryContainer container = CreateContainer( 31 );
            ItemInstance instance = ItemInstance.CreateDurable(
                instanceKey,
                17,
                23,
                5,
                20,
                10,
                container,
                null,
                _createdAtUtc );
            writeContext.Containers.Add( container );
            writeContext.ItemInstances.Add( instance );
            await writeContext.SaveChangesAsync();

            instance.ApplyDamage(
                15,
                0,
                Guid.NewGuid(),
                _createdAtUtc.AddMinutes( 1 ) );
            await writeContext.SaveChangesAsync();
        }

        await using ( InventoryDbContext readContext = new InventoryDbContext( options ) )
        {
            ItemInstance instance = await readContext.ItemInstances
                .Include( item => item.Operations )
                .SingleAsync( item => item.InstanceKey == instanceKey );

            Assert.Equal( 5, instance.Hardness );
            Assert.Equal( 20, instance.MaximumHitPoints );
            Assert.Equal( 10, instance.CurrentHitPoints );
            Assert.Equal( 10, instance.BrokenThreshold );
            Assert.True( instance.IsBroken );
            InventoryOperation operation = Assert.Single( instance.Operations );
            Assert.Equal( InventoryOperationKind.DamageItem, operation.Kind );
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
