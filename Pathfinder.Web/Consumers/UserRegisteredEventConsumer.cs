using System.Threading.Tasks;
using MassTransit;
using MassTransit.Mediator;
using Pathfinder.CharacterManagement.Application.UseCases.Accounts;
using Pathfinder.Contracts.Events;

namespace Pathfinder.Web.Consumers;

public class UserRegisteredEventConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IMediator _mediator;

    public UserRegisteredEventConsumer( IMediator mediator )
    {
        _mediator = mediator;
    }

    public async Task Consume( ConsumeContext<UserRegisteredEvent> context ) => await _mediator.Send( new CreateNewAccountCommand( context.Message.UserId, context.Message.Name, context.Message.Surname ) );
}