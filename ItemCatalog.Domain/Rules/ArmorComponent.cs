using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Rules;

public sealed class ArmorComponent : Entity
{
    private ArmorComponent()
    {
    }

    public int ItemRevisionId { get; private set; }
    public ArmorCategory Category { get; private set; }
    public int ArmorClassBonus { get; private set; }
    public int DexterityCap { get; private set; }
    public int CheckPenalty { get; private set; }
    public int SpeedPenaltyFeet { get; private set; }
    public int StrengthRequirement { get; private set; }

    public static ArmorComponent Create(
        ArmorCategory category,
        int armorClassBonus,
        int dexterityCap,
        int checkPenalty,
        int speedPenaltyFeet,
        int strengthRequirement )
    {
        if ( !Enum.IsDefined( category ) )
        {
            throw new ItemCatalogException( "Armor category is invalid." );
        }

        if ( ( armorClassBonus < 0 ) || ( dexterityCap < 0 ) ||
             ( checkPenalty > 0 ) || ( speedPenaltyFeet > 0 ) ||
             ( strengthRequirement < 0 ) )
        {
            throw new ItemCatalogException( "Armor statistics are invalid." );
        }

        return new ArmorComponent
        {
            Category = category,
            ArmorClassBonus = armorClassBonus,
            DexterityCap = dexterityCap,
            CheckPenalty = checkPenalty,
            SpeedPenaltyFeet = speedPenaltyFeet,
            StrengthRequirement = strengthRequirement,
        };
    }
}