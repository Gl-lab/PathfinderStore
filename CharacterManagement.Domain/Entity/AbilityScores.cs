namespace Pathfinder.CharacterManagement.Domain.Entity;

public class AbilityScores
{
    public Characteristic Strength { get; private set; }
    public Characteristic Dexterity { get; private set; }
    public Characteristic Constitution { get; private set; }
    public Characteristic Intelligence { get; private set; }
    public Characteristic Wisdom { get; private set; }
    public Characteristic Charisma { get; private set; }

    public int MaxPortableWeight => MaxPortableWeightByLevel[
        Strength.Value < 0 ? 0 : Strength.Value > 29 ? 29 : Strength.Value ];

    // Приватный конструктор для EF Core
    private AbilityScores()
    {
    }

    public AbilityScores(
        Characteristic strength,
        Characteristic dexterity,
        Characteristic constitution,
        Characteristic intelligence,
        Characteristic wisdom,
        Characteristic charisma )
    {
        Strength = strength ?? throw new ArgumentNullException( nameof( strength ) );
        Dexterity = dexterity ?? throw new ArgumentNullException( nameof( dexterity ) );
        Constitution = constitution ?? throw new ArgumentNullException( nameof( constitution ) );
        Intelligence = intelligence ?? throw new ArgumentNullException( nameof( intelligence ) );
        Wisdom = wisdom ?? throw new ArgumentNullException( nameof( wisdom ) );
        Charisma = charisma ?? throw new ArgumentNullException( nameof( charisma ) );
    }

    public static AbilityScores CreateDefault()
    {
        return new AbilityScores(
            new Characteristic( AbilityType.Strength, 10 ),
            new Characteristic( AbilityType.Dexterity, 10 ),
            new Characteristic( AbilityType.Constitution, 10 ),
            new Characteristic( AbilityType.Intelligence, 10 ),
            new Characteristic( AbilityType.Wisdom, 10 ),
            new Characteristic( AbilityType.Charisma, 10 )
        );
    }

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

    public void UpdateCharacteristic( AbilityType abilityType, int newValue )
    {
        Characteristic characteristic = new Characteristic( abilityType, newValue );
        switch ( abilityType )
        {
            case AbilityType.Strength:
                Strength = characteristic;
                break;
            case AbilityType.Dexterity:
                Dexterity = characteristic;
                break;
            case AbilityType.Constitution:
                Constitution = characteristic;
                break;
            case AbilityType.Intelligence:
                Intelligence = characteristic;
                break;
            case AbilityType.Wisdom:
                Wisdom = characteristic;
                break;
            case AbilityType.Charisma:
                Charisma = characteristic;
                break;
            default:
                throw new ArgumentOutOfRangeException( nameof( abilityType ), abilityType, null );
        }
    }

    public void ApplyAbilityBoost( AbilityType abilityType )
    {
        Characteristic current = GetCharacteristic( abilityType );
        Characteristic updated = current.Increase( 2 );
        UpdateCharacteristic( abilityType, updated.Value );
    }

    public void RemoveAbilityBoost( AbilityType abilityType )
    {
        Characteristic current = GetCharacteristic( abilityType );
        Characteristic updated = current.Decrease( 2 );
        UpdateCharacteristic( abilityType, updated.Value );
    }

    public void RemoveAbilityFlaw( AbilityType abilityType )
    {
        Characteristic current = GetCharacteristic( abilityType );
        Characteristic updated = current.Increase( 2 );
        UpdateCharacteristic( abilityType, updated.Value );
    }

    public void ApplyAbilityFlaw( AbilityType abilityType )
    {
        Characteristic current = GetCharacteristic( abilityType );
        Characteristic updated = current.Decrease( 2 );
        UpdateCharacteristic( abilityType, updated.Value );
    }

    private readonly Dictionary<int, int> MaxPortableWeightByLevel = new()
    {
        { 0, 0 },
        { 1, 10 },
        { 2, 20 },
        { 3, 30 },
        { 4, 40 },
        { 5, 50 },
        { 6, 60 },
        { 7, 70 },
        { 8, 80 },
        { 9, 90 },
        { 10, 100 },
        { 11, 115 },
        { 12, 130 },
        { 13, 150 },
        { 14, 175 },
        { 15, 200 },
        { 16, 230 },
        { 17, 260 },
        { 18, 300 },
        { 19, 350 },
        { 20, 400 },
        { 21, 460 },
        { 22, 520 },
        { 23, 600 },
        { 24, 700 },
        { 25, 800 },
        { 26, 920 },
        { 27, 1040 },
        { 28, 1200 },
        { 29, 1400 }
    };
}