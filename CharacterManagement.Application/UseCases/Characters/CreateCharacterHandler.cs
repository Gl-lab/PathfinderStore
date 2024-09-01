using MediatR;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public class CreateCharacterHandler : IRequestHandler<CreateCharacterCommand>
{
    public CreateCharacterHandler(  )
    {
    }

    public async Task Handle( CreateCharacterCommand request, CancellationToken cancellationToken )
    {
        //await _accountService.CreateCharacterAsync(request.Character);
    }
}