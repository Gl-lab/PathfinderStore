using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Interfaces.Auth;

namespace Pathfinder.Application.UseCases.Characters;

public class GetCurrentCharacterForAccountHandler : IRequestHandler<GetCurrentCharacterForAccountCommand, CharacterDto>
{
    private readonly ICharacterService _characterService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public GetCurrentCharacterForAccountHandler(ICharacterService characterService, IUserService userService,
        IMapper mapper)
    {
        _characterService = characterService;
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<CharacterDto> Handle(GetCurrentCharacterForAccountCommand request,
        CancellationToken cancellationToken)
    {
        var user = _userService.GetCurrentUser();
        var character = await _characterService.GetCharacterAsync(user.Id);
        return _mapper.Map<CharacterDto>(character);
    }
}