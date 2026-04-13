using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Pathfinder.CharacterManagement.Application.UseCases.Accounts;
using Pathfinder.Contracts.Events;

namespace Pathfinder.CharacterManagement.Infrastructure.Consumers;

public sealed class UserRegisteredEventConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventConsumer> _logger;
    private readonly IMediator _mediator;

    public UserRegisteredEventConsumer(
        IMediator mediator,
        ILogger<UserRegisteredEventConsumer> logger )
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume( ConsumeContext<UserRegisteredEvent> context )
    {
        _logger.LogInformation(
            "UserRegisteredEvent received for user {UserId}. MessageId: {MessageId}. CorrelationId: {CorrelationId}.",
            context.Message.UserId,
            context.MessageId,
            context.CorrelationId );

        await _mediator.Send(
            new EnsureAccountExistsCommand(
                context.Message.UserId,
                context.Message.Name,
                context.Message.Surname ) );
    }
}
