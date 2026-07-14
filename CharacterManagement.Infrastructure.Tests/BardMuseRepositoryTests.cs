using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class BardMuseRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsFourPlayerCoreMuses()
    {
        BardMuseRepository repository = new BardMuseRepository();

        IReadOnlyCollection<BardMuse> muses = repository.GetAll();

        Assert.Equal( 4, muses.Count );
        Assert.Equal(
            [
                "bard_muse.enigma",
                "bard_muse.maestro",
                "bard_muse.polymath",
                "bard_muse.warrior",
            ],
            muses.Select( muse => muse.Id ).OrderBy( id => id ) );
    }

    [Fact]
    public void GetBardMuse_Enigma_ReturnsVerifiedBenefits()
    {
        BardMuseRepository repository = new BardMuseRepository();

        BardMuse muse = repository.GetBardMuse( "bard_muse.enigma" );

        Assert.Contains( muse.Benefits, benefit => benefit.Id == "feat.bardic_lore" );
        Assert.Contains( muse.Benefits, benefit => benefit.Id == "spell.sure_strike" );
        Assert.Equal( 98, muse.Source.Page );
    }
}
