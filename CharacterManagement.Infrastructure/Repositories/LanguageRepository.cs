using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class LanguageRepository : ILanguageRepository
{
    private static readonly Dictionary<string, LanguageDefinition> _languages = CreateLanguages()
        .ToDictionary( language => language.Id.Value, StringComparer.Ordinal );

    public IReadOnlyCollection<LanguageDefinition> GetAll() => _languages.Values.ToList();

    public LanguageDefinition GetLanguage( string languageId )
    {
        if ( String.IsNullOrWhiteSpace( languageId ) )
        {
            throw new ArgumentException( "Language id cannot be empty.", nameof( languageId ) );
        }

        if ( !_languages.TryGetValue( languageId, out LanguageDefinition? language ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( languageId ),
                $"Language '{languageId}' is not defined." );
        }

        return language;
    }

    private static IReadOnlyCollection<LanguageDefinition> CreateLanguages()
    {
        SourceReference standardSource = new SourceReference( "Player Core", 89 );
        SourceReference regionalSource = new SourceReference( "Player Core", 34 );
        return
        [
            Standard( LanguageIds.Common, "Common", "Humans, dwarves, elves, halflings, and other common ancestries", LanguageRarity.Common, standardSource ),
            Standard( LanguageIds.Draconic, "Draconic", "Dragons and reptilian humanoids", LanguageRarity.Common, standardSource ),
            Standard( LanguageIds.Dwarven, "Dwarven", "Dwarves", LanguageRarity.Common, standardSource ),
            Standard( LanguageIds.Elven, "Elven", "Elves and aiuvarins", LanguageRarity.Common, standardSource ),
            Standard( LanguageIds.Fey, "Fey", "Fey, centaurs, plant creatures, and fungus creatures", LanguageRarity.Common, standardSource ),
            Standard( LanguageIds.Gnomish, "Gnomish", "Gnomes", LanguageRarity.Common, standardSource ),
            Standard( LanguageIds.Goblin, "Goblin", "Goblins, hobgoblins, and bugbears", LanguageRarity.Common, standardSource ),
            Standard( LanguageIds.Halfling, "Halfling", "Halflings", LanguageRarity.Common, standardSource ),
            Standard( LanguageIds.Jotun, "Jotun", "Giants, ogres, trolls, ettins, and cyclopes", LanguageRarity.Common, standardSource ),
            Standard( LanguageIds.Orcish, "Orcish", "Orcs and dromaars", LanguageRarity.Common, standardSource ),
            Standard( LanguageIds.Sakvroth, "Sakvroth", "Subterranean civilizations and serpentfolk", LanguageRarity.Common, standardSource ),
            Standard( LanguageIds.Aklo, "Aklo", "Evil fey and otherworldly monsters", LanguageRarity.Uncommon, standardSource ),
            Standard( LanguageIds.Chthonian, "Chthonian", "Demons", LanguageRarity.Uncommon, standardSource ),
            Standard( LanguageIds.Diabolic, "Diabolic", "Devils", LanguageRarity.Uncommon, standardSource ),
            Standard( LanguageIds.Empyrean, "Empyrean", "Angels and other celestials", LanguageRarity.Uncommon, standardSource ),
            Standard( LanguageIds.Kholo, "Kholo", "Kholos", LanguageRarity.Uncommon, standardSource ),
            Standard( LanguageIds.Necril, "Necril", "Ghouls and intelligent undead", LanguageRarity.Uncommon, standardSource ),
            Standard( LanguageIds.Petran, "Petran", "Earth elemental creatures", LanguageRarity.Uncommon, standardSource ),
            Standard( LanguageIds.Pyric, "Pyric", "Fire elemental creatures", LanguageRarity.Uncommon, standardSource ),
            Standard( LanguageIds.Shadowtongue, "Shadowtongue", "Nidalese and Netherworld creatures", LanguageRarity.Uncommon, standardSource ),
            Standard( LanguageIds.Sussuran, "Sussuran", "Air elemental creatures and flying creatures", LanguageRarity.Uncommon, standardSource ),
            Standard( LanguageIds.Thalassic, "Thalassic", "Aquatic creatures and water elemental creatures", LanguageRarity.Uncommon, standardSource ),
            Standard( LanguageIds.Muan, "Muan", "Wood elemental creatures", LanguageRarity.Uncommon, standardSource ),
            Standard( LanguageIds.Talican, "Talican", "Metal elemental creatures", LanguageRarity.Uncommon, standardSource ),
            Regional( LanguageIds.Hallit, "Hallit", "Broken Lands, Eye of Dread, and Saga Lands", regionalSource ),
            Regional( LanguageIds.Kelish, "Kelish", "Golden Road", regionalSource ),
            Regional( LanguageIds.Mwangi, "Mwangi", "Mwangi Expanse, Shackles, Thuvia, and Vidrian", regionalSource ),
            Regional( LanguageIds.Osiriani, "Osiriani", "Geb, Katapesh, Mana Wastes, Nex, Osirion, Rahadoum, and Thuvia", regionalSource ),
            Regional( LanguageIds.Shoanti, "Shoanti", "Hold of Belkzen and Varisia", regionalSource ),
            Regional( LanguageIds.Skald, "Skald", "Irrisen and Lands of the Linnorm Kings", regionalSource ),
            Regional( LanguageIds.Tien, "Tien", "Lands of the Linnorm Kings, Realm of the Mammoth Lords, and Tian Xia", regionalSource ),
            Regional( LanguageIds.Varisian, "Varisian", "Brevoy, Gravelands, Nidal, Nirmathas, Ustalav, and Varisia", regionalSource ),
            Regional( LanguageIds.Vudrani, "Vudrani", "Jalmeray, Katapesh, Nex, and Vudra", regionalSource ),
        ];
    }

    private static LanguageDefinition Standard(
        LanguageId id,
        string name,
        string speakers,
        LanguageRarity rarity,
        SourceReference source ) => new LanguageDefinition(
            id,
            name,
            speakers,
            rarity,
            LanguageCategory.Standard,
            source );

    private static LanguageDefinition Regional(
        LanguageId id,
        string name,
        string speakers,
        SourceReference source ) => new LanguageDefinition(
            id,
            name,
            speakers,
            LanguageRarity.Uncommon,
            LanguageCategory.Regional,
            source );
}
