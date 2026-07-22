using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Transfers;

namespace Pathfinder.Inventory.Domain.Tests;

public sealed class PartyGiftTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new( 2026, 7, 22, 18, 0, 0, TimeSpan.Zero );

    [Fact]
    public void CreateRequiresDifferentCharacters()
    {
        Assert.Throws<InventoryException>( () => PartyGift.Create(
            Guid.NewGuid(),
            17,
            23,
            31,
            31,
            Guid.NewGuid(),
            0,
            _createdAtUtc,
            _createdAtUtc.AddDays( 1 ) ) );
    }

    [Fact]
    public void AcceptRejectsChangedItemVersion()
    {
        PartyGift gift = CreateGift();

        InventoryException exception = Assert.Throws<InventoryException>( () => gift.Accept(
            1,
            Guid.NewGuid(),
            _createdAtUtc.AddHours( 1 ) ) );

        Assert.Contains( "changed", exception.Message, StringComparison.OrdinalIgnoreCase );
        Assert.Equal( PartyGiftStatus.Pending, gift.Status );
    }

    [Fact]
    public void AcceptIsIdempotentForSameOperation()
    {
        PartyGift gift = CreateGift();
        Guid operationId = Guid.NewGuid();

        bool applied = gift.Accept( 0, operationId, _createdAtUtc.AddHours( 1 ) );
        bool replay = gift.Accept( 0, operationId, _createdAtUtc.AddHours( 1 ) );

        Assert.True( applied );
        Assert.False( replay );
        Assert.Equal( PartyGiftStatus.Accepted, gift.Status );
        Assert.Equal( operationId, gift.AcceptanceOperationId );
    }

    [Fact]
    public void AcceptRejectsExpiredGift()
    {
        PartyGift gift = CreateGift();

        Assert.Throws<InventoryException>( () => gift.Accept(
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddDays( 2 ) ) );
    }

    private static PartyGift CreateGift()
    {
        return PartyGift.Create(
            Guid.NewGuid(),
            17,
            23,
            31,
            32,
            Guid.NewGuid(),
            0,
            _createdAtUtc,
            _createdAtUtc.AddDays( 1 ) );
    }
}
