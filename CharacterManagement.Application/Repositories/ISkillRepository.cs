using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface ISkillRepository
{
    IReadOnlyCollection<SkillDefinition> GetAll();
    SkillDefinition GetSkill( string skillId );
}
