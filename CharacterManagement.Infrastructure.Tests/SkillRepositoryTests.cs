using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class SkillRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsPlayerCoreCatalog()
    {
        SkillRepository repository = new SkillRepository();

        IReadOnlyCollection<SkillDefinition> skills = repository.GetAll();

        Assert.Equal( 16, skills.Count );
        Assert.Equal( skills.Count, skills.Select( skill => skill.Id ).Distinct().Count() );
        Assert.DoesNotContain( skills, skill => skill.Name == "Lore" );
        Assert.All( skills, skill => Assert.StartsWith( "skill.", skill.Id ) );
    }

    [Theory]
    [InlineData( "skill.acrobatics", AbilityType.Dexterity )]
    [InlineData( "skill.athletics", AbilityType.Strength )]
    [InlineData( "skill.arcana", AbilityType.Intelligence )]
    [InlineData( "skill.religion", AbilityType.Wisdom )]
    [InlineData( "skill.diplomacy", AbilityType.Charisma )]
    public void GetSkill_ReturnsCorrectKeyAbility( string skillId, AbilityType expectedKeyAbility )
    {
        SkillRepository repository = new SkillRepository();

        SkillDefinition skill = repository.GetSkill( skillId );

        Assert.Equal( expectedKeyAbility, skill.KeyAbility );
    }

    [Fact]
    public void GetSkill_UnknownId_Throws()
    {
        SkillRepository repository = new SkillRepository();

        Assert.Throws<ArgumentOutOfRangeException>( () =>
            repository.GetSkill( "skill.unknown" ) );
    }
}
