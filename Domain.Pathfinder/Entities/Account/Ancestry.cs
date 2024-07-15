using System.Collections.Generic;

namespace Pathfinder.Core.Entities.Account;

public class Ancestry
{
    public AncestryType AncestryType { get; set; }
    public List<AbilityType> AbilityBoosts { get; set; }
    public List<AbilityType> AbilityFlaws { get; set; }
    public int BaseHitPoints { get; set; }
    public RaceSizeType Size { get; set; }
    public int BaseSpeed { get; set; }
    //public List<Language> Languages
    //public List<Language> AdditionalLanguages
    public bool Darkvision { get; set; }
    public bool LowLightVision { get; set; }
}