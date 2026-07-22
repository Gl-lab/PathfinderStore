using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Transfers;

namespace Pathfinder.Inventory.Domain.Tests;

public sealed class PartyExchangeTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new( 2026, 7, 22, 19, 0, 0, TimeSpan.Zero );

    [Fact]
    public void CreateRequiresLinesFromBothCharacters()
    {
        PartyExchangeLineRequest[] lines =
        [
            new PartyExchangeLineRequest( 31, Guid.NewGuid(), 0 ),
            new PartyExchangeLineRequest( 31, Guid.NewGuid(), 0 ),
        ];

        Assert.Throws<InventoryException>( () => PartyExchange.Create(
            Guid.NewGuid(),
            17,
            41,
            31,
            32,
            lines,
            _createdAtUtc,
            _createdAtUtc.AddDays( 1 ) ) );
    }

    [Fact]
    public void ReserveChangesVersionAndRejectsSecondReservation()
    {
        InventoryContainer container = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            17,
            InventoryContainerOwnerKind.Character,
            31,
            _createdAtUtc );
        ItemInstance item = ItemInstance.Create(
            Guid.NewGuid(),
            17,
            23,
            container,
            null,
            _createdAtUtc );
        Guid reservationKey = Guid.NewGuid();

        bool applied = item.Reserve(
            reservationKey,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) );

        Assert.True( applied );
        Assert.Equal( reservationKey, item.ReservationKey );
        Assert.Equal( 1, item.Version );
        Assert.Throws<InventoryException>( () => item.Reserve(
            Guid.NewGuid(),
            1,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 2 ) ) );
    }
}
