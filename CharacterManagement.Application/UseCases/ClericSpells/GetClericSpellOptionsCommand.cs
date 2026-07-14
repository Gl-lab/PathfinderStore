using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.ClericSpells;

public sealed class GetClericSpellOptionsCommand : IRequest<ClericSpellOptionsDto>
{
    public string DeityId { get; }

    public GetClericSpellOptionsCommand( string deityId )
    {
        DeityId = deityId;
    }
}
