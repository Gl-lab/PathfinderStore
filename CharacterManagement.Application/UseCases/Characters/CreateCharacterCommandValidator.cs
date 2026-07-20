using FluentValidation;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
{
    public CreateCharacterCommandValidator()
    {
        RuleFor( command => command.UserId )
            .GreaterThan( 0 );

        RuleFor( command => command.Character )
            .NotNull();

        When(
            command => command.Character is not null,
            () =>
            {
                RuleFor( command => command.Character.Name )
                    .NotEmpty();

                RuleFor( command => command.Character.Gender )
                    .Must( gender =>
                        ( gender == CharacterGender.Male ) ||
                        ( gender == CharacterGender.Female ) )
                    .WithMessage( "Character gender must be Male or Female." );

                RuleFor( command => command.Character.AncestryType )
                    .NotEqual( AncestryType.None );

                RuleFor( command => command.Character.HeritageId )
                    .NotEmpty();

                RuleFor( command => command.Character.AncestryFeatId )
                    .NotEmpty();

                RuleFor( command => command.Character.FreeBoosts )
                    .NotNull();

                RuleFor( command => command.Character.BackgroundId )
                    .NotEmpty();

                RuleFor( command => command.Character.BackgroundRestrictedBoost )
                    .NotNull();

                RuleFor( command => command.Character.BackgroundFreeBoost )
                    .NotNull();

                RuleFor( command => command.Character.BackgroundTrainingChoices )
                    .NotNull();

                RuleFor( command => command.Character.ClassId )
                    .NotEmpty();

                RuleFor( command => command.Character.ClassKeyAbility )
                    .NotNull();

                RuleFor( command => command.Character.RogueTrainingChoices )
                    .NotNull();

                When(
                    command => command.Character.ClassId == "class.rogue",
                    () => RuleFor( command => command.Character.RogueRacketId ).NotEmpty() );

                When(
                    command => command.Character.ClassId != "class.rogue",
                    () =>
                    {
                        RuleFor( command => command.Character.RogueRacketId )
                            .Empty();
                        RuleFor( command => command.Character.RogueTrainingChoices )
                            .Empty();
                    } );

                When(
                    command => command.Character.ClassId == "class.ranger",
                    () => RuleFor( command => command.Character.HuntersEdgeId ).NotEmpty() );

                When(
                    command => command.Character.ClassId != "class.ranger",
                    () => RuleFor( command => command.Character.HuntersEdgeId ).Empty() );

                When(
                    command => command.Character.ClassId == "class.druid",
                    () => RuleFor( command => command.Character.DruidicOrderId ).NotEmpty() );

                When(
                    command => command.Character.ClassId != "class.druid",
                    () => RuleFor( command => command.Character.DruidicOrderId ).Empty() );

                When(
                    command => command.Character.ClassId == "class.druid",
                    () =>
                    {
                        RuleFor( command => command.Character.DruidCantripIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 5 );
                        RuleFor( command => command.Character.DruidPreparedSpellIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 2 );
                    } );

                When(
                    command => command.Character.ClassId != "class.druid",
                    () =>
                    {
                        RuleFor( command => command.Character.DruidCantripIds ).Empty();
                        RuleFor( command => command.Character.DruidPreparedSpellIds ).Empty();
                    } );

                When(
                    command => command.Character.ClassId == "class.bard",
                    () => RuleFor( command => command.Character.BardMuseId ).NotEmpty() );

                When(
                    command => command.Character.ClassId != "class.bard",
                    () => RuleFor( command => command.Character.BardMuseId ).Empty() );

                When(
                    command => command.Character.ClassId == "class.bard",
                    () =>
                    {
                        RuleFor( command => command.Character.BardCantripIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 5 )
                            .WithMessage( "Exactly 5 Bard cantrips must be selected." );
                        RuleFor( command => command.Character.BardSpellIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 2 )
                            .WithMessage( "Exactly 2 Bard rank-1 spells must be selected." );
                    } );

                When(
                    command => command.Character.ClassId != "class.bard",
                    () =>
                    {
                        RuleFor( command => command.Character.BardCantripIds ).Empty();
                        RuleFor( command => command.Character.BardSpellIds ).Empty();
                    } );

                When(
                    command => command.Character.ClassId == "class.witch",
                    () =>
                    {
                        RuleFor( command => command.Character.WitchPatronId ).NotEmpty();
                        RuleFor( command => command.Character.WitchFamiliarCantripIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 10 );
                        RuleFor( command => command.Character.WitchFamiliarSpellIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 5 );
                        RuleFor( command => command.Character.WitchPreparedCantripIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 5 );
                        RuleFor( command => command.Character.WitchPreparedSpellIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 2 );
                        RuleFor( command => command.Character.WitchFocusHexId ).NotEmpty();
                    } );

                When(
                    command => command.Character.ClassId != "class.witch",
                    () =>
                    {
                        RuleFor( command => command.Character.WitchPatronId ).Empty();
                        RuleFor( command => command.Character.WitchPatronFamiliarSpellId ).Empty();
                        RuleFor( command => command.Character.WitchFamiliarCantripIds ).Empty();
                        RuleFor( command => command.Character.WitchFamiliarSpellIds ).Empty();
                        RuleFor( command => command.Character.WitchPreparedCantripIds ).Empty();
                        RuleFor( command => command.Character.WitchPreparedSpellIds ).Empty();
                        RuleFor( command => command.Character.WitchFocusHexId ).Empty();
                    } );

                When(
                    command => command.Character.ClassId == "class.wizard",
                    () =>
                    {
                        RuleFor( command => command.Character.ArcaneSchoolId ).NotEmpty();
                        RuleFor( command => command.Character.WizardSpellbookCantripIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 10 );
                        RuleFor( command => command.Character.WizardSpellbookSpellIds )
                            .NotNull()
                            .Must( spellIds => ( spellIds.Count == 5 ) || ( spellIds.Count == 6 ) );
                        RuleFor( command => command.Character.WizardPreparedCantripIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 5 );
                        RuleFor( command => command.Character.WizardPreparedSpellIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 2 );
                    } );

                When(
                    command => command.Character.ClassId != "class.wizard",
                    () =>
                    {
                        RuleFor( command => command.Character.ArcaneSchoolId ).Empty();
                        RuleFor( command => command.Character.WizardSpellbookCantripIds ).Empty();
                        RuleFor( command => command.Character.WizardSpellbookSpellIds ).Empty();
                        RuleFor( command => command.Character.WizardCurriculumCantripId ).Empty();
                        RuleFor( command => command.Character.WizardCurriculumSpellIds ).Empty();
                        RuleFor( command => command.Character.WizardPreparedCantripIds ).Empty();
                        RuleFor( command => command.Character.WizardPreparedSpellIds ).Empty();
                        RuleFor( command => command.Character.WizardPreparedCurriculumCantripId ).Empty();
                        RuleFor( command => command.Character.WizardPreparedCurriculumSpellId ).Empty();
                    } );

                When(
                    command => command.Character.ClassId == "class.wizard",
                    () => RuleFor( command => command.Character.ArcaneThesisId ).NotEmpty() );

                When(
                    command => command.Character.ClassId != "class.wizard",
                    () => RuleFor( command => command.Character.ArcaneThesisId ).Empty() );

                When(
                    command => command.Character.ClassId == "class.cleric",
                    () =>
                    {
                        RuleFor( command => command.Character.ClericDoctrineId ).NotEmpty();
                        RuleFor( command => command.Character.DeityId ).NotEmpty();
                        RuleFor( command => command.Character.DivineFont ).NotNull();
                        RuleFor( command => command.Character.ClericCantripIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 5 )
                            .WithMessage( "Exactly 5 Cleric cantrips must be selected." );
                        RuleFor( command => command.Character.ClericPreparedSpellIds )
                            .NotNull()
                            .Must( spellIds => spellIds.Count == 2 )
                            .WithMessage( "Exactly 2 Cleric spell slots must be prepared." );
                    } );

                When(
                    command => command.Character.ClassId == "class.cleric" &&
                               command.Character.ClericDoctrineId == "cleric_doctrine.cloistered",
                    () => RuleFor( command => command.Character.ClericDomainId ).NotEmpty() );

                When(
                    command => command.Character.ClassId != "class.cleric" ||
                               command.Character.ClericDoctrineId != "cleric_doctrine.cloistered",
                    () => RuleFor( command => command.Character.ClericDomainId ).Empty() );

                When(
                    command => command.Character.ClassId != "class.cleric",
                    () =>
                    {
                        RuleFor( command => command.Character.ClericDoctrineId ).Empty();
                        RuleFor( command => command.Character.DeityId ).Empty();
                        RuleFor( command => command.Character.DivineFont ).Empty();
                        RuleFor( command => command.Character.DivineSanctification ).Empty();
                        RuleFor( command => command.Character.DeitySkillReplacementId ).Empty();
                        RuleFor( command => command.Character.ClericDomainId ).Empty();
                        RuleFor( command => command.Character.ClericCantripIds ).Empty();
                        RuleFor( command => command.Character.ClericPreparedSpellIds ).Empty();
                    } );

                RuleFor( command => command.Character.FinalFreeBoosts )
                    .Cascade( CascadeMode.Stop )
                    .NotNull()
                    .Must( boosts => boosts?.Count == 4 )
                    .WithMessage( "Exactly 4 final free boosts must be selected." )
                    .Must( boosts => boosts is not null && boosts.Distinct().Count() == boosts.Count )
                    .WithMessage( "Final free boosts must target different abilities." )
                    .Must( boosts => boosts is not null && boosts.All( Enum.IsDefined ) )
                    .WithMessage( "Final free boosts contain an unknown ability type." );

                RuleFor( command => command.Character.ClassSkillGrantChoices )
                    .NotNull();

                RuleFor( command => command.Character.AdditionalClassTrainingChoices )
                    .NotNull();
            } );
    }
}
