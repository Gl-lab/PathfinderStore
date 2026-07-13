using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.CharacterClasses;

public sealed class GetCharacterClassesHandler : IRequestHandler<GetCharacterClassesCommand, IReadOnlyCollection<CharacterClassDto>>
{
    private readonly ICharacterClassRepository _characterClassRepository;

    public GetCharacterClassesHandler( ICharacterClassRepository characterClassRepository )
    {
        _characterClassRepository = characterClassRepository;
    }

    public Task<IReadOnlyCollection<CharacterClassDto>> Handle(
        GetCharacterClassesCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<CharacterClassDto> characterClasses = _characterClassRepository
            .GetAll()
            .Select( CharacterClassDtoMapper.Map )
            .OrderBy( characterClass => characterClass.Name )
            .ToList();

        return Task.FromResult( characterClasses );
    }
}
