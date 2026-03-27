using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Ancestries;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetAncestriesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAllConfiguredAncestriesSortedByName()
    {
        AncestryRepository ancestryRepository = new AncestryRepository();
        GetAncestriesHandler handler = new GetAncestriesHandler( ancestryRepository );

        IReadOnlyCollection<AncestryDto> result = await handler.Handle( new GetAncestriesCommand(), CancellationToken.None );

        Assert.Equal( 6, result.Count );
        Assert.Equal(
            new[] { "Dwarf", "Elf", "Gnome", "Goblin", "Halfling", "Human", },
            result.Select( ancestry => ancestry.Name )
               .ToArray() );
    }

    [Fact]
    public async Task Handle_MapsHumanFreeBoostsAndVisionFlags()
    {
        AncestryRepository ancestryRepository = new AncestryRepository();
        GetAncestriesHandler handler = new GetAncestriesHandler( ancestryRepository );

        IReadOnlyCollection<AncestryDto> result = await handler.Handle( new GetAncestriesCommand(), CancellationToken.None );
        AncestryDto human = Assert.Single( result.Where( ancestry => ancestry.Type == AncestryType.Human ) );

        Assert.Equal( 2, human.AbilityBoosts.Count );
        Assert.All( human.AbilityBoosts, boost => Assert.True( boost.IsFree ) );
        Assert.Empty( human.AbilityFlaws );
        Assert.False( human.Darkvision );
        Assert.False( human.LowLightVision );
    }

    [Fact]
    public async Task Handle_MapsGoblinDarkvisionAndFixedBoosts()
    {
        AncestryRepository ancestryRepository = new AncestryRepository();
        GetAncestriesHandler handler = new GetAncestriesHandler( ancestryRepository );

        IReadOnlyCollection<AncestryDto> result = await handler.Handle( new GetAncestriesCommand(), CancellationToken.None );
        AncestryDto goblin = Assert.Single( result.Where( ancestry => ancestry.Type == AncestryType.Goblin ) );

        Assert.True( goblin.Darkvision );
        Assert.False( goblin.LowLightVision );
        Assert.Contains( goblin.AbilityFlaws, abilityType => abilityType == AbilityType.Wisdom );
        Assert.Contains(
            goblin.AbilityBoosts,
            boost => boost.AbilityType == AbilityType.Dexterity && !boost.IsFree );
        Assert.Contains(
            goblin.AbilityBoosts,
            boost => boost.AbilityType == AbilityType.Charisma && !boost.IsFree );
        Assert.Contains( goblin.AbilityBoosts, boost => boost.IsFree );
    }
}