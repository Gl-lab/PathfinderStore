using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Operations;

namespace Pathfinder.Inventory.Domain.Tests;

public sealed class ItemDurabilityTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 27, 19, 0, 0, TimeSpan.Zero );

    [Fact]
    public void DamageAppliesHardnessAndCrossesBrokenThreshold()
    {
        ItemInstance item = CreateItem();

        item.ApplyDamage(
            12,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) );

        Assert.Equal( 13, item.CurrentHitPoints );
        Assert.False( item.IsBroken );

        item.ApplyDamage(
            8,
            1,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 2 ) );

        Assert.Equal( 10, item.CurrentHitPoints );
        Assert.True( item.IsBroken );
        Assert.False( item.IsDestroyed );
    }

    [Fact]
    public void DamageCannotReduceHitPointsBelowZeroAndDestroysInstance()
    {
        ItemInstance item = CreateItem();

        item.ApplyDamage(
            100,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) );

        Assert.Equal( 0, item.CurrentHitPoints );
        Assert.True( item.IsDestroyed );
        Assert.True( item.IsDepleted );
        Assert.Throws<InventoryException>( () => item.Repair(
            1,
            1,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 2 ) ) );
    }

    [Fact]
    public void RepairRestoresBrokenItemWithoutExceedingMaximum()
    {
        ItemInstance item = CreateItem();
        item.ApplyDamage(
            15,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) );

        Assert.True( item.IsBroken );
        Assert.True( item.Repair(
            6,
            1,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 2 ) ) );
        Assert.Equal( 16, item.CurrentHitPoints );
        Assert.False( item.IsBroken );
        Assert.Throws<InventoryException>( () => item.Repair(
            5,
            2,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 3 ) ) );
    }

    [Fact]
    public void DamageReplayIsIdempotentAndRejectsDifferentDamage()
    {
        ItemInstance item = CreateItem();
        Guid operationId = Guid.NewGuid();

        Assert.True( item.ApplyDamage(
            6,
            0,
            operationId,
            _createdAtUtc.AddMinutes( 1 ) ) );
        Assert.False( item.ApplyDamage(
            6,
            0,
            operationId,
            _createdAtUtc.AddMinutes( 1 ) ) );
        Assert.Throws<InventoryException>( () => item.ApplyDamage(
            7,
            0,
            operationId,
            _createdAtUtc.AddMinutes( 1 ) ) );
        Assert.Equal( 19, item.CurrentHitPoints );
        Assert.Equal( 1, item.Version );
    }

    [Fact]
    public void ConcurrentDamageWithStaleVersionIsRejected()
    {
        ItemInstance item = CreateItem();
        item.ApplyDamage(
            6,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) );

        Assert.Throws<InventoryException>( () => item.ApplyDamage(
            6,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) ) );
    }

    [Fact]
    public void RegularItemRejectsDurabilityChanges()
    {
        ItemInstance item = ItemInstance.Create(
            Guid.NewGuid(),
            17,
            23,
            CreateContainer(),
            null,
            _createdAtUtc );

        Assert.Throws<InventoryException>( () => item.ApplyDamage(
            1,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) ) );
    }

    private static ItemInstance CreateItem()
    {
        return ItemInstance.CreateDurable(
            Guid.NewGuid(),
            17,
            23,
            5,
            20,
            10,
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
