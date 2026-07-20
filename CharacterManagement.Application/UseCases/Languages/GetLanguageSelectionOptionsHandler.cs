using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Languages;

namespace Pathfinder.CharacterManagement.Application.UseCases.Languages;

public sealed class GetLanguageSelectionOptionsHandler
    : IRequestHandler<GetLanguageSelectionOptionsCommand, LanguageSelectionOptionsDto>
{
    private readonly IAncestryRepository _ancestryRepository;
    private readonly ILanguageRepository _languageRepository;

    public GetLanguageSelectionOptionsHandler(
        IAncestryRepository ancestryRepository,
        ILanguageRepository languageRepository )
    {
        _ancestryRepository = ancestryRepository;
        _languageRepository = languageRepository;
    }

    public Task<LanguageSelectionOptionsDto> Handle(
        GetLanguageSelectionOptionsCommand request,
        CancellationToken cancellationToken )
    {
        Ancestry ancestry = _ancestryRepository.GetAncestry( request.AncestryType );
        int intelligenceModifier = new Characteristic( request.IntelligenceScore ).Modifier;
        LanguageSelectionOptions options = LanguageSelectionResolver.ResolveOptions(
            ancestry,
            intelligenceModifier,
            _languageRepository.GetAll() );
        IReadOnlyList<LanguageDto> availableLanguages = options.AvailableLanguageIds
            .Select( languageId => _languageRepository.GetLanguage( languageId.Value ) )
            .Select( LanguageDtoMapper.Map )
            .ToArray();

        return Task.FromResult( new LanguageSelectionOptionsDto
        {
            RequiredCount = options.RequiredCount,
            AvailableLanguages = availableLanguages,
        } );
    }
}
