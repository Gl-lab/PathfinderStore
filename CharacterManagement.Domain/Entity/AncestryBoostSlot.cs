namespace Pathfinder.CharacterManagement.Domain.Entity;

public abstract record AncestryBoostSlot
{
    public sealed record FixedBoost( AbilityType AbilityType ) : AncestryBoostSlot;
    public sealed record FreeBoost : AncestryBoostSlot;

    public static AncestryBoostSlot Fixed( AbilityType abilityType ) => new FixedBoost( abilityType );
    public static AncestryBoostSlot Free() => new FreeBoost();
}
