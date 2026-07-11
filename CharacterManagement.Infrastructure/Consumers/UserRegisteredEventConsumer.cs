using System.Collections.Generic;
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
        using IDisposable? logScope = _logger.BeginScope(
            new Dictionary<string, object?>
            {
                [ "UserId" ] = context.Message.UserId,
                [ "MessageId" ] = context.MessageId,
                [ "CorrelationId" ] = context.CorrelationId,
                [ "ConversationId" ] = context.ConversationId,
            } );

        try
        {
            _logger.LogInformation( "Received {EventType}.", nameof( UserRegisteredEvent ) );

            await _mediator.Send(
                new EnsureAccountExistsCommand(
                    context.Message.UserId,
                    context.Message.Name,
                    context.Message.Surname ),
                context.CancellationToken );

            _logger.LogInformation( "Processed {EventType}.", nameof( UserRegisteredEvent ) );
        }
        catch ( Exception exception )
        {
            _logger.LogError(
                exception,
                "Failed to process {EventType}.",
                nameof( UserRegisteredEvent ) );
            throw;
        }
    }
}
