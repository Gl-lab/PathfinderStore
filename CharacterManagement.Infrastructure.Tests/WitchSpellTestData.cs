using Pathfinder.CharacterManagement.Application.DTO;

namespace CharacterManagement.Infrastructure.Tests;

internal static class WitchSpellTestData
{
    public static string[] DivineCantrips() =>
        [
            "spell.daze", "spell.detect_magic", "spell.divine_lance", "spell.forbidding_ward",
            "spell.guidance", "spell.know_the_way", "spell.light", "spell.message",
            "spell.prestidigitation", "spell.read_aura",
        ];

    public static string[] DivineSpells() =>
        [ "spell.bane", "spell.bless", "spell.harm", "spell.heal", "spell.mending" ];

    public static string[] PrimalCantrips() =>
        [
            "spell.caustic_blast", "spell.detect_magic", "spell.electric_arc", "spell.frostbite",
            "spell.gouging_claw", "spell.guidance", "spell.ignition", "spell.know_the_way",
            "spell.light", "spell.prestidigitation",
        ];

    public static string[] PrimalSpells() =>
        [ "spell.air_bubble", "spell.alarm", "spell.ant_haul", "spell.fear", "spell.heal" ];

    public static void ApplyPrimal( CreateCharacterRequestDto character )
    {
        character.WitchFamiliarCantripIds = PrimalCantrips();
        character.WitchFamiliarSpellIds = PrimalSpells();
        character.WitchPreparedCantripIds = PrimalCantrips().Take( 5 ).ToArray();
        character.WitchPreparedSpellIds = [ "spell.heal", "spell.heal" ];
        character.WitchFocusHexId = "spell.phase_familiar";
    }
}
