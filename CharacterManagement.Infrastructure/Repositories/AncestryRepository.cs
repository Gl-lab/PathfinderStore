using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class AncestryRepository : IAncestryRepository
{
    private static readonly Dictionary<AncestryType, Ancestry> Ancestries = new()
    {
        [AncestryType.Dwarf] = new Ancestry(
            AncestryType.Dwarf,
            abilityBoosts:
            [
                AncestryBoostSlot.Fixed( AbilityType.Constitution ),
                AncestryBoostSlot.Fixed( AbilityType.Wisdom ),
                AncestryBoostSlot.Free()
            ],
            abilityFlaws: [ AbilityType.Charisma ],
            baseHitPoints: 10,
            size: RaceSizeType.Medium,
            baseSpeed: 20 ),

        [AncestryType.Elf] = new Ancestry(
            AncestryType.Elf,
            abilityBoosts:
            [
                AncestryBoostSlot.Fixed( AbilityType.Dexterity ),
                AncestryBoostSlot.Fixed( AbilityType.Intelligence ),
                AncestryBoostSlot.Free()
            ],
            abilityFlaws: [ AbilityType.Constitution ],
            baseHitPoints: 6,
            size: RaceSizeType.Medium,
            baseSpeed: 30,
            lowLightVision: true ),

        [AncestryType.Gnome] = new Ancestry(
            AncestryType.Gnome,
            abilityBoosts:
            [
                AncestryBoostSlot.Fixed( AbilityType.Constitution ),
                AncestryBoostSlot.Fixed( AbilityType.Charisma ),
                AncestryBoostSlot.Free()
            ],
            abilityFlaws: [ AbilityType.Strength ],
            baseHitPoints: 8,
            size: RaceSizeType.Small,
            baseSpeed: 25,
            lowLightVision: true ),

        [AncestryType.Goblin] = new Ancestry(
            AncestryType.Goblin,
            abilityBoosts:
            [
                AncestryBoostSlot.Fixed( AbilityType.Dexterity ),
                AncestryBoostSlot.Fixed( AbilityType.Charisma ),
                AncestryBoostSlot.Free()
            ],
            abilityFlaws: [ AbilityType.Wisdom ],
            baseHitPoints: 6,
            size: RaceSizeType.Small,
            baseSpeed: 25,
            darkvision: true ),

        [AncestryType.Halfling] = new Ancestry(
            AncestryType.Halfling,
            abilityBoosts:
            [
                AncestryBoostSlot.Fixed( AbilityType.Dexterity ),
                AncestryBoostSlot.Fixed( AbilityType.Wisdom ),
                AncestryBoostSlot.Free()
            ],
            abilityFlaws: [ AbilityType.Strength ],
            baseHitPoints: 6,
            size: RaceSizeType.Small,
            baseSpeed: 25 ),

        [AncestryType.Human] = new Ancestry(
            AncestryType.Human,
            abilityBoosts: [ AncestryBoostSlot.Free(), AncestryBoostSlot.Free() ],
            abilityFlaws: [],
            baseHitPoints: 8,
            size: RaceSizeType.Medium,
            baseSpeed: 25 ),
    };

    public IReadOnlyCollection<Ancestry> GetAll() => Ancestries.Values.ToList();

    public Ancestry GetAncestry( AncestryType ancestryType )
    {
        if ( !Ancestries.TryGetValue( ancestryType, out Ancestry? ancestry ) )
        {
            throw new ArgumentOutOfRangeException( nameof( ancestryType ), $"Ancestry '{ancestryType}' is not defined." );
        }

        return ancestry;
    }
}
