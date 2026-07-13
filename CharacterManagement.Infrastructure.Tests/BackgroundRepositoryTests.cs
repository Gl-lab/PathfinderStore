using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class BackgroundRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsCompleteBaselineCatalog()
    {
        BackgroundRepository repository = new BackgroundRepository();

        IReadOnlyCollection<Background> backgrounds = repository.GetAll();

        Assert.Equal( 35, backgrounds.Count );
        Assert.Equal( backgrounds.Count, backgrounds.Select( background => background.Id ).Distinct().Count() );
        Assert.All( backgrounds, background => Assert.Equal( 2, background.RestrictedBoostOptions.Count ) );
        Assert.All( backgrounds, background => Assert.Equal( 1, background.FreeBoostCount ) );
        Assert.All( backgrounds, background => Assert.Equal( 3, background.Grants.Count ) );
    }

    [Fact]
    public void GetBackground_KnownId_ReturnsBackground()
    {
        BackgroundRepository repository = new BackgroundRepository();

        Background background = repository.GetBackground( "background.acrobat" );

        Assert.Equal( "Acrobat", background.Name );
        Assert.Equal(
            [ AbilityType.Strength, AbilityType.Dexterity ],
            background.RestrictedBoostOptions );
    }

    [Fact]
    public void GetBackground_UnknownId_Throws()
    {
        BackgroundRepository repository = new BackgroundRepository();

        Assert.Throws<ArgumentOutOfRangeException>( () =>
            repository.GetBackground( "background.unknown" ) );
    }
}
