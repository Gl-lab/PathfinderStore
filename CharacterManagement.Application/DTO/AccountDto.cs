using CharacterManagement.Application.DTO.Base;

namespace CharacterManagement.Application.DTO
{
    public class AccountDto : BaseDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int UserId { get; set; }
        public virtual List<CharacterDto> Characters { get; set; }
        public virtual CharacterDto CurrentCharacter { get; set; }

       //public virtual UserDto User { get; set; }
    }
}