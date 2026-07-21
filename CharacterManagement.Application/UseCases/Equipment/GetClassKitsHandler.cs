using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.Equipment;

public sealed class GetClassKitsHandler : IRequestHandler<GetClassKitsCommand, IReadOnlyCollection<ClassKitDto>>
{
    private readonly IEquipmentRepository _equipmentRepository;

    public GetClassKitsHandler( IEquipmentRepository equipmentRepository )
    {
        _equipmentRepository = equipmentRepository;
    }

    public Task<IReadOnlyCollection<ClassKitDto>> Handle(
        GetClassKitsCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<ClassKitDto> kits = _equipmentRepository
            .GetClassKits()
            .Select( EquipmentDtoMapper.Map )
            .OrderBy( kit => kit.Name )
            .ToArray();
        return Task.FromResult( kits );
    }
}
