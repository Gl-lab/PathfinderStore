using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Feats;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetFeatOptionsHandlerTests
{
    [Theory]
    [InlineData( FeatCategory.Ancestry, "Human" )]
    [InlineData( FeatCategory.Class, "Fighter" )]
    [InlineData( FeatCategory.Skill, "Skill" )]
    public async Task Handle_ReturnsCommonOptionsForRequestedCategoryAndTrait(
        FeatCategory category,
        string requiredTrait )
    {
        GetFeatOptionsHandler handler = new GetFeatOptionsHandler( new FeatRepository(
            new AncestryRepository(),
            new BackgroundRepository() ) );

        IReadOnlyCollection<FeatDefinitionDto> result = await handler.Handle(
            new GetFeatOptionsCommand( category, 1, requiredTrait ),
            CancellationToken.None );

        Assert.NotEmpty( result );
        Assert.All( result, feat => Assert.Equal( category, feat.Category ) );
        Assert.All( result, feat => Assert.Equal( 1, feat.Level ) );
        Assert.All( result, feat => Assert.Equal( FeatRarity.Common, feat.Rarity ) );
        Assert.All(
            result,
            feat => Assert.Contains( requiredTrait, feat.Traits, StringComparer.OrdinalIgnoreCase ) );
    }
}
