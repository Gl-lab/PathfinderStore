using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IWitchPatronRepository
{
    IReadOnlyCollection<WitchPatron> GetAll();
    WitchPatron GetWitchPatron( string witchPatronId );
}
