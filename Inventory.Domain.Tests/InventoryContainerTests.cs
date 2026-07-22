using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;

namespace Pathfinder.Inventory.Domain.Tests;

public sealed class InventoryContainerTests
{
    private static readonly Guid _containerKey = new Guid( "ae28472b-0158-40d6-9e50-92d2122e7e65" );
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 13, 0, 0, TimeSpan.Zero );

    [Theory]
    [InlineData( InventoryContainerOwnerKind.Character )]
    [InlineData( InventoryContainerOwnerKind.Party )]
    [InlineData( InventoryContainerOwnerKind.Shop )]
    [InlineData( InventoryContainerOwnerKind.World )]
    public void CreateRootStoresSupportedOwner(
        InventoryContainerOwnerKind ownerKind )
    {
        InventoryContainer container = InventoryContainer.CreateRoot(
            _containerKey,
            17,
            ownerKind,
            23,
            _createdAtUtc );

        Assert.Equal( _containerKey, container.ContainerKey );
        Assert.Equal( 17, container.CampaignId );
        Assert.Equal( ownerKind, container.OwnerKind );
        Assert.Equal( 23, container.OwnerId );
        Assert.Equal( _createdAtUtc, container.CreatedAtUtc );
    }

    [Theory]
    [InlineData( 0, 1 )]
    [InlineData( -1, 1 )]
    [InlineData( 1, 0 )]
    [InlineData( 1, -1 )]
    public void CreateRootRejectsInvalidCampaignOrOwner(
        int campaignId,
        int ownerId )
    {
        Assert.Throws<InventoryException>( () => InventoryContainer.CreateRoot(
            _containerKey,
            campaignId,
            InventoryContainerOwnerKind.Character,
            ownerId,
            _createdAtUtc ) );
    }

    [Fact]
    public void CreateRootRejectsEmptyGlobalIdentity()
    {
        Assert.Throws<InventoryException>( () => InventoryContainer.CreateRoot(
            Guid.Empty,
            17,
            InventoryContainerOwnerKind.Character,
            23,
            _createdAtUtc ) );
    }

    [Fact]
    public void CreateRootRejectsUnknownOwnerKind()
    {
        Assert.Throws<InventoryException>( () => InventoryContainer.CreateRoot(
            _containerKey,
            17,
            ( InventoryContainerOwnerKind )999,
            23,
            _createdAtUtc ) );
    }

    [Fact]
    public void CreateRootRejectsNonUtcTimestamp()
    {
        DateTimeOffset nonUtc = new DateTimeOffset( 2026, 7, 22, 16, 0, 0, TimeSpan.FromHours( 3 ) );

        Assert.Throws<InventoryException>( () => InventoryContainer.CreateRoot(
            _containerKey,
            17,
            InventoryContainerOwnerKind.Character,
            23,
            nonUtc ) );
    }
}
