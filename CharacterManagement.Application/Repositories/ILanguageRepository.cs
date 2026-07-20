using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface ILanguageRepository
{
    IReadOnlyCollection<LanguageDefinition> GetAll();
    LanguageDefinition GetLanguage( string languageId );
}
