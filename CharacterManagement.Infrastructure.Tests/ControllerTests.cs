using System.Security.Claims;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.UseCases.Ancestries;
using Pathfinder.CharacterManagement.Application.UseCases.Backgrounds;
using Pathfinder.CharacterManagement.Application.UseCases.CharacterClasses;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Application.UseCases.Skills;
using Pathfinder.CharacterManagement.Application.UseCases.HuntersEdges;
using Pathfinder.CharacterManagement.Application.UseCases.DruidicOrders;
using Pathfinder.CharacterManagement.Application.UseCases.BardMuses;
using Pathfinder.CharacterManagement.Application.UseCases.WitchPatrons;
using Pathfinder.CharacterManagement.Application.UseCases.ArcaneSchools;
using Pathfinder.CharacterManagement.Application.UseCases.ArcaneTheses;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;
using Pathfinder.Web.Controllers;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class ControllerTests
{
    [Fact]
    public async Task AncestriesController_Get_ReturnsOkWithPayload()
    {
        IReadOnlyCollection<AncestryDto> expected =
        [
            new AncestryDto
            {
                Type = AncestryType.Human,
                Name = "Human",
                AbilityBoosts =
                [
                    new AncestryBoostDto
                    {
                        IsFree = true,
                    },
                ],
                AbilityFlaws = [ ],
            },
        ];
        TestMediator mediator = new TestMediator();
        mediator.Register( new GetAncestriesCommand(), expected );
        AncestriesController controller = new AncestriesController( mediator );

        ActionResult<IReadOnlyCollection<AncestryDto>> actionResult = await controller.Get();

        OkObjectResult okResult = Assert.IsType<OkObjectResult>( actionResult.Result );
        IReadOnlyCollection<AncestryDto> payload = Assert.IsAssignableFrom<IReadOnlyCollection<AncestryDto>>( okResult.Value );
        Assert.Same( expected, payload );
    }

    [Fact]
    public async Task BackgroundsController_Get_ReturnsOkWithPayload()
    {
        IReadOnlyCollection<BackgroundDto> expected =
        [
            new BackgroundDto
            {
                Id = "background.acrobat",
                Name = "Acrobat",
                RestrictedBoostOptions = [ AbilityType.Strength, AbilityType.Dexterity ],
                FreeBoostCount = 1,
                Grants = [],
            },
        ];
        TestMediator mediator = new TestMediator();
        mediator.Register( new GetBackgroundsCommand(), expected );
        BackgroundsController controller = new BackgroundsController( mediator );

        ActionResult<IReadOnlyCollection<BackgroundDto>> actionResult = await controller.Get();

        OkObjectResult okResult = Assert.IsType<OkObjectResult>( actionResult.Result );
        IReadOnlyCollection<BackgroundDto> payload = Assert.IsAssignableFrom<IReadOnlyCollection<BackgroundDto>>( okResult.Value );
        Assert.Same( expected, payload );
    }

    [Fact]
    public async Task ClassesController_Get_ReturnsOkWithPayload()
    {
        IReadOnlyCollection<CharacterClassDto> expected =
        [
            new CharacterClassDto
            {
                Id = "class.fighter",
                Name = "Fighter",
                BaseHitPoints = 10,
                KeyAbilityOptions = [ AbilityType.Strength, AbilityType.Dexterity ],
            },
        ];
        TestMediator mediator = new TestMediator();
        mediator.Register( new GetCharacterClassesCommand(), expected );
        ClassesController controller = new ClassesController( mediator );

        ActionResult<IReadOnlyCollection<CharacterClassDto>> actionResult = await controller.Get();

        OkObjectResult okResult = Assert.IsType<OkObjectResult>( actionResult.Result );
        IReadOnlyCollection<CharacterClassDto> payload = Assert.IsAssignableFrom<IReadOnlyCollection<CharacterClassDto>>( okResult.Value );
        Assert.Same( expected, payload );
    }

    [Fact]
    public async Task ClassesController_GetHuntersEdges_ReturnsOkWithPayload()
    {
        IReadOnlyCollection<HuntersEdgeDto> expected =
        [
            new HuntersEdgeDto
            {
                Id = "hunters_edge.precision",
                Name = "Precision",
            },
        ];
        TestMediator mediator = new TestMediator();
        mediator.Register( new GetHuntersEdgesCommand(), expected );
        ClassesController controller = new ClassesController( mediator );

        ActionResult<IReadOnlyCollection<HuntersEdgeDto>> actionResult = await controller.GetHuntersEdges();

        OkObjectResult okResult = Assert.IsType<OkObjectResult>( actionResult.Result );
        IReadOnlyCollection<HuntersEdgeDto> payload =
            Assert.IsAssignableFrom<IReadOnlyCollection<HuntersEdgeDto>>( okResult.Value );
        Assert.Same( expected, payload );
    }

    [Fact]
    public async Task ClassesController_GetDruidicOrders_ReturnsOkWithPayload()
    {
        IReadOnlyCollection<DruidicOrderDto> expected =
        [
            new DruidicOrderDto
            {
                Id = "druidic_order.animal",
                Name = "Animal",
            },
        ];
        TestMediator mediator = new TestMediator();
        mediator.Register( new GetDruidicOrdersCommand(), expected );
        ClassesController controller = new ClassesController( mediator );

        ActionResult<IReadOnlyCollection<DruidicOrderDto>> actionResult =
            await controller.GetDruidicOrders();

        OkObjectResult okResult = Assert.IsType<OkObjectResult>( actionResult.Result );
        IReadOnlyCollection<DruidicOrderDto> payload =
            Assert.IsAssignableFrom<IReadOnlyCollection<DruidicOrderDto>>( okResult.Value );
        Assert.Same( expected, payload );
    }

    [Fact]
    public async Task ClassesController_GetBardMuses_ReturnsOkWithPayload()
    {
        IReadOnlyCollection<BardMuseDto> expected =
        [
            new BardMuseDto
            {
                Id = "bard_muse.enigma",
                Name = "Enigma",
            },
        ];
        TestMediator mediator = new TestMediator();
        mediator.Register( new GetBardMusesCommand(), expected );
        ClassesController controller = new ClassesController( mediator );

        ActionResult<IReadOnlyCollection<BardMuseDto>> actionResult =
            await controller.GetBardMuses();

        OkObjectResult okResult = Assert.IsType<OkObjectResult>( actionResult.Result );
        IReadOnlyCollection<BardMuseDto> payload =
            Assert.IsAssignableFrom<IReadOnlyCollection<BardMuseDto>>( okResult.Value );
        Assert.Same( expected, payload );
    }

    [Fact]
    public async Task ClassesController_GetWitchPatrons_ReturnsOkWithPayload()
    {
        IReadOnlyCollection<WitchPatronDto> expected =
        [
            new WitchPatronDto
            {
                Id = "witch_patron.resentment",
                Name = "The Resentment",
            },
        ];
        TestMediator mediator = new TestMediator();
        mediator.Register( new GetWitchPatronsCommand(), expected );
        ClassesController controller = new ClassesController( mediator );

        ActionResult<IReadOnlyCollection<WitchPatronDto>> actionResult =
            await controller.GetWitchPatrons();

        OkObjectResult okResult = Assert.IsType<OkObjectResult>( actionResult.Result );
        IReadOnlyCollection<WitchPatronDto> payload =
            Assert.IsAssignableFrom<IReadOnlyCollection<WitchPatronDto>>( okResult.Value );
        Assert.Same( expected, payload );
    }

    [Fact]
    public async Task ClassesController_GetArcaneSchools_ReturnsOkWithPayload()
    {
        IReadOnlyCollection<ArcaneSchoolDto> expected =
        [
            new ArcaneSchoolDto
            {
                Id = "arcane_school.mentalism",
                Name = "School of Mentalism",
            },
        ];
        TestMediator mediator = new TestMediator();
        mediator.Register( new GetArcaneSchoolsCommand(), expected );
        ClassesController controller = new ClassesController( mediator );

        ActionResult<IReadOnlyCollection<ArcaneSchoolDto>> actionResult =
            await controller.GetArcaneSchools();

        OkObjectResult okResult = Assert.IsType<OkObjectResult>( actionResult.Result );
        IReadOnlyCollection<ArcaneSchoolDto> payload =
            Assert.IsAssignableFrom<IReadOnlyCollection<ArcaneSchoolDto>>( okResult.Value );
        Assert.Same( expected, payload );
    }

    [Fact]
    public async Task ClassesController_GetArcaneTheses_ReturnsOkWithPayload()
    {
        IReadOnlyCollection<ArcaneThesisDto> expected =
        [
            new ArcaneThesisDto
            {
                Id = "arcane_thesis.spell_substitution",
                Name = "Spell Substitution",
            },
        ];
        TestMediator mediator = new TestMediator();
        mediator.Register( new GetArcaneThesesCommand(), expected );
        ClassesController controller = new ClassesController( mediator );

        ActionResult<IReadOnlyCollection<ArcaneThesisDto>> actionResult =
            await controller.GetArcaneTheses();

        OkObjectResult okResult = Assert.IsType<OkObjectResult>( actionResult.Result );
        IReadOnlyCollection<ArcaneThesisDto> payload =
            Assert.IsAssignableFrom<IReadOnlyCollection<ArcaneThesisDto>>( okResult.Value );
        Assert.Same( expected, payload );
    }

    [Fact]
    public async Task SkillsController_Get_ReturnsOkWithPayload()
    {
        IReadOnlyCollection<SkillDto> expected =
        [
            new SkillDto
            {
                Id = "skill.nature",
                Name = "Nature",
                KeyAbility = AbilityType.Wisdom,
            },
        ];
        TestMediator mediator = new TestMediator();
        mediator.Register( new GetSkillsCommand(), expected );
        SkillsController controller = new SkillsController( mediator );

        ActionResult<IReadOnlyCollection<SkillDto>> actionResult = await controller.Get();

        OkObjectResult okResult = Assert.IsType<OkObjectResult>( actionResult.Result );
        IReadOnlyCollection<SkillDto> payload = Assert.IsAssignableFrom<IReadOnlyCollection<SkillDto>>( okResult.Value );
        Assert.Same( expected, payload );
    }

    [Fact]
    public async Task CharacterController_Create_WhenUserIdClaimIsMissing_ReturnsUnauthorized()
    {
        TestMediator mediator = new TestMediator();
        CharacterController controller = CreateCharacterController( mediator );
        CreateCharacterRequestDto request = new CreateCharacterRequestDto
        {
            Name = "Thorin",
            AncestryType = AncestryType.Human,
            FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
        };

        ActionResult actionResult = await controller.Create( request );

        Assert.IsType<UnauthorizedResult>( actionResult );
    }

    [Fact]
    public async Task CharacterController_Create_WhenMediatorThrowsValidationException_ReturnsBadRequest()
    {
        TestMediator mediator = new TestMediator();
        CreateCharacterRequestDto request = new CreateCharacterRequestDto
        {
            Name = String.Empty,
            AncestryType = AncestryType.Human,
            FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
        };
        mediator.RegisterException<CreateCharacterCommand>(
            new ValidationException(
            [
                new ValidationFailure( "Name", "'Name' must not be empty." ),
            ] ) );
        CharacterController controller = CreateCharacterController( mediator, 77 );

        ActionResult actionResult = await controller.Create( request );

        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>( actionResult );
        IReadOnlyCollection<string> errors = Assert.IsAssignableFrom<IReadOnlyCollection<string>>( badRequestResult.Value );
        Assert.Contains( "'Name' must not be empty.", errors );
    }

    [Fact]
    public async Task CharacterController_Create_WhenMediatorThrowsDomainException_ReturnsBadRequest()
    {
        TestMediator mediator = new TestMediator();
        CreateCharacterRequestDto request = new CreateCharacterRequestDto
        {
            Name = "Thorin",
            AncestryType = AncestryType.Human,
            FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
        };
        mediator.RegisterException<CreateCharacterCommand>(
            new Pathfinder.CharacterManagement.Domain.Exceptions.CharacterManagementException(
                "Background restricted boost is not allowed." ) );
        CharacterController controller = CreateCharacterController( mediator, 77 );

        ActionResult actionResult = await controller.Create( request );

        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>( actionResult );
        IReadOnlyCollection<string> errors = Assert.IsAssignableFrom<IReadOnlyCollection<string>>( badRequestResult.Value );
        Assert.Contains( "Background restricted boost is not allowed.", errors );
    }

    [Fact]
    public async Task CharacterController_GetById_WhenMediatorThrowsDomainException_ReturnsNotFound()
    {
        TestMediator mediator = new TestMediator();
        mediator.RegisterException<GetCharacterByIdCommand>(
            new CharacterManagementException( "Character 15 was not found for current user." ) );
        CharacterController controller = CreateCharacterController( mediator, 77 );

        ActionResult<CharacterDto> actionResult = await controller.GetById( 15 );

        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>( actionResult.Result );
        IReadOnlyCollection<string> errors = Assert.IsAssignableFrom<IReadOnlyCollection<string>>( notFoundResult.Value );
        Assert.Contains( "Character 15 was not found for current user.", errors );
    }

    [Fact]
    public async Task CharacterController_Finalize_ReturnsServerCreationState()
    {
        TestMediator mediator = new TestMediator();
        CharacterCreationStateDto expected = new CharacterCreationStateDto
        {
            CreationStatus = CharacterCreationStatus.Completed,
            CompletedAtUtc = DateTimeOffset.UtcNow,
            Completion = new CharacterCompletionDto { IsComplete = true },
        };
        mediator.Register( new FinalizeCharacterCommand( 77, 15 ), expected );
        CharacterController controller = CreateCharacterController( mediator, 77 );

        ActionResult<CharacterCreationStateDto> actionResult = await controller.Finalize( 15 );

        OkObjectResult okResult = Assert.IsType<OkObjectResult>( actionResult.Result );
        Assert.Same( expected, okResult.Value );
    }

    [Fact]
    public async Task CharacterController_ChangeHitPoints_ReturnsServerState()
    {
        TestMediator mediator = new TestMediator();
        CharacterHitPointStateDto expected = new CharacterHitPointStateDto
        {
            Current = 12,
            Temporary = 3,
            Maximum = 18,
        };
        mediator.Register(
            new ChangeHitPointsCommand( 77, 5, 15, HitPointOperation.ApplyDamage, 6 ),
            expected );
        CampaignCharactersController controller = CreateCampaignCharactersController( mediator, 77 );

        ActionResult<CharacterHitPointStateDto> actionResult = await controller.ChangeHitPoints(
            5,
            15,
            new ChangeHitPointsRequestDto
            {
                Operation = HitPointOperation.ApplyDamage,
                Amount = 6,
            } );

        OkObjectResult okResult = Assert.IsType<OkObjectResult>( actionResult.Result );
        Assert.Same( expected, okResult.Value );
    }

    [Fact]
    public async Task CharacterController_Get_WhenMediatorThrowsDbUpdateException_ReturnsServiceUnavailable()
    {
        TestMediator mediator = new TestMediator();
        mediator.RegisterException<GetCharactersCommand>(
            new DbUpdateException( "Database read failed.", new Exception( "inner" ) ) );
        CharacterController controller = CreateCharacterController( mediator, 77 );

        ActionResult<IReadOnlyCollection<CharacterDto>> actionResult = await controller.Get();

        ObjectResult objectResult = Assert.IsType<ObjectResult>( actionResult.Result );
        Assert.Equal( StatusCodes.Status503ServiceUnavailable, objectResult.StatusCode );
        IReadOnlyCollection<string> errors = Assert.IsAssignableFrom<IReadOnlyCollection<string>>( objectResult.Value );
        Assert.Contains( "Character data is temporarily unavailable.", errors );
    }

    [Fact]
    public async Task CharacterController_Delete_WhenMediatorThrowsDomainException_ReturnsNotFound()
    {
        TestMediator mediator = new TestMediator();
        mediator.RegisterException<DeleteCharacterCommand>(
            new CharacterManagementException( "Character 19 was not found for current user." ) );
        CharacterController controller = CreateCharacterController( mediator, 77 );

        ActionResult actionResult = await controller.Delete( 19 );

        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>( actionResult );
        IReadOnlyCollection<string> errors = Assert.IsAssignableFrom<IReadOnlyCollection<string>>( notFoundResult.Value );
        Assert.Contains( "Character 19 was not found for current user.", errors );
    }

    [Fact]
    public async Task CharacterController_SetGender_WithOwnedLegacyCharacter_ReturnsOk()
    {
        TestMediator mediator = new TestMediator();
        CharacterController controller = CreateCharacterController( mediator, 77 );

        ActionResult actionResult = await controller.SetGender(
            19,
            new SetCharacterGenderRequestDto { Gender = CharacterGender.Female } );

        Assert.IsType<OkResult>( actionResult );
    }

    [Fact]
    public async Task CharacterController_SetGender_WhenAlreadySpecified_ReturnsBadRequest()
    {
        TestMediator mediator = new TestMediator();
        mediator.RegisterException<SetCharacterGenderCommand>(
            new Pathfinder.CharacterManagement.Domain.Exceptions.CharacterManagementException(
                "Character gender has already been specified." ) );
        CharacterController controller = CreateCharacterController( mediator, 77 );

        ActionResult actionResult = await controller.SetGender(
            19,
            new SetCharacterGenderRequestDto { Gender = CharacterGender.Female } );

        BadRequestObjectResult badRequestResult =
            Assert.IsType<BadRequestObjectResult>( actionResult );
        IReadOnlyCollection<string> errors =
            Assert.IsAssignableFrom<IReadOnlyCollection<string>>( badRequestResult.Value );
        Assert.Contains( "Character gender has already been specified.", errors );
    }

    private static CharacterController CreateCharacterController( IMediator mediator, int? userId = null )
    {
        CharacterController controller = new CharacterController(
            mediator,
            NullLogger<CharacterController>.Instance );
        ClaimsIdentity identity = userId.HasValue
            ? new ClaimsIdentity(
                [
                    new Claim( ClaimTypes.NameIdentifier, userId.Value.ToString() ),
                ],
                "TestAuthenticationType" )
            : new ClaimsIdentity();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal( identity ),
            },
        };

        return controller;
    }

    private static CampaignCharactersController CreateCampaignCharactersController(
        IMediator mediator,
        int? userId = null )
    {
        CampaignCharactersController controller = new CampaignCharactersController(
            mediator,
            NullLogger<CampaignCharactersController>.Instance );
        ClaimsIdentity identity = userId.HasValue
            ? new ClaimsIdentity(
                [
                    new Claim( ClaimTypes.NameIdentifier, userId.Value.ToString() ),
                ],
                "TestAuthenticationType" )
            : new ClaimsIdentity();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal( identity ),
            },
        };
        return controller;
    }

    private sealed class TestMediator : IMediator
    {
        private readonly Dictionary<Type, object?> _responses = new();
        private readonly Dictionary<Type, Exception> _exceptions = new();

        public void Register<TRequest, TResponse>( TRequest request, TResponse response )
            where TRequest : IRequest<TResponse>
        {
            _responses[ request.GetType() ] = response;
        }

        public void RegisterException<TRequest>( Exception exception )
        {
            _exceptions[ typeof( TRequest ) ] = exception;
        }

        public Task Publish( object notification, CancellationToken cancellationToken = default ) => Task.CompletedTask;

        public Task Publish<TNotification>( TNotification notification, CancellationToken cancellationToken = default )
            where TNotification : INotification
        {
            return Task.CompletedTask;
        }

        public Task<TResponse> Send<TResponse>( IRequest<TResponse> request, CancellationToken cancellationToken = default )
        {
            Type requestType = request.GetType();
            if ( _exceptions.TryGetValue( requestType, out Exception? exception ) )
            {
                throw exception;
            }

            if ( !_responses.TryGetValue( requestType, out object? response ) )
            {
                throw new InvalidOperationException( $"No response registered for {requestType.Name}." );
            }

            return Task.FromResult( ( TResponse )response! );
        }

        public Task Send( IRequest request, CancellationToken cancellationToken = default )
        {
            Type requestType = request.GetType();
            if ( _exceptions.TryGetValue( requestType, out Exception? exception ) )
            {
                throw exception;
            }

            return Task.CompletedTask;
        }

        public Task<object?> Send( object request, CancellationToken cancellationToken = default )
        {
            Type requestType = request.GetType();
            if ( _exceptions.TryGetValue( requestType, out Exception? exception ) )
            {
                throw exception;
            }

            return _responses.TryGetValue( requestType, out object? response )
                ? Task.FromResult( response )
                : Task.FromResult<object?>( null );
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default )
        {
            throw new NotSupportedException( "Streaming is not used in these controller tests." );
        }

        Task ISender.Send<TRequest>( TRequest request, CancellationToken cancellationToken )
        {
            return Send( ( IRequest )request!, cancellationToken );
        }

        public IAsyncEnumerable<object?> CreateStream( object request, CancellationToken cancellationToken = default )
        {
            throw new NotSupportedException( "Streaming is not used in these controller tests." );
        }
    }
}
