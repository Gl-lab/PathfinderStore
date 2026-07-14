using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class BardMuseRepository : IBardMuseRepository
{
    private static readonly Dictionary<string, BardMuse> BardMuses = CreateBardMuses()
        .ToDictionary( bardMuse => bardMuse.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<BardMuse> GetAll() => BardMuses.Values.ToArray();

    public BardMuse GetBardMuse( string bardMuseId )
    {
        if ( String.IsNullOrWhiteSpace( bardMuseId ) )
        {
            throw new ArgumentException( "Bard Muse id cannot be empty.", nameof( bardMuseId ) );
        }

        if ( !BardMuses.TryGetValue( bardMuseId, out BardMuse? bardMuse ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( bardMuseId ),
                $"Bard Muse '{bardMuseId}' is not defined." );
        }

        return bardMuse;
    }

    private static IReadOnlyCollection<BardMuse> CreateBardMuses()
    {
        return
        [
            Create( "enigma", "Enigma", "Bardic Lore", "bardic_lore", "Sure Strike", "sure_strike" ),
            Create( "maestro", "Maestro", "Lingering Composition", "lingering_composition", "Soothe", "soothe" ),
            Create( "polymath", "Polymath", "Versatile Performance", "versatile_performance", "Phantasmal Minion", "phantasmal_minion" ),
            Create( "warrior", "Warrior", "Martial Performance", "martial_performance", "Fear", "fear" ),
        ];
    }

    private static BardMuse Create(
        string id,
        string name,
        string featName,
        string featId,
        string spellName,
        string spellId )
    {
        return new BardMuse(
            $"bard_muse.{id}",
            name,
            new SourceReference( "Player Core", 98 ),
            [
                new BardMuseBenefitDescriptor(
                    $"feat.{featId}",
                    BardMuseBenefitKind.ClassFeat,
                    featName,
                    [ CharacterClassDependencyType.ClassFeatCatalog ] ),
                new BardMuseBenefitDescriptor(
                    $"spell.{spellId}",
                    BardMuseBenefitKind.RepertoireSpell,
                    spellName,
                    [ CharacterClassDependencyType.SpellCatalog ] ),
            ] );
    }
}
