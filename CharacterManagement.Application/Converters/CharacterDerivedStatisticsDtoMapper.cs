using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Equipment;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;
using Pathfinder.CharacterManagement.Domain.Rules.Feats;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class CharacterDerivedStatisticsDtoMapper
{
    private const int CharacterLevel = 1;

    public static CharacterDerivedStatisticsDto Map(
        DraftCharacter character,
        Ancestry ancestry,
        CharacterClass characterClass,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        ISkillRepository? skillRepository,
        FeatTrainingResult? featTraining = null,
        AllowedEquipmentLoadout? equipment = null,
        SpellTradition? spellTradition = null )
    {
        ArgumentNullException.ThrowIfNull( proficiencies );

        CharacterHitPoints hitPoints = CharacterHitPoints.Calculate(
            character,
            ancestry,
            characterClass );

        return new CharacterDerivedStatisticsDto
        {
            HitPoints = new CharacterHitPointsDto
            {
                Maximum = hitPoints.MaximumHitPoints,
                Ancestry = hitPoints.AncestryHitPoints,
                Class = hitPoints.ClassHitPoints,
                ConstitutionModifier = hitPoints.ConstitutionModifier,
            },
            ArmorClass = MapArmorClass( character, proficiencies, equipment ),
            Strikes = MapStrikes( character, proficiencies, equipment ),
            ClassDifficultyClass = MapClassDifficultyClass(
                character,
                characterClass,
                proficiencies ),
            Spellcasting = MapSpellcasting(
                character,
                proficiencies,
                spellTradition ),
            Perception = Map(
                character,
                proficiencies,
                ProficiencyTargets.Perception,
                AbilityType.Wisdom ),
            SavingThrows = new CharacterSavingThrowsDto
            {
                Fortitude = Map(
                    character,
                    proficiencies,
                    ProficiencyTargets.Fortitude,
                    AbilityType.Constitution ),
                Reflex = Map(
                    character,
                    proficiencies,
                    ProficiencyTargets.Reflex,
                    AbilityType.Dexterity ),
                Will = Map(
                    character,
                    proficiencies,
                    ProficiencyTargets.Will,
                    AbilityType.Wisdom ),
            },
            SkillModifiers = CharacterSkillModifiersDtoMapper.Map(
                character,
                skillRepository,
                CharacterLevel,
                featTraining?.Skills,
                featTraining?.Lore ),
        };
    }

    private static CharacterArmorClassDto MapArmorClass(
        DraftCharacter character,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        AllowedEquipmentLoadout? equipment )
    {
        AllowedEquipmentItem? equippedArmor = equipment?.Items
            .SingleOrDefault( item => ( item.EquippedQuantity > 0 ) && ( item.Armor is not null ) );
        string proficiencyTargetId = equippedArmor?.ProficiencyTargetId ?? ProficiencyTargets.UnarmoredDefense.Id;
        EffectiveProficiency proficiency = proficiencies
            .Single( item => item.Target.Id == proficiencyTargetId );
        IReadOnlyList<StatisticBonus> bonuses = equippedArmor?.Armor is null
            ? []
            :
            [
                new StatisticBonus(
                    equippedArmor.Id,
                    StatisticBonusType.Item,
                    equippedArmor.Armor.ArmorClassBonus ),
            ];
        ArmorClassStatistic armorClass = ArmorClassStatistic.Calculate(
            character.AbilityScores.Dexterity,
            proficiency,
            equippedArmor?.Armor?.DexterityCap,
            bonuses,
            CharacterLevel );

        return new CharacterArmorClassDto
        {
            Base = armorClass.Base,
            Ability = armorClass.Ability,
            AbilityModifier = armorClass.AbilityModifier,
            AbilityCap = armorClass.AbilityCap,
            AppliedAbilityModifier = armorClass.AppliedAbilityModifier,
            ProficiencyTargetId = armorClass.ProficiencyTargetId,
            ProficiencyRank = armorClass.ProficiencyRank,
            ProficiencyBonus = armorClass.ProficiencyBonus,
            ProficiencySourceGrantIds = armorClass.ProficiencySourceGrantIds,
            ItemBonuses = armorClass.ItemBonuses.Select( MapBonus ).ToArray(),
            StatusBonuses = armorClass.StatusBonuses.Select( MapBonus ).ToArray(),
            CircumstanceBonuses = armorClass.CircumstanceBonuses.Select( MapBonus ).ToArray(),
            Total = armorClass.Total,
        };
    }

    private static CharacterStatisticBonusDto MapBonus( StatisticBonus bonus )
    {
        return new CharacterStatisticBonusDto
        {
            SourceId = bonus.SourceId,
            Type = bonus.Type,
            Value = bonus.Value,
        };
    }

    private static IReadOnlyList<CharacterStrikeDto> MapStrikes(
        DraftCharacter character,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        AllowedEquipmentLoadout? equipment )
    {
        List<(StrikeProfile Profile, string ProficiencyTargetId)> profiles =
        [
            (
                new StrikeProfile(
                    "strike.unarmed.fist",
                    "Fist",
                    StrikeKind.Unarmed,
                    StrikeMode.Melee,
                    4,
                    "Bludgeoning",
                    [ "Agile", "Finesse", "Nonlethal", "Unarmed" ] ),
                ProficiencyTargets.UnarmedAttacks.Id
            ),
        ];
        IEnumerable<AllowedEquipmentItem> equippedWeapons = equipment?.Items
            .Where( item => ( item.EquippedQuantity > 0 ) && ( item.Weapon is not null ) ) ?? [];
        foreach ( AllowedEquipmentItem weaponItem in equippedWeapons )
        {
            profiles.AddRange( CreateWeaponProfiles( weaponItem ) );
        }

        return profiles
            .Select( entry =>
            {
                EffectiveProficiency proficiency = proficiencies
                    .Single( item => item.Target.Id == entry.ProficiencyTargetId );
                StrikeStatistic strike = StrikeStatistic.Calculate(
                    entry.Profile,
                    character.AbilityScores,
                    proficiency,
                    [],
                    [],
                    CharacterLevel );
                return MapStrike( strike );
            } )
            .ToArray();
    }

    private static IReadOnlyList<(StrikeProfile Profile, string ProficiencyTargetId)> CreateWeaponProfiles(
        AllowedEquipmentItem item )
    {
        AllowedWeaponStatistics weapon = item.Weapon ?? throw new ArgumentException(
            "Allowed equipment item must contain weapon statistics.",
            nameof( item ) );
        string proficiencyTargetId = item.ProficiencyTargetId ?? throw new InvalidOperationException(
            $"Equipped weapon '{item.Id}' must have a proficiency target." );
        StrikeMode primaryMode = weapon.Type == EquipmentWeaponType.Melee
            ? StrikeMode.Melee
            : StrikeMode.Ranged;
        List<(StrikeProfile Profile, string ProficiencyTargetId)> profiles =
        [
            (
                new StrikeProfile(
                    $"strike.{item.Id}.{primaryMode.ToString().ToLowerInvariant()}",
                    item.Name,
                    StrikeKind.Weapon,
                    primaryMode,
                    weapon.DamageDie,
                    weapon.DamageType,
                    weapon.Traits.ToArray() ),
                proficiencyTargetId
            ),
        ];
        bool hasThrownMode = ( primaryMode == StrikeMode.Melee ) &&
                             weapon.Traits.Any( trait => trait.StartsWith( "Thrown", StringComparison.OrdinalIgnoreCase ) );
        if ( hasThrownMode )
        {
            profiles.Add(
                (
                    new StrikeProfile(
                        $"strike.{item.Id}.ranged",
                        item.Name,
                        StrikeKind.Weapon,
                        StrikeMode.Ranged,
                        weapon.DamageDie,
                        weapon.DamageType,
                        weapon.Traits.ToArray() ),
                    proficiencyTargetId
                ) );
        }

        return profiles;
    }

    private static CharacterStrikeDto MapStrike( StrikeStatistic strike )
    {
        return new CharacterStrikeDto
        {
            Id = strike.Profile.Id,
            Name = strike.Profile.Name,
            Kind = strike.Profile.Kind,
            Mode = strike.Profile.Mode,
            Traits = strike.Profile.Traits,
            Attack = new CharacterStrikeAttackDto
            {
                Ability = strike.Attack.Ability,
                AbilityModifier = strike.Attack.AbilityModifier,
                ProficiencyTargetId = strike.Attack.ProficiencyTargetId,
                ProficiencyRank = strike.Attack.ProficiencyRank,
                ProficiencyBonus = strike.Attack.ProficiencyBonus,
                ProficiencySourceGrantIds = strike.Attack.ProficiencySourceGrantIds,
                ItemBonuses = strike.Attack.ItemBonuses.Select( MapBonus ).ToArray(),
                StatusBonuses = strike.Attack.StatusBonuses.Select( MapBonus ).ToArray(),
                CircumstanceBonuses = strike.Attack.CircumstanceBonuses.Select( MapBonus ).ToArray(),
                Total = strike.Attack.Total,
            },
            Damage = new CharacterStrikeDamageDto
            {
                DiceCount = strike.Damage.DiceCount,
                Die = strike.Damage.Die,
                DamageType = strike.Damage.DamageType,
                Ability = strike.Damage.Ability,
                AbilityModifier = strike.Damage.AbilityModifier,
                ItemBonuses = strike.Damage.ItemBonuses.Select( MapBonus ).ToArray(),
                StatusBonuses = strike.Damage.StatusBonuses.Select( MapBonus ).ToArray(),
                CircumstanceBonuses = strike.Damage.CircumstanceBonuses.Select( MapBonus ).ToArray(),
                Formula = strike.Damage.Formula,
            },
        };
    }

    private static CharacterCombatProficiencyStatisticDto? MapClassDifficultyClass(
        DraftCharacter character,
        CharacterClass characterClass,
        IReadOnlyList<EffectiveProficiency> proficiencies )
    {
        if ( !character.SelectedClassKeyAbility.HasValue )
        {
            return null;
        }

        ProficiencyTarget target = ProficiencyTargets.ClassDc( characterClass.Id, characterClass.Name );
        EffectiveProficiency proficiency = proficiencies.Single( item => item.Target.Id == target.Id );
        return MapCombatProficiencyStatistic(
            ProficiencyStatisticKind.DifficultyClass,
            character.SelectedClassKeyAbility.Value,
            character,
            proficiency );
    }

    private static CharacterSpellcastingStatisticsDto? MapSpellcasting(
        DraftCharacter character,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        SpellTradition? tradition )
    {
        if ( !tradition.HasValue || !character.SelectedClassKeyAbility.HasValue )
        {
            return null;
        }

        EffectiveProficiency attackProficiency = proficiencies.Single(
            item => item.Target.Id == ProficiencyTargets.SpellAttack( tradition.Value ).Id );
        EffectiveProficiency dcProficiency = proficiencies.Single(
            item => item.Target.Id == ProficiencyTargets.SpellDc( tradition.Value ).Id );
        return new CharacterSpellcastingStatisticsDto
        {
            Tradition = tradition.Value,
            Attack = MapCombatProficiencyStatistic(
                ProficiencyStatisticKind.Modifier,
                character.SelectedClassKeyAbility.Value,
                character,
                attackProficiency ),
            DifficultyClass = MapCombatProficiencyStatistic(
                ProficiencyStatisticKind.DifficultyClass,
                character.SelectedClassKeyAbility.Value,
                character,
                dcProficiency ),
        };
    }

    private static CharacterCombatProficiencyStatisticDto MapCombatProficiencyStatistic(
        ProficiencyStatisticKind kind,
        AbilityType ability,
        DraftCharacter character,
        EffectiveProficiency proficiency )
    {
        ProficiencyStatistic statistic = ProficiencyStatistic.Calculate(
            kind,
            ability,
            character.AbilityScores.GetCharacteristic( ability ),
            proficiency,
            [],
            CharacterLevel );
        return new CharacterCombatProficiencyStatisticDto
        {
            Kind = statistic.Kind,
            Base = statistic.Base,
            Ability = statistic.Ability,
            AbilityModifier = statistic.AbilityModifier,
            ProficiencyTargetId = statistic.ProficiencyTargetId,
            ProficiencyRank = statistic.ProficiencyRank,
            ProficiencyBonus = statistic.ProficiencyBonus,
            ProficiencySourceGrantIds = statistic.ProficiencySourceGrantIds,
            ItemBonuses = statistic.ItemBonuses.Select( MapBonus ).ToArray(),
            StatusBonuses = statistic.StatusBonuses.Select( MapBonus ).ToArray(),
            CircumstanceBonuses = statistic.CircumstanceBonuses.Select( MapBonus ).ToArray(),
            Total = statistic.Total,
        };
    }

    private static CharacterProficiencyStatisticDto Map(
        DraftCharacter character,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        ProficiencyTarget target,
        AbilityType ability )
    {
        EffectiveProficiency proficiency = proficiencies
            .Single( item => item.Target.Id == target.Id );
        ProficiencyBasedStatistic statistic = ProficiencyBasedStatistic.Calculate(
            ability,
            character.AbilityScores.GetCharacteristic( ability ),
            proficiency,
            CharacterLevel );

        return new CharacterProficiencyStatisticDto
        {
            TargetId = target.Id,
            Name = target.Name,
            Ability = statistic.Ability,
            AbilityModifier = statistic.AbilityModifier,
            ProficiencyRank = statistic.ProficiencyRank,
            ProficiencyBonus = statistic.ProficiencyBonus,
            Total = statistic.Total,
            SourceGrantIds = statistic.SourceGrantIds,
        };
    }
}
