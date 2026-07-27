using Pathfinder.Inventory.Domain.Items;

namespace Pathfinder.Inventory.Application.Lifecycle;

public sealed record ItemLifecycleDto(
    Guid InstanceKey,
    int Version,
    int Quantity,
    bool IsDepleted,
    int? MaximumCharges,
    int? CurrentCharges,
    int? Hardness,
    int? MaximumHitPoints,
    int? CurrentHitPoints,
    int? BrokenThreshold,
    bool IsBroken,
    bool IsDestroyed,
    string? AttachableRuneCode,
    ItemRuneTargetKind? RuneTargetKind,
    Guid? AttachedToInstanceKey )
{
    public static ItemLifecycleDto FromDomain( ItemInstance item )
    {
        return new ItemLifecycleDto(
            item.InstanceKey,
            item.Version,
            item.Quantity,
            item.IsDepleted,
            item.MaximumCharges,
            item.CurrentCharges,
            item.Hardness,
            item.MaximumHitPoints,
            item.CurrentHitPoints,
            item.BrokenThreshold,
            item.IsBroken,
            item.IsDestroyed,
            item.AttachableRuneCode,
            item.RuneTargetKind,
            item.AttachedToInstanceKey );
    }
}
