using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Movements;

namespace Pathfinder.Inventory.Domain.Tests;

public sealed class InventoryMovementTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 15, 0, 0, TimeSpan.Zero );

    [Fact]
    public void CreateRequiresInitialContainerInSameCampaign()
    {
        InventoryContainer foreignContainer = CreateContainer( 18 );

        Assert.Throws<InventoryException>( () => ItemInstance.Create(
            Guid.NewGuid(),
            17,
            23,
            foreignContainer,
            null,
            _createdAtUtc ) );
    }

    [Fact]
    public void MoveChangesCurrentLocationAndAppendsHistory()
    {
        InventoryContainer source = CreateContainer( 17 );
        InventoryContainer destination = CreateContainer( 17 );
        ItemInstance instance = CreateInstance( source );
        Guid operationId = new Guid( "646397f3-74c4-41f1-8900-17fa210038bb" );
        DateTimeOffset occurredAtUtc = _createdAtUtc.AddMinutes( 1 );

        InventoryMovement movement = instance.MoveTo(
            destination,
            "  equipped  ",
            0,
            operationId,
            "  user:31  ",
            occurredAtUtc );

        Assert.Equal( destination.ContainerKey, instance.CurrentContainerKey );
        Assert.Single( instance.Movements );
        Assert.Same( movement, instance.Movements[ 0 ] );
        Assert.Equal( instance.InstanceKey, movement.ItemInstanceKey );
        Assert.Equal( source.ContainerKey, movement.FromContainerKey );
        Assert.Equal( destination.ContainerKey, movement.ToContainerKey );
        Assert.Equal( 1, movement.Quantity );
        Assert.Equal( "equipped", movement.Reason );
        Assert.Equal( operationId, movement.OperationId );
        Assert.Equal( "user:31", movement.PerformedBy );
        Assert.Equal( occurredAtUtc, movement.OccurredAtUtc );
    }

    [Fact]
    public void MoveRecordsWholeStackQuantity()
    {
        InventoryContainer source = CreateContainer( 17 );
        InventoryContainer destination = CreateContainer( 17 );
        ItemInstance stack = ItemInstance.CreateStack(
            Guid.NewGuid(),
            17,
            23,
            7,
            source,
            null,
            _createdAtUtc );

        InventoryMovement movement = stack.MoveTo(
            destination,
            "transfer",
            0,
            Guid.NewGuid(),
            "user:31",
            _createdAtUtc );

        Assert.Equal( 7, movement.Quantity );
    }

    [Fact]
    public void MoveRejectsContainerInAnotherCampaign()
    {
        ItemInstance instance = CreateInstance( CreateContainer( 17 ) );
        InventoryContainer destination = CreateContainer( 18 );

        Assert.Throws<InventoryException>( () => instance.MoveTo(
            destination,
            "transfer",
            0,
            Guid.NewGuid(),
            "user:31",
            _createdAtUtc ) );
        Assert.Empty( instance.Movements );
    }

    [Fact]
    public void MoveRejectsCurrentContainer()
    {
        InventoryContainer container = CreateContainer( 17 );
        ItemInstance instance = CreateInstance( container );

        Assert.Throws<InventoryException>( () => instance.MoveTo(
            container,
            "transfer",
            0,
            Guid.NewGuid(),
            "user:31",
            _createdAtUtc ) );
    }

    [Fact]
    public void MoveRejectsTimestampBeforeHistory()
    {
        InventoryContainer first = CreateContainer( 17 );
        InventoryContainer second = CreateContainer( 17 );
        InventoryContainer third = CreateContainer( 17 );
        ItemInstance instance = CreateInstance( first );
        instance.MoveTo(
            second,
            "first",
            0,
            Guid.NewGuid(),
            "user:31",
            _createdAtUtc.AddMinutes( 2 ) );

        Assert.Throws<InventoryException>( () => instance.MoveTo(
            third,
            "second",
            1,
            Guid.NewGuid(),
            "user:31",
            _createdAtUtc.AddMinutes( 1 ) ) );
        Assert.Equal( second.ContainerKey, instance.CurrentContainerKey );
        Assert.Single( instance.Movements );
    }

    [Theory]
    [InlineData( "", "user:31" )]
    [InlineData( "transfer", "" )]
    public void MoveRejectsMissingAuditText(
        string reason,
        string performedBy )
    {
        ItemInstance instance = CreateInstance( CreateContainer( 17 ) );

        Assert.Throws<InventoryException>( () => instance.MoveTo(
            CreateContainer( 17 ),
            reason,
            0,
            Guid.NewGuid(),
            performedBy,
            _createdAtUtc ) );
    }

    [Fact]
    public void MovementHistoryCannotBeChangedThroughPublicView()
    {
        ItemInstance instance = CreateInstance( CreateContainer( 17 ) );
        InventoryMovement movement = instance.MoveTo(
            CreateContainer( 17 ),
            "transfer",
            0,
            Guid.NewGuid(),
            "user:31",
            _createdAtUtc );
        ICollection<InventoryMovement> movements =
            Assert.IsAssignableFrom<ICollection<InventoryMovement>>( instance.Movements );

        Assert.Throws<NotSupportedException>( () => movements.Add( movement ) );
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
