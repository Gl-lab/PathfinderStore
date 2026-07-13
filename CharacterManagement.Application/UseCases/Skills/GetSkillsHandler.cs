using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.Skills;

public sealed class GetSkillsHandler : IRequestHandler<GetSkillsCommand, IReadOnlyCollection<SkillDto>>
{
    private readonly ISkillRepository _skillRepository;

    public GetSkillsHandler( ISkillRepository skillRepository )
    {
        _skillRepository = skillRepository;
    }

    public Task<IReadOnlyCollection<SkillDto>> Handle(
        GetSkillsCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<SkillDto> skills = _skillRepository
            .GetAll()
            .Select( SkillDtoMapper.Map )
            .OrderBy( skill => skill.Name )
            .ToList();
        return Task.FromResult( skills );
    }
}
