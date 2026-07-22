using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Movements;
using Pathfinder.Inventory.Domain.Operations;

namespace Pathfinder.Inventory.Domain.Tests;

public sealed class ItemOperationTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 16, 0, 0, TimeSpan.Zero );

    [Fact]
    public void NewInstanceStartsAtVersionZero()
    {
        ItemInstance instance = CreateInstance( CreateContainer() );

        Assert.Equal( 0, instance.Version );
        Assert.Empty( instance.Operations );
    }

    [Fact]
    public void MoveIncrementsVersionAndReplayReturnsOriginalResult()
    {
        InventoryContainer source = CreateContainer();
        InventoryContainer destination = CreateContainer();
        ItemInstance instance = CreateInstance( source );
        Guid operationId = Guid.NewGuid();

        InventoryMovement applied = instance.MoveTo(
            destination,
            "transfer",
            0,
            operationId,
            "user:31",
            _createdAtUtc );
        InventoryMovement replay = instance.MoveTo(
            destination,
            "transfer",
            0,
            operationId,
            "user:31",
            _createdAtUtc );

        Assert.Same( applied, replay );
        Assert.Equal( 1, instance.Version );
        Assert.Single( instance.Movements );
        Assert.Single( instance.Operations );
        Assert.Equal( 1, instance.Operations[ 0 ].VersionAfter );
    }

    [Fact]
    public void NewOperationRejectsStaleExpectedVersion()
    {
        ItemInstance instance = CreateInstance( CreateContainer() );
        instance.MoveTo(
            CreateContainer(),
            "first",
            0,
            Guid.NewGuid(),
            "user:31",
            _createdAtUtc );

        Assert.Throws<InventoryException>( () => instance.MoveTo(
            CreateContainer(),
            "second",
            0,
            Guid.NewGuid(),
            "user:31",
            _createdAtUtc ) );
        Assert.Equal( 1, instance.Version );
    }

    [Fact]
    public void ReusedMovementOperationIdRejectsDifferentPayload()
    {
        ItemInstance instance = CreateInstance( CreateContainer() );
        Guid operationId = Guid.NewGuid();
        instance.MoveTo(
            CreateContainer(),
            "first",
            0,
            operationId,
            "user:31",
            _createdAtUtc );

        Assert.Throws<InventoryException>( () => instance.MoveTo(
            CreateContainer(),
            "second",
            0,
            operationId,
            "user:31",
            _createdAtUtc ) );
    }

    [Fact]
    public void SplitReplayDoesNotDecreaseQuantityTwice()
    {
        ItemInstance source = ItemInstance.CreateStack(
            Guid.NewGuid(),
            17,
            23,
            5,
            CreateContainer(),
            null,
            _createdAtUtc );
        Guid newInstanceKey = Guid.NewGuid();
        Guid operationId = Guid.NewGuid();

        ItemSplitResult applied = source.Split(
            newInstanceKey,
            2,
            0,
            operationId,
            _createdAtUtc );
        ItemSplitResult replay = source.Split(
            newInstanceKey,
            2,
            0,
            operationId,
            _createdAtUtc );

        Assert.False( applied.IsReplay );
        Assert.NotNull( applied.NewInstance );
        Assert.True( replay.IsReplay );
        Assert.Null( replay.NewInstance );
        Assert.Equal( newInstanceKey, replay.NewInstanceKey );
        Assert.Equal( 3, source.Quantity );
        Assert.Equal( 1, source.Version );
        Assert.Single( source.Operations );
    }

    [Fact]
    public void ReusedSplitOperationIdRejectsDifferentQuantity()
    {
        ItemInstance source = ItemInstance.CreateStack(
            Guid.NewGuid(),
            17,
            23,
            5,
            CreateContainer(),
            null,
            _createdAtUtc );
        Guid operationId = Guid.NewGuid();
        Guid newInstanceKey = Guid.NewGuid();
        source.Split( newInstanceKey, 2, 0, operationId, _createdAtUtc );

        Assert.Throws<InventoryException>( () => source.Split(
            newInstanceKey,
            1,
            0,
            operationId,
            _createdAtUtc ) );
        Assert.Equal( 3, source.Quantity );
    }

    [Fact]
    public void EmptyOperationIdIsRejectedBeforeMutation()
    {
        ItemInstance instance = CreateInstance( CreateContainer() );

        Assert.Throws<InventoryException>( () => instance.MoveTo(
            CreateContainer(),
            "transfer",
            0,
            Guid.Empty,
            "user:31",
            _createdAtUtc ) );
        Assert.Equal( 0, instance.Version );
        Assert.Empty( instance.Movements );
    }

    private static ItemInstance CreateInstance( InventoryContainer container )
    {
        return ItemInstance.Create(
            Guid.NewGuid(),
            17,
            23,
            container,
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
