namespace Pathfinder.CharacterManagement.Domain.Entity;

public sealed class Ancestry
{
    public AncestryType AncestryType { get; }
    public IReadOnlyList<AncestryBoostSlot> AbilityBoosts { get; }
    public IReadOnlyList<AbilityType> AbilityFlaws { get; }
    public int BaseHitPoints { get; }
    public RaceSizeType Size { get; }
    public int BaseSpeed { get; }
    public VisionType Vision { get; }
    public bool Darkvision => Vision == VisionType.Darkvision;
    public bool LowLightVision => Vision == VisionType.LowLight;
    public SourceReference Source { get; }
    public IReadOnlyList<LanguageId> StartingLanguages { get; }
    public AdditionalLanguageRule? AdditionalLanguageRule { get; }
    public IReadOnlyList<GrantedItem> GrantedItems { get; }
    public IReadOnlyList<GrantedRule> GrantedRules { get; }
    public IReadOnlyList<Heritage> Heritages { get; }
    public IReadOnlyList<AncestryFeat> AncestryFeats { get; }

    public Ancestry(
        AncestryType ancestryType,
        IReadOnlyList<AncestryBoostSlot> abilityBoosts,
        IReadOnlyList<AbilityType> abilityFlaws,
        int baseHitPoints,
        RaceSizeType size,
        int baseSpeed,
        bool darkvision = false,
        bool lowLightVision = false,
        SourceReference? source = null,
        VisionType? vision = null,
        IReadOnlyList<LanguageId>? startingLanguages = null,
        AdditionalLanguageRule? additionalLanguageRule = null,
        IReadOnlyList<GrantedItem>? grantedItems = null,
        IReadOnlyList<GrantedRule>? grantedRules = null,
        IReadOnlyList<Heritage>? heritages = null,
        IReadOnlyList<AncestryFeat>? ancestryFeats = null )
    {
        AncestryType = ancestryType;
        AbilityBoosts = abilityBoosts;
        AbilityFlaws = abilityFlaws;
        BaseHitPoints = baseHitPoints;
        Size = size;
        BaseSpeed = baseSpeed;
        Vision = vision ?? ResolveLegacyVision( darkvision, lowLightVision );
        Source = source ?? SourceReference.Unknown;
        StartingLanguages = startingLanguages ?? [];
        AdditionalLanguageRule = additionalLanguageRule;
        GrantedItems = grantedItems ?? [];
        GrantedRules = grantedRules ?? [];
        Heritages = heritages ?? [];
        AncestryFeats = ancestryFeats ?? [];
    }

    private static VisionType ResolveLegacyVision( bool darkvision, bool lowLightVision )
    {
        if ( darkvision )
        {
            return VisionType.Darkvision;
        }

        if ( lowLightVision )
        {
            return VisionType.LowLight;
        }

        return VisionType.None;
    }
}
