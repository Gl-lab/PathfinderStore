namespace Pathfinder.CharacterManagement.Domain.Entity;

public class Ancestry
{
    public AncestryType AncestryType { get; }
    public IReadOnlyList<AncestryBoostSlot> AbilityBoosts { get; }
    public IReadOnlyList<AbilityType> AbilityFlaws { get; }
    public int BaseHitPoints { get; }
    public RaceSizeType Size { get; }
    public int BaseSpeed { get; }
    public bool Darkvision { get; }
    public bool LowLightVision { get; }

    public Ancestry(
        AncestryType ancestryType,
        IReadOnlyList<AncestryBoostSlot> abilityBoosts,
        IReadOnlyList<AbilityType> abilityFlaws,
        int baseHitPoints,
        RaceSizeType size,
        int baseSpeed,
        bool darkvision = false,
        bool lowLightVision = false )
    {
        AncestryType = ancestryType;
        AbilityBoosts = abilityBoosts;
        AbilityFlaws = abilityFlaws;
        BaseHitPoints = baseHitPoints;
        Size = size;
        BaseSpeed = baseSpeed;
        Darkvision = darkvision;
        LowLightVision = lowLightVision;
    }
}
