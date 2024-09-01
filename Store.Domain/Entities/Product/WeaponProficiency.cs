namespace Pathfinder.Store.Domain.Entities.Product;

public class WeaponProficiency
{
    public WeaponProficiencyType Id { get; set; }
    //Пока не заморачиваемся
    public string Name => Id.ToString();
}
    
public enum WeaponProficiencyType: byte
{
    Simple,
    Martial,
    Exotic 
}