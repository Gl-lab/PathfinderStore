namespace Pathfinder.CharacterManagement.Domain.Entity;

public class Characteristic
{
    public AbilityType AbilityType { get; set; }
    public int Value { get; set; }
    public int Modifier => (Value - 10)/2;
}