using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Inventory.Domain.Items;

public sealed class ItemInstance : Entity, IAggregateRoot
{
    public const int CustomNameMaxLength = 200;

    private ItemInstance()
    {
    }

    public Guid InstanceKey { get; private set; }
    public int CampaignId { get; private set; }
    public int ItemConfigurationId { get; private set; }
    public string? CustomName { get; private set; }
    public bool IsStackable { get; private set; }
    public int Quantity { get; private set; }
    public bool IsDepleted => Quantity == 0;
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static ItemInstance Create(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        string? customName,
        DateTimeOffset createdAtUtc )
    {
        return CreateCore(
            instanceKey,
            campaignId,
            itemConfigurationId,
            customName,
            false,
            1,
            createdAtUtc );
    }

    public static ItemInstance CreateStack(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        int quantity,
        string? customName,
        DateTimeOffset createdAtUtc )
    {
        if ( quantity <= 0 )
        {
            throw new InventoryException( "Item stack quantity must be greater than zero." );
        }

        return CreateCore(
            instanceKey,
            campaignId,
            itemConfigurationId,
            customName,
            true,
            quantity,
            createdAtUtc );
    }

    public ItemInstance Split(
        Guid newInstanceKey,
        int splitQuantity,
        DateTimeOffset createdAtUtc )
    {
        EnsureActiveStack();
        if ( ( splitQuantity <= 0 ) || ( splitQuantity >= Quantity ) )
        {
            throw new InventoryException(
                "Split quantity must be greater than zero and less than the current quantity." );
        }

        if ( newInstanceKey == InstanceKey )
        {
            throw new InventoryException(
                "A split item stack must have a different instance key." );
        }

        if ( createdAtUtc.Offset != TimeSpan.Zero )
        {
            throw new InventoryException( "Split timestamp must use UTC." );
        }

        if ( createdAtUtc < CreatedAtUtc )
        {
            throw new InventoryException(
                "Split timestamp cannot precede item instance creation." );
        }

        ItemInstance split = CreateStack(
            newInstanceKey,
            CampaignId,
            ItemConfigurationId,
            splitQuantity,
            CustomName,
            createdAtUtc );
        Quantity -= splitQuantity;
        return split;
    }

    public void MergeFrom( ItemInstance source )
    {
        ArgumentNullException.ThrowIfNull( source );
        EnsureActiveStack();
        source.EnsureActiveStack();

        if ( InstanceKey == source.InstanceKey )
        {
            throw new InventoryException( "An item stack cannot be merged with itself." );
        }

        if ( ( CampaignId != source.CampaignId ) ||
             ( ItemConfigurationId != source.ItemConfigurationId ) ||
             !String.Equals( CustomName, source.CustomName, StringComparison.Ordinal ) )
        {
            throw new InventoryException(
                "Item stacks must have the same campaign, configuration, and custom name." );
        }

        if ( Quantity > ( Int32.MaxValue - source.Quantity ) )
        {
            throw new InventoryException( "Merged item stack quantity is too large." );
        }

        Quantity += source.Quantity;
        source.Quantity = 0;
    }

    private static ItemInstance CreateCore(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        string? customName,
        bool isStackable,
        int quantity,
        DateTimeOffset createdAtUtc )
    {
        if ( instanceKey == Guid.Empty )
        {
            throw new InventoryException( "Item instance key cannot be empty." );
        }

        if ( campaignId <= 0 )
        {
            throw new InventoryException( "Campaign id must be greater than zero." );
        }

        if ( itemConfigurationId <= 0 )
        {
            throw new InventoryException( "Item configuration id must be greater than zero." );
        }

        if ( createdAtUtc.Offset != TimeSpan.Zero )
        {
            throw new InventoryException( "Item instance creation timestamp must use UTC." );
        }

        string? normalizedCustomName = NormalizeCustomName( customName );
        return new ItemInstance
        {
            InstanceKey = instanceKey,
            CampaignId = campaignId,
            ItemConfigurationId = itemConfigurationId,
            CustomName = normalizedCustomName,
            IsStackable = isStackable,
            Quantity = quantity,
            CreatedAtUtc = createdAtUtc,
        };
    }

    private void EnsureActiveStack()
    {
        if ( !IsStackable )
        {
            throw new InventoryException( "This item instance cannot be stacked." );
        }

        if ( IsDepleted )
        {
            throw new InventoryException( "A depleted item stack cannot be changed." );
        }
    }

    private static string? NormalizeCustomName( string? customName )
    {
        if ( String.IsNullOrWhiteSpace( customName ) )
        {
            return null;
        }

        string normalizedCustomName = customName.Trim();
        if ( normalizedCustomName.Length > CustomNameMaxLength )
        {
            throw new InventoryException(
                $"Item instance custom name cannot exceed {CustomNameMaxLength} characters." );
        }

        return normalizedCustomName;
    }
}
