using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.Languages;

public sealed class GetLanguagesHandler : IRequestHandler<GetLanguagesCommand, IReadOnlyCollection<LanguageDto>>
{
    private readonly ILanguageRepository _languageRepository;

    public GetLanguagesHandler( ILanguageRepository languageRepository )
    {
        _languageRepository = languageRepository;
    }

    public Task<IReadOnlyCollection<LanguageDto>> Handle(
        GetLanguagesCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<LanguageDto> languages = _languageRepository
            .GetAll()
            .Select( LanguageDtoMapper.Map )
            .OrderBy( language => language.Name )
            .ToList();
        return Task.FromResult( languages );
    }
}
