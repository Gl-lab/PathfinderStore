using FluentValidation;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class CreateCharacterCommandValidatorTests
{
    [Fact]
    public void Validate_WhenUserIdIsNotPositive_ThrowsValidationException()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterCommand command = new CreateCharacterCommand(
            0,
            new CreateCharacterRequestDto
            {
                Name = "Thorin",
                AncestryType = AncestryType.Human,
                HeritageId = "human.skilled",
                AncestryFeatId = "human.cooperative_nature",
                FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
                BackgroundId = "background.acrobat",
                BackgroundRestrictedBoost = AbilityType.Dexterity,
                BackgroundFreeBoost = AbilityType.Charisma,
                ClassId = "class.fighter",
                ClassKeyAbility = AbilityType.Strength,
                FinalFreeBoosts =
                [
                    AbilityType.Strength,
                    AbilityType.Dexterity,
                    AbilityType.Constitution,
                    AbilityType.Wisdom
                ],
            } );

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow( command ) );
    }

    [Fact]
    public void Validate_WhenCharacterNameIsEmpty_ThrowsValidationException()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterCommand command = new CreateCharacterCommand(
            42,
            new CreateCharacterRequestDto
            {
                Name = "",
                AncestryType = AncestryType.Human,
                FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
            } );

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow( command ) );
    }

    [Fact]
    public void Validate_WhenCommandIsValid_DoesNotThrow()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterCommand command = new CreateCharacterCommand(
            42,
            new CreateCharacterRequestDto
            {
                Name = "Thorin",
                AncestryType = AncestryType.Human,
                HeritageId = "human.skilled",
                AncestryFeatId = "human.cooperative_nature",
                FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
                BackgroundId = "background.acrobat",
                BackgroundRestrictedBoost = AbilityType.Dexterity,
                BackgroundFreeBoost = AbilityType.Charisma,
                ClassId = "class.fighter",
                ClassKeyAbility = AbilityType.Strength,
                FinalFreeBoosts =
                [
                    AbilityType.Strength,
                    AbilityType.Dexterity,
                    AbilityType.Constitution,
                    AbilityType.Wisdom
                ],
            } );

        validator.ValidateAndThrow( command );
    }

    [Fact]
    public void Validate_WhenBackgroundIdIsEmpty_ThrowsValidationException()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterCommand command = new CreateCharacterCommand(
            42,
            new CreateCharacterRequestDto
            {
                Name = "Thorin",
                AncestryType = AncestryType.Human,
                HeritageId = "human.skilled",
                AncestryFeatId = "human.cooperative_nature",
                FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
                BackgroundId = String.Empty,
            } );

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow( command ) );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public void Validate_WhenBackgroundBoostIsMissing_ThrowsValidationException( bool omitRestrictedBoost )
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = new CreateCharacterRequestDto
        {
            Name = "Thorin",
            AncestryType = AncestryType.Human,
            HeritageId = "human.skilled",
            AncestryFeatId = "human.cooperative_nature",
            FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
            BackgroundId = "background.acrobat",
            BackgroundRestrictedBoost = omitRestrictedBoost ? null : AbilityType.Dexterity,
            BackgroundFreeBoost = omitRestrictedBoost ? AbilityType.Charisma : null,
        };
        CreateCharacterCommand command = new CreateCharacterCommand( 42, character );

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow( command ) );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public void Validate_WhenClassChoiceFieldIsMissing_ThrowsValidationException( bool omitClassId )
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterCommand command = new CreateCharacterCommand(
            42,
            new CreateCharacterRequestDto
            {
                Name = "Thorin",
                AncestryType = AncestryType.Human,
                HeritageId = "human.skilled",
                AncestryFeatId = "human.cooperative_nature",
                FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
                BackgroundId = "background.acrobat",
                BackgroundRestrictedBoost = AbilityType.Dexterity,
                BackgroundFreeBoost = AbilityType.Charisma,
                ClassId = omitClassId ? String.Empty : "class.fighter",
                ClassKeyAbility = omitClassId ? AbilityType.Strength : null,
            } );

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow( command ) );
    }

    [Theory]
    [MemberData( nameof( InvalidFinalFreeBoosts ) )]
    public void Validate_WhenFinalFreeBoostsAreInvalid_ThrowsValidationException(
        IReadOnlyList<AbilityType>? finalFreeBoosts )
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterCommand command = new CreateCharacterCommand(
            42,
            new CreateCharacterRequestDto
            {
                Name = "Thorin",
                AncestryType = AncestryType.Human,
                HeritageId = "human.skilled",
                AncestryFeatId = "human.cooperative_nature",
                FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
                BackgroundId = "background.acrobat",
                BackgroundRestrictedBoost = AbilityType.Dexterity,
                BackgroundFreeBoost = AbilityType.Charisma,
                ClassId = "class.fighter",
                ClassKeyAbility = AbilityType.Strength,
                FinalFreeBoosts = finalFreeBoosts,
            } );

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow( command ) );
    }

    [Theory]
    [InlineData( "class.cleric", null )]
    [InlineData( "class.fighter", "cleric_doctrine.warpriest" )]
    public void Validate_WhenClericDoctrineDoesNotMatchClass_ThrowsValidationException(
        string classId,
        string? clericDoctrineId )
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        character.ClassId = classId;
        character.ClassKeyAbility = classId == "class.cleric"
            ? AbilityType.Wisdom
            : AbilityType.Strength;
        character.ClericDoctrineId = clericDoctrineId;
        character.DeityId = classId == "class.cleric" ? "deity.iomedae" : null;
        character.DivineFont = classId == "class.cleric" ? DivineFont.Heal : null;
        character.DivineSanctification = classId == "class.cleric"
            ? DivineSanctification.Holy
            : null;

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow(
            new CreateCharacterCommand( 42, character ) ) );
    }

    [Fact]
    public void Validate_WhenClericDoctrineIsSelected_DoesNotThrow()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        character.ClassId = "class.cleric";
        character.ClassKeyAbility = AbilityType.Wisdom;
        character.ClericDoctrineId = "cleric_doctrine.cloistered";
        character.DeityId = "deity.iomedae";
        character.DivineFont = DivineFont.Heal;
        character.DivineSanctification = DivineSanctification.Holy;

        validator.ValidateAndThrow( new CreateCharacterCommand( 42, character ) );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public void Validate_WhenClassTrainingCollectionIsNull_ThrowsValidationException(
        bool omitGrantChoices )
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        if ( omitGrantChoices )
        {
            character.ClassSkillGrantChoices = null!;
        }
        else
        {
            character.AdditionalClassTrainingChoices = null!;
        }

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow(
            new CreateCharacterCommand( 42, character ) ) );
    }

    [Theory]
    [InlineData( "class.ranger", null )]
    [InlineData( "class.fighter", "hunters_edge.flurry" )]
    public void Validate_WhenHuntersEdgeDoesNotMatchClass_ThrowsValidationException(
        string classId,
        string? huntersEdgeId )
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        character.ClassId = classId;
        character.ClassKeyAbility = AbilityType.Strength;
        character.HuntersEdgeId = huntersEdgeId;

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow(
            new CreateCharacterCommand( 42, character ) ) );
    }

    [Fact]
    public void Validate_WhenRangerHasHuntersEdge_DoesNotThrow()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        character.ClassId = "class.ranger";
        character.ClassKeyAbility = AbilityType.Dexterity;
        character.HuntersEdgeId = "hunters_edge.flurry";

        validator.ValidateAndThrow( new CreateCharacterCommand( 42, character ) );
    }

    [Theory]
    [InlineData( "class.druid", null )]
    [InlineData( "class.fighter", "druidic_order.animal" )]
    public void Validate_WhenDruidicOrderDoesNotMatchClass_ThrowsValidationException(
        string classId,
        string? druidicOrderId )
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        character.ClassId = classId;
        character.ClassKeyAbility = classId == "class.druid"
            ? AbilityType.Wisdom
            : AbilityType.Strength;
        character.DruidicOrderId = druidicOrderId;

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow(
            new CreateCharacterCommand( 42, character ) ) );
    }

    [Fact]
    public void Validate_WhenDruidHasOrder_DoesNotThrow()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        character.ClassId = "class.druid";
        character.ClassKeyAbility = AbilityType.Wisdom;
        character.DruidicOrderId = "druidic_order.animal";

        validator.ValidateAndThrow( new CreateCharacterCommand( 42, character ) );
    }

    [Theory]
    [InlineData( "class.bard", null )]
    [InlineData( "class.fighter", "bard_muse.enigma" )]
    public void Validate_WhenBardMuseDoesNotMatchClass_ThrowsValidationException(
        string classId,
        string? bardMuseId )
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        character.ClassId = classId;
        character.ClassKeyAbility = classId == "class.bard"
            ? AbilityType.Charisma
            : AbilityType.Strength;
        character.BardMuseId = bardMuseId;

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow(
            new CreateCharacterCommand( 42, character ) ) );
    }

    [Fact]
    public void Validate_WhenBardHasMuse_DoesNotThrow()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        character.ClassId = "class.bard";
        character.ClassKeyAbility = AbilityType.Charisma;
        character.BardMuseId = "bard_muse.enigma";

        validator.ValidateAndThrow( new CreateCharacterCommand( 42, character ) );
    }

    [Theory]
    [InlineData( "class.witch", null, null )]
    [InlineData( "class.fighter", "witch_patron.resentment", null )]
    [InlineData( "class.fighter", null, "spell.enfeeble" )]
    public void Validate_WhenWitchPatronDoesNotMatchClass_ThrowsValidationException(
        string classId,
        string? patronId,
        string? familiarSpellId )
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        character.ClassId = classId;
        character.ClassKeyAbility = classId == "class.witch"
            ? AbilityType.Intelligence
            : AbilityType.Strength;
        character.WitchPatronId = patronId;
        character.WitchPatronFamiliarSpellId = familiarSpellId;

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow(
            new CreateCharacterCommand( 42, character ) ) );
    }

    [Fact]
    public void Validate_WhenWitchHasPatron_AllowsCatalogAwareNestedValidation()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        character.ClassId = "class.witch";
        character.ClassKeyAbility = AbilityType.Intelligence;
        character.WitchPatronId = "witch_patron.wilding_steward";
        character.WitchPatronFamiliarSpellId = "spell.summon_animal";

        validator.ValidateAndThrow( new CreateCharacterCommand( 42, character ) );
    }

    [Theory]
    [InlineData( "class.wizard", null )]
    [InlineData( "class.fighter", "arcane_school.mentalism" )]
    public void Validate_WhenArcaneSchoolDoesNotMatchClass_ThrowsValidationException(
        string classId,
        string? arcaneSchoolId )
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        character.ClassId = classId;
        character.ClassKeyAbility = classId == "class.wizard"
            ? AbilityType.Intelligence
            : AbilityType.Strength;
        character.ArcaneSchoolId = arcaneSchoolId;

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow(
            new CreateCharacterCommand( 42, character ) ) );
    }

    [Fact]
    public void Validate_WhenWizardHasArcaneSchool_DoesNotThrow()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = CreateValidRequest();
        character.ClassId = "class.wizard";
        character.ClassKeyAbility = AbilityType.Intelligence;
        character.ArcaneSchoolId = "arcane_school.mentalism";

        validator.ValidateAndThrow( new CreateCharacterCommand( 42, character ) );
    }

    private static CreateCharacterRequestDto CreateValidRequest()
    {
        return new CreateCharacterRequestDto
        {
            Name = "Thorin",
            AncestryType = AncestryType.Human,
            HeritageId = "human.skilled",
            AncestryFeatId = "human.cooperative_nature",
            FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
            BackgroundId = "background.acrobat",
            BackgroundRestrictedBoost = AbilityType.Dexterity,
            BackgroundFreeBoost = AbilityType.Charisma,
            ClassId = "class.fighter",
            ClassKeyAbility = AbilityType.Strength,
            FinalFreeBoosts =
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom,
            ],
        };
    }

    public static IEnumerable<object?[]> InvalidFinalFreeBoosts()
    {
        yield return [ null ];
        yield return
        [
            new AbilityType[]
            {
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution
            }
        ];
        yield return
        [
            new AbilityType[]
            {
                AbilityType.Strength,
                AbilityType.Strength,
                AbilityType.Constitution,
                AbilityType.Wisdom
            }
        ];
        yield return
        [
            new AbilityType[]
            {
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                ( AbilityType )999
            }
        ];
    }
}
