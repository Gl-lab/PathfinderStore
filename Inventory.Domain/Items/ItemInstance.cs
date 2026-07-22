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
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static ItemInstance Create(
        Guid instanceKey,
        int campaignId,
        int itemConfigurationId,
        string? customName,
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
            CreatedAtUtc = createdAtUtc,
        };
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
