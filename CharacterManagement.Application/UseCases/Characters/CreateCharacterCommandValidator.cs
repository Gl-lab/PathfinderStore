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
                    command => command.Character.ClassId == "class.cleric",
                    () =>
                    {
                        RuleFor( command => command.Character.ClericDoctrineId ).NotEmpty();
                        RuleFor( command => command.Character.DeityId ).NotEmpty();
                        RuleFor( command => command.Character.DivineFont ).NotNull();
                    } );

                When(
                    command => command.Character.ClassId != "class.cleric",
                    () =>
                    {
                        RuleFor( command => command.Character.ClericDoctrineId ).Empty();
                        RuleFor( command => command.Character.DeityId ).Empty();
                        RuleFor( command => command.Character.DivineFont ).Empty();
                        RuleFor( command => command.Character.DivineSanctification ).Empty();
                        RuleFor( command => command.Character.DeitySkillReplacementId ).Empty();
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
