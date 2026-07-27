using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;

namespace Pathfinder.Inventory.Domain.Tests;

public sealed class ItemRuneTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 27, 20, 0, 0, TimeSpan.Zero );

    [Fact]
    public void RuneAttachesAndTransfersBetweenCompatibleItems()
    {
        ItemInstance rune = CreateRune( ItemRuneTargetKind.Weapon );
        ItemInstance source = CreateTarget( ItemRuneTargetKind.Weapon );
        ItemInstance destination = CreateTarget( ItemRuneTargetKind.Weapon );
        Guid attachOperationId = Guid.NewGuid();

        Assert.True( rune.AttachRuneTo(
            source,
            0,
            0,
            attachOperationId,
            _createdAtUtc.AddMinutes( 1 ) ) );
        Assert.False( rune.AttachRuneTo(
            source,
            0,
            0,
            attachOperationId,
            _createdAtUtc.AddMinutes( 1 ) ) );
        Assert.Equal( source.InstanceKey, rune.AttachedToInstanceKey );

        Assert.True( rune.TransferRuneTo(
            source,
            destination,
            1,
            1,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 2 ) ) );
        Assert.Equal( destination.InstanceKey, rune.AttachedToInstanceKey );
        Assert.Equal( 2, rune.Version );
        Assert.Equal( 2, source.Version );
        Assert.Equal( 1, destination.Version );
    }

    [Fact]
    public void RuneRejectsIncompatibleTarget()
    {
        ItemInstance rune = CreateRune( ItemRuneTargetKind.Weapon );
        ItemInstance armor = CreateTarget( ItemRuneTargetKind.Armor );

        Assert.Throws<InventoryException>( () => rune.AttachRuneTo(
            armor,
            0,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) ) );
        Assert.Null( rune.AttachedToInstanceKey );
    }

    [Fact]
    public void RuneTransferRejectsStaleTargetVersion()
    {
        ItemInstance rune = CreateRune( ItemRuneTargetKind.Weapon );
        ItemInstance source = CreateTarget( ItemRuneTargetKind.Weapon );
        ItemInstance destination = CreateTarget( ItemRuneTargetKind.Weapon );
        rune.AttachRuneTo(
            source,
            0,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) );

        Assert.Throws<InventoryException>( () => rune.TransferRuneTo(
            source,
            destination,
            1,
            0,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 2 ) ) );
        Assert.Equal( source.InstanceKey, rune.AttachedToInstanceKey );
    }

    private static ItemInstance CreateRune( ItemRuneTargetKind targetKind )
    {
        return ItemInstance.CreateAttachableRune(
            Guid.NewGuid(),
            17,
            23,
            "rune.potency",
            targetKind,
            CreateContainer(),
            null,
            _createdAtUtc );
    }

    private static ItemInstance CreateTarget( ItemRuneTargetKind targetKind )
    {
        return ItemInstance.CreateRuneCompatible(
            Guid.NewGuid(),
            17,
            24,
            targetKind,
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
