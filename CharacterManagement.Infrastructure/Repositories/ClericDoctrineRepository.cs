using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class ClericDoctrineRepository : IClericDoctrineRepository
{
    private static readonly Dictionary<string, ClericDoctrine> Doctrines = CreateDoctrines()
        .ToDictionary( doctrine => doctrine.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<ClericDoctrine> GetAll() => Doctrines.Values.ToList();

    public ClericDoctrine GetClericDoctrine( string clericDoctrineId )
    {
        if ( String.IsNullOrWhiteSpace( clericDoctrineId ) )
        {
            throw new ArgumentException( "Cleric doctrine id cannot be empty.", nameof( clericDoctrineId ) );
        }

        if ( !Doctrines.TryGetValue( clericDoctrineId, out ClericDoctrine? doctrine ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( clericDoctrineId ),
                $"Cleric doctrine '{clericDoctrineId}' is not defined." );
        }

        return doctrine;
    }

    private static IReadOnlyCollection<ClericDoctrine> CreateDoctrines()
    {
        return
        [
            new ClericDoctrine(
                "cleric_doctrine.cloistered",
                "Cloistered Cleric",
                new SourceReference( "Player Core", 112 ),
                [],
                [
                    Effect(
                        "cloistered",
                        "domain_initiate",
                        "Domain Initiate",
                        "Grants Domain Initiate and requires a domain choice.",
                        CharacterClassDependencyType.ClassFeatCatalog,
                        CharacterClassDependencyType.DomainCatalog,
                        CharacterClassDependencyType.DeityCatalog )
                ],
                [
                    CharacterClassDependencyType.ClassFeatCatalog,
                    CharacterClassDependencyType.DomainCatalog,
                    CharacterClassDependencyType.DeityCatalog,
                ] ),
            new ClericDoctrine(
                "cleric_doctrine.warpriest",
                "Warpriest",
                new SourceReference( "Player Core", 112 ),
                [
                    Proficiency(
                        ProficiencyTargets.Fortitude,
                        ProficiencyRank.Expert,
                        "fortitude" ),
                    Proficiency(
                        ProficiencyTargets.LightArmor,
                        ProficiencyRank.Trained,
                        "light_armor" ),
                    Proficiency(
                        ProficiencyTargets.MediumArmor,
                        ProficiencyRank.Trained,
                        "medium_armor" ),
                ],
                [
                    Effect(
                        "warpriest",
                        "shield_block",
                        "Shield Block",
                        "Grants the Shield Block general feat.",
                        CharacterClassDependencyType.GeneralFeatCatalog ),
                    Effect(
                        "warpriest",
                        "deadly_simplicity",
                        "Deadly Simplicity",
                        "Conditionally grants Deadly Simplicity for an eligible deity favored weapon.",
                        CharacterClassDependencyType.ClassFeatCatalog,
                        CharacterClassDependencyType.DeityCatalog,
                        CharacterClassDependencyType.WeaponCatalog ),
                ],
                [
                    CharacterClassDependencyType.GeneralFeatCatalog,
                    CharacterClassDependencyType.ClassFeatCatalog,
                    CharacterClassDependencyType.DeityCatalog,
                    CharacterClassDependencyType.WeaponCatalog,
                ] ),
        ];
    }

    private static ProficiencyGrant Proficiency(
        ProficiencyTarget target,
        ProficiencyRank rank,
        string grantId )
    {
        return new ProficiencyGrant(
            target,
            rank,
            $"cleric_doctrine.warpriest.proficiency.{grantId}" );
    }

    private static ClericDoctrineEffectDescriptor Effect(
        string doctrineId,
        string effectId,
        string name,
        string summary,
        params CharacterClassDependencyType[] deferredDependencies )
    {
        return new ClericDoctrineEffectDescriptor(
            $"cleric_doctrine.{doctrineId}.effect.{effectId}",
            name,
            summary,
            deferredDependencies );
    }
}
