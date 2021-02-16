namespace Pathfinder.Application.DTO
{
    public class GroupCharacteristicDto
    {
        public virtual CharacteristicDto Strength { get; set; }
        public virtual CharacteristicDto Dexterity { get; set; }
        public virtual CharacteristicDto Constitution { get; set; }
        public virtual CharacteristicDto Intelligence { get; set; }
        public virtual CharacteristicDto Wisdom { get; set; }
        public virtual CharacteristicDto Charisma { get; set; }
        public int MaxPortableWeight { get; set; }
    }
}