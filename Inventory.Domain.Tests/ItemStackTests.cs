using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;

namespace Pathfinder.Inventory.Domain.Tests;

public sealed class ItemStackTests
{
    private static readonly Guid _instanceKey = new Guid( "36070d94-3f71-414b-a42d-e695acb3abb6" );
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 14, 0, 0, TimeSpan.Zero );

    [Fact]
    public void CreateStackStoresQuantity()
    {
        ItemInstance stack = CreateStack( _instanceKey, 17, 23, 5, null );

        Assert.True( stack.IsStackable );
        Assert.Equal( 5, stack.Quantity );
        Assert.False( stack.IsDepleted );
    }

    [Fact]
    public void RegularInstanceCannotBeSplit()
    {
        ItemInstance instance = ItemInstance.Create(
            _instanceKey,
            17,
            23,
            CreateContainer( 17 ),
            null,
            _createdAtUtc );

        Assert.Throws<InventoryException>( () => instance.Split(
            Guid.NewGuid(),
            1,
            _createdAtUtc ) );
    }

    [Fact]
    public void SplitMovesQuantityIntoNewCompatibleStack()
    {
        ItemInstance source = CreateStack( _instanceKey, 17, 23, 5, "Arrows" );
        Guid splitKey = new Guid( "4812dbff-de74-468f-8294-b952c28aed33" );

        ItemInstance split = source.Split( splitKey, 2, _createdAtUtc.AddMinutes( 1 ) );

        Assert.Equal( 3, source.Quantity );
        Assert.Equal( splitKey, split.InstanceKey );
        Assert.Equal( 2, split.Quantity );
        Assert.Equal( source.CampaignId, split.CampaignId );
        Assert.Equal( source.ItemConfigurationId, split.ItemConfigurationId );
        Assert.Equal( source.CurrentContainerKey, split.CurrentContainerKey );
        Assert.Equal( source.CustomName, split.CustomName );
    }

    [Theory]
    [InlineData( 0 )]
    [InlineData( -1 )]
    [InlineData( 5 )]
    [InlineData( 6 )]
    public void SplitRejectsInvalidQuantity( int splitQuantity )
    {
        ItemInstance source = CreateStack( _instanceKey, 17, 23, 5, null );

        Assert.Throws<InventoryException>( () => source.Split(
            Guid.NewGuid(),
            splitQuantity,
            _createdAtUtc ) );
        Assert.Equal( 5, source.Quantity );
    }

    [Fact]
    public void SplitRejectsSourceInstanceKey()
    {
        ItemInstance source = CreateStack( _instanceKey, 17, 23, 5, null );

        Assert.Throws<InventoryException>( () => source.Split(
            _instanceKey,
            2,
            _createdAtUtc ) );
        Assert.Equal( 5, source.Quantity );
    }

    [Fact]
    public void MergeMovesAllQuantityAndDepletesSource()
    {
        InventoryContainer container = CreateContainer( 17 );
        ItemInstance target = CreateStack( _instanceKey, 17, 23, 5, null, container );
        ItemInstance source = CreateStack( Guid.NewGuid(), 17, 23, 2, null, container );

        target.MergeFrom( source );

        Assert.Equal( 7, target.Quantity );
        Assert.Equal( 0, source.Quantity );
        Assert.True( source.IsDepleted );
    }

    [Fact]
    public void MergeRejectsSecondUseOfDepletedSource()
    {
        InventoryContainer container = CreateContainer( 17 );
        ItemInstance first = CreateStack( _instanceKey, 17, 23, 5, null, container );
        ItemInstance second = CreateStack( Guid.NewGuid(), 17, 23, 2, null, container );
        first.MergeFrom( second );

        Assert.Throws<InventoryException>( () => first.MergeFrom( second ) );
        Assert.Equal( 7, first.Quantity );
    }

    [Theory]
    [InlineData( 18, 23, null )]
    [InlineData( 17, 24, null )]
    [InlineData( 17, 23, "Named" )]
    public void MergeRejectsIncompatibleStack(
        int campaignId,
        int itemConfigurationId,
        string? customName )
    {
        InventoryContainer targetContainer = CreateContainer( 17 );
        InventoryContainer sourceContainer = campaignId == 17
            ? targetContainer
            : CreateContainer( campaignId );
        ItemInstance target = CreateStack( _instanceKey, 17, 23, 5, null, targetContainer );
        ItemInstance source = CreateStack(
            Guid.NewGuid(),
            campaignId,
            itemConfigurationId,
            2,
            customName,
            sourceContainer );

        Assert.Throws<InventoryException>( () => target.MergeFrom( source ) );
        Assert.Equal( 5, target.Quantity );
        Assert.Equal( 2, source.Quantity );
    }

    [Fact]
    public void MergeRejectsNonStackableInstance()
    {
        InventoryContainer container = CreateContainer( 17 );
        ItemInstance target = CreateStack( _instanceKey, 17, 23, 5, null, container );
        ItemInstance source = ItemInstance.Create(
            Guid.NewGuid(),
            17,
            23,
            container,
            null,
            _createdAtUtc );

        Assert.Throws<InventoryException>( () => target.MergeFrom( source ) );
    }

    [Fact]
    public void MergeRejectsQuantityOverflow()
    {
        InventoryContainer container = CreateContainer( 17 );
        ItemInstance target = CreateStack(
            _instanceKey,
            17,
            23,
            Int32.MaxValue,
            null,
            container );
        ItemInstance source = CreateStack( Guid.NewGuid(), 17, 23, 1, null, container );

        Assert.Throws<InventoryException>( () => target.MergeFrom( source ) );
        Assert.Equal( Int32.MaxValue, target.Quantity );
        Assert.Equal( 1, source.Quantity );
    }

    private static ItemInstance CreateStack(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        int quantity,
        string? customName,
        InventoryContainer? container = null )
    {
        return ItemInstance.CreateStack(
            instanceKey,
            campaignId,
            itemConfigurationId,
            quantity,
            container ?? CreateContainer( campaignId ),
            customName,
            _createdAtUtc );
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
