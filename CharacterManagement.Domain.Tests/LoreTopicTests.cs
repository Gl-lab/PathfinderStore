using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public sealed class LoreTopicTests
{
    [Fact]
    public void CreateKnown_AddsLoreSuffixWhenMissing()
    {
        LoreTopic result = LoreTopic.CreateKnown( "lore.warfare", "Warfare" );

        Assert.Equal( "lore.warfare", result.Id );
        Assert.Equal( "Warfare Lore", result.Name );
    }

    [Theory]
    [InlineData( "Forest", "lore.custom.forest", "Forest Lore" )]
    [InlineData( "  Forest Lore  ", "lore.custom.forest", "Forest Lore" )]
    [InlineData( "Ancient Ruins", "lore.custom.ancient_ruins", "Ancient Ruins Lore" )]
    public void CreateCustom_NormalizesIdentityAndDisplayName(
        string topic,
        string expectedId,
        string expectedName )
    {
        LoreTopic result = LoreTopic.CreateCustom( topic, CreateSkills() );

        Assert.Equal( expectedId, result.Id );
        Assert.Equal( expectedName, result.Name );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( " Lore" )]
    [InlineData( "---" )]
    public void CreateCustom_InvalidTopic_Throws( string topic )
    {
        Assert.Throws<CharacterManagementException>( () =>
            LoreTopic.CreateCustom( topic, CreateSkills() ) );
    }

    [Theory]
    [InlineData( "Nature" )]
    [InlineData( "nature lore" )]
    public void CreateCustom_DuplicatesGeneralSkill_Throws( string topic )
    {
        Assert.Throws<CharacterManagementException>( () =>
            LoreTopic.CreateCustom( topic, CreateSkills() ) );
    }

    private static IReadOnlyCollection<SkillDefinition> CreateSkills()
    {
        return
        [
            new SkillDefinition(
                "skill.nature",
                "Nature",
                AbilityType.Wisdom,
                SourceReference.Unknown ),
        ];
    }
}
