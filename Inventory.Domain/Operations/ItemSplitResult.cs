using Pathfinder.Inventory.Domain.Items;

namespace Pathfinder.Inventory.Domain.Operations;

public sealed class ItemSplitResult
{
    private ItemSplitResult(
        bool isReplay,
        Guid newInstanceKey,
        ItemInstance? newInstance )
    {
        IsReplay = isReplay;
        NewInstanceKey = newInstanceKey;
        NewInstance = newInstance;
    }

    public bool IsReplay { get; }
    public Guid NewInstanceKey { get; }
    public ItemInstance? NewInstance { get; }

    internal static ItemSplitResult Applied( ItemInstance newInstance )
    {
        return new ItemSplitResult( false, newInstance.InstanceKey, newInstance );
    }

    internal static ItemSplitResult Replay( Guid newInstanceKey )
    {
        return new ItemSplitResult( true, newInstanceKey, null );
    }
}
