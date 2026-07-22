using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;

namespace Pathfinder.Inventory.Domain.Tests;

public sealed class ItemInstanceTests
{
    private static readonly Guid _instanceKey = new Guid( "4b1e17e8-3320-4db6-95c0-cb5ba5783523" );
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 12, 0, 0, TimeSpan.Zero );

    [Fact]
    public void CreateStoresGlobalIdentityCampaignAndExactConfiguration()
    {
        ItemInstance instance = ItemInstance.Create(
            _instanceKey,
            17,
            23,
            CreateContainer( 17 ),
            "  Dawnbringer  ",
            _createdAtUtc );

        Assert.Equal( _instanceKey, instance.InstanceKey );
        Assert.Equal( 17, instance.CampaignId );
        Assert.Equal( 23, instance.ItemConfigurationId );
        Assert.Equal( "Dawnbringer", instance.CustomName );
        Assert.Equal( _createdAtUtc, instance.CreatedAtUtc );
    }

    [Fact]
    public void CreateAllowsDifferentInstancesOfSameConfiguration()
    {
        ItemInstance first = ItemInstance.Create(
            _instanceKey,
            17,
            23,
            CreateContainer( 17 ),
            null,
            _createdAtUtc );
        ItemInstance second = ItemInstance.Create(
            new Guid( "83687638-ef14-4f8a-9415-e62612405ab5" ),
            17,
            23,
            CreateContainer( 17 ),
            null,
            _createdAtUtc );

        Assert.NotEqual( first.InstanceKey, second.InstanceKey );
        Assert.Equal( first.ItemConfigurationId, second.ItemConfigurationId );
    }

    [Theory]
    [InlineData( 0, 1 )]
    [InlineData( -1, 1 )]
    [InlineData( 1, 0 )]
    [InlineData( 1, -1 )]
    public void CreateRejectsInvalidCampaignOrConfiguration(
        int campaignId,
        int itemConfigurationId )
    {
        Assert.Throws<InventoryException>( () => ItemInstance.Create(
            _instanceKey,
            campaignId,
            itemConfigurationId,
            CreateContainer( campaignId > 0 ? campaignId : 17 ),
            null,
            _createdAtUtc ) );
    }

    [Fact]
    public void CreateRejectsEmptyGlobalIdentity()
    {
        Assert.Throws<InventoryException>( () => ItemInstance.Create(
            Guid.Empty,
            17,
            23,
            CreateContainer( 17 ),
            null,
            _createdAtUtc ) );
    }

    [Fact]
    public void CreateRejectsNonUtcTimestamp()
    {
        DateTimeOffset nonUtc = new DateTimeOffset( 2026, 7, 22, 15, 0, 0, TimeSpan.FromHours( 3 ) );

        Assert.Throws<InventoryException>( () => ItemInstance.Create(
            _instanceKey,
            17,
            23,
            CreateContainer( 17 ),
            null,
            nonUtc ) );
    }

    [Fact]
    public void CreateNormalizesEmptyCustomNameToNull()
    {
        ItemInstance instance = ItemInstance.Create(
            _instanceKey,
            17,
            23,
            CreateContainer( 17 ),
            "   ",
            _createdAtUtc );

        Assert.Null( instance.CustomName );
    }

    [Fact]
    public void CreateRejectsTooLongCustomName()
    {
        string customName = new string( 'a', ItemInstance.CustomNameMaxLength + 1 );

        Assert.Throws<InventoryException>( () => ItemInstance.Create(
            _instanceKey,
            17,
            23,
            CreateContainer( 17 ),
            customName,
            _createdAtUtc ) );
    }

    private static InventoryContainer CreateContainer( int campaignId )
    {
        return InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            campaignId,
            InventoryContainerOwnerKind.Character,
            31,
            _createdAtUtc );
    }
}
