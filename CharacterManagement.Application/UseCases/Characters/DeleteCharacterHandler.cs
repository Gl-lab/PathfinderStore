using MediatR;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public class DeleteCharacterHandler : IRequestHandler<DeleteCharacterCommand>
{
    public DeleteCharacterHandler()
    {
    }

    public async Task Handle( DeleteCharacterCommand request, CancellationToken cancellationToken )
    {
        throw new NotImplementedException();
    }
}