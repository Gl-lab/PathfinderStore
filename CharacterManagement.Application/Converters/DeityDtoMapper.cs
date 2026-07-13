using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class DeityDtoMapper
{
    public static DeityDto Map( Deity deity )
    {
        ArgumentNullException.ThrowIfNull( deity );

        return new DeityDto
        {
            Id = deity.Id,
            Name = deity.Name,
            Source = new SourceReferenceDto
            {
                Book = deity.Source.Book,
                Page = deity.Source.Page,
            },
            CanGrantClericPowers = deity.CanGrantClericPowers,
            DivineSkillId = deity.DivineSkillId,
            FavoredWeapons = deity.FavoredWeapons.Select( Map ).ToList(),
            DivineFontOptions = deity.DivineFontOptions.ToList(),
            SanctificationOptions = deity.SanctificationOptions.ToList(),
            RequiredSanctification = deity.RequiredSanctification,
            PrimaryDomainIds = deity.PrimaryDomainIds.ToList(),
            GrantedSpells = deity.GrantedSpells.Select( Map ).ToList(),
        };
    }

    public static DeityPackageDto MapPackage( DraftCharacter character, Deity deity )
    {
        ArgumentNullException.ThrowIfNull( character );
        ArgumentNullException.ThrowIfNull( deity );

        DivineFont divineFont = character.SelectedDivineFont
            ?? throw new InvalidOperationException( "Selected deity package does not define a Divine Font." );

        string? replacementId = character.TrainedSkills
            .FirstOrDefault( training => training.SourceGrantId == deity.DivineSkillGrantId )
            ?.SkillId;
        if ( replacementId == deity.DivineSkillId )
        {
            replacementId = null;
        }

        return new DeityPackageDto
        {
            Id = deity.Id,
            Name = deity.Name,
            DivineSkillId = deity.DivineSkillId!,
            DivineSkillReplacementId = replacementId,
            FavoredWeapons = deity.FavoredWeapons.Select( Map ).ToList(),
            DivineFont = divineFont,
            Sanctification = character.SelectedDivineSanctification,
            PrimaryDomainIds = deity.PrimaryDomainIds.ToList(),
            GrantedSpells = deity.GrantedSpells.Select( Map ).ToList(),
        };
    }

    private static DeityFavoredWeaponDto Map( DeityFavoredWeapon weapon )
    {
        return new DeityFavoredWeaponDto
        {
            Id = weapon.Id,
            Name = weapon.Name,
            Category = weapon.Category,
        };
    }

    private static DeityGrantedSpellDto Map( DeityGrantedSpell spell )
    {
        return new DeityGrantedSpellDto
        {
            Rank = spell.Rank,
            Id = spell.Id,
            Name = spell.Name,
        };
    }
}
