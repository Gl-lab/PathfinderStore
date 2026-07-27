using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Operations;

namespace Pathfinder.Inventory.Domain.Tests;

public sealed class ItemConsumptionTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 27, 18, 0, 0, TimeSpan.Zero );

    [Fact]
    public void UniqueConsumableEndsLifecycleAfterOneUse()
    {
        ItemInstance item = ItemInstance.CreateConsumable(
            Guid.NewGuid(),
            17,
            23,
            ItemConsumptionMode.DestroyInstance,
            1,
            CreateContainer(),
            null,
            _createdAtUtc );

        Assert.True( item.Consume(
            1,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) ) );

        Assert.True( item.IsDepleted );
        Assert.Equal( 0, item.Quantity );
        Assert.Equal( 1, item.Version );
        InventoryOperation operation = Assert.Single( item.Operations );
        Assert.Equal( InventoryOperationKind.ConsumeItem, operation.Kind );
        Assert.Equal( 1, operation.Quantity );
    }

    [Theory]
    [InlineData( ItemConsumptionMode.ReduceStack )]
    [InlineData( ItemConsumptionMode.ConsumeAmmunition )]
    public void StackConsumptionUsesConfiguredQuantity( ItemConsumptionMode mode )
    {
        ItemInstance item = CreateStack( 10, mode, 2 );

        item.Consume(
            3,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) );

        Assert.Equal( 4, item.Quantity );
        Assert.False( item.IsDepleted );
        Assert.Equal( 6, Assert.Single( item.Operations ).Quantity );
    }

    [Fact]
    public void ConcurrentConsumptionOfLastStackUnitOnlySucceedsOnce()
    {
        ItemInstance item = CreateStack( 1, ItemConsumptionMode.ConsumeAmmunition, 1 );

        Assert.True( item.Consume(
            1,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) ) );
        Assert.Throws<InventoryException>( () => item.Consume(
            1,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) ) );
        Assert.Equal( 0, item.Quantity );
    }

    [Fact]
    public void ConsumptionReplayIsIdempotentAndRejectsDifferentUseCount()
    {
        ItemInstance item = CreateStack( 5, ItemConsumptionMode.ReduceStack, 1 );
        Guid operationId = Guid.NewGuid();

        Assert.True( item.Consume(
            1,
            0,
            operationId,
            _createdAtUtc.AddMinutes( 1 ) ) );
        Assert.False( item.Consume(
            1,
            0,
            operationId,
            _createdAtUtc.AddMinutes( 1 ) ) );
        Assert.Throws<InventoryException>( () => item.Consume(
            2,
            0,
            operationId,
            _createdAtUtc.AddMinutes( 1 ) ) );
        Assert.Equal( 4, item.Quantity );
        Assert.Equal( 1, item.Version );
    }

    [Fact]
    public void SplitPreservesConsumptionProfileAndMergeRequiresSameProfile()
    {
        ItemInstance source = CreateStack( 5, ItemConsumptionMode.ReduceStack, 1 );
        ItemSplitResult result = source.Split(
            Guid.NewGuid(),
            2,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) );
        ItemInstance split = Assert.IsType<ItemInstance>( result.NewInstance );

        Assert.Equal( ItemConsumptionMode.ReduceStack, split.ConsumptionMode );
        Assert.Equal( 1, split.ConsumptionQuantity );

        ItemInstance incompatible = CreateStack(
            2,
            ItemConsumptionMode.ConsumeAmmunition,
            1 );
        Assert.Throws<InventoryException>( () => split.MergeFrom(
            incompatible,
            0,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 2 ) ) );
    }

    [Fact]
    public void FactoriesRejectConsumptionModeThatDoesNotMatchStackability()
    {
        Assert.Throws<InventoryException>( () => ItemInstance.CreateConsumable(
            Guid.NewGuid(),
            17,
            23,
            ItemConsumptionMode.ReduceStack,
            1,
            CreateContainer(),
            null,
            _createdAtUtc ) );
        Assert.Throws<InventoryException>( () => ItemInstance.CreateConsumableStack(
            Guid.NewGuid(),
            17,
            23,
            2,
            ItemConsumptionMode.DestroyInstance,
            1,
            CreateContainer(),
            null,
            _createdAtUtc ) );
    }

    [Fact]
    public void RegularItemRejectsConsumption()
    {
        ItemInstance item = ItemInstance.Create(
            Guid.NewGuid(),
            17,
            23,
            CreateContainer(),
            null,
            _createdAtUtc );

        Assert.Throws<InventoryException>( () => item.Consume(
            1,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) ) );
    }

    private static ItemInstance CreateStack(
        int initialQuantity,
        ItemConsumptionMode mode,
        int consumptionQuantity )
    {
        return ItemInstance.CreateConsumableStack(
            Guid.NewGuid(),
            17,
            23,
            initialQuantity,
            mode,
            consumptionQuantity,
            CreateContainer(),
            null,
            _createdAtUtc );
    }

    private static InventoryContainer CreateContainer()
    {
        return InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            17,
            InventoryContainerOwnerKind.Character,
            31,
            _createdAtUtc );
    }
}
