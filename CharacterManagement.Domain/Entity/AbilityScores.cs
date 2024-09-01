namespace Pathfinder.CharacterManagement.Domain.Entity;

public class AbilityScores: Pathfinder.Utils.Entities.Base.Entity
{
    public Characteristic Strength { get; set; }
    public Characteristic Dexterity { get; set; }
    public Characteristic Constitution { get; set; }
    public Characteristic Intelligence { get; set; }
    public Characteristic Wisdom { get; set; }
    public Characteristic Charisma { get; set; }
    public int MaxPortableWeight => MaxPortableWeightByLevel[Strength.Value < 0? 0: Strength.Value>29?29:Strength.Value];

    public Characteristic GetCharacteristic( AbilityType abilityType )
    {
        return abilityType switch
               {
                   AbilityType.Strength => Strength,
                   AbilityType.Dexterity => Dexterity,
                   AbilityType.Constitution => Constitution,
                   AbilityType.Intelligence => Intelligence,
                   AbilityType.Wisdom => Wisdom,
                   AbilityType.Charisma => Charisma,
                   _ => throw new ArgumentOutOfRangeException( nameof( abilityType ), abilityType, null )
               };
    }
        
    public static AbilityScores InitializationAbilityScores()
    {
        AbilityScores abilityScores = new()
        {
            Charisma = new Characteristic()
            {
                Value = 10,
                AbilityType = AbilityType.Charisma
            },
            Constitution = new Characteristic()
            {
                Value = 10,
                AbilityType = AbilityType.Constitution
            },
            Wisdom = new Characteristic()
            {
                Value = 10,
                AbilityType = AbilityType.Wisdom
            },
            Dexterity = new Characteristic()
            {
                Value = 10,
                AbilityType = AbilityType.Dexterity
            },
            Intelligence = new Characteristic()
            {
                Value = 10,
                AbilityType = AbilityType.Intelligence
            },
            Strength = new Characteristic()
            {
                Value = 10,
                AbilityType = AbilityType.Strength
            }
        };
        return abilityScores;
    }
    private readonly Dictionary<int, int> MaxPortableWeightByLevel = new()
    {
        {0,0},
        {1,10},
        {2,20},
        {3,30},
        {4,40},
        {5,50},
        {6,60},
        {7,70},
        {8,80},
        {9,90},
        {10,100},
        {11,115},
        {12,130},
        {13,150},
        {14,175},
        {15,200},
        {16,230},
        {17,260},
        {18,300},
        {19,350},
        {20,400},
        {21,460},
        {22,520},
        {23,600},
        {24,700},
        {25,800},
        {26,920},
        {27,1040},
        {28,1200},
        {29,1400}
    };
}