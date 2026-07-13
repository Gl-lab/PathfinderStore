using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.CharacterClasses;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetCharacterClassesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteCatalogSortedByName()
    {
        GetCharacterClassesHandler handler = new GetCharacterClassesHandler( new CharacterClassRepository() );

        IReadOnlyCollection<CharacterClassDto> result = await handler.Handle(
            new GetCharacterClassesCommand(),
            CancellationToken.None );

        Assert.Equal( 8, result.Count );
        Assert.Equal(
            result.Select( characterClass => characterClass.Name ).OrderBy( name => name ),
            result.Select( characterClass => characterClass.Name ) );
    }

    [Fact]
    public async Task Handle_MapsClassDataAndDeclarativeRules()
    {
        GetCharacterClassesHandler handler = new GetCharacterClassesHandler( new CharacterClassRepository() );

        IReadOnlyCollection<CharacterClassDto> result = await handler.Handle(
            new GetCharacterClassesCommand(),
            CancellationToken.None );
        CharacterClassDto fighter = Assert.Single(
            result.Where( characterClass => characterClass.Id == "class.fighter" ) );

        Assert.Equal( 10, fighter.BaseHitPoints );
        Assert.Equal( [ AbilityType.Strength, AbilityType.Dexterity ], fighter.KeyAbilityOptions );
        Assert.Contains( fighter.Rules, rule => rule.Kind == CharacterClassRuleKind.InitialProficiencies );
        Assert.Contains( fighter.Rules, rule => rule.Kind == CharacterClassRuleKind.ClassFeatChoice );
    }
}
