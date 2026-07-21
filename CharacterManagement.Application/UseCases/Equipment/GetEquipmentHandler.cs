using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.Equipment;

public sealed class GetEquipmentHandler : IRequestHandler<GetEquipmentCommand, IReadOnlyCollection<EquipmentDto>>
{
    private readonly IEquipmentRepository _equipmentRepository;

    public GetEquipmentHandler( IEquipmentRepository equipmentRepository )
    {
        _equipmentRepository = equipmentRepository;
    }

    public Task<IReadOnlyCollection<EquipmentDto>> Handle(
        GetEquipmentCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<EquipmentDto> equipment = _equipmentRepository
            .GetAll()
            .Select( EquipmentDtoMapper.Map )
            .OrderBy( definition => definition.Name )
            .ToArray();
        return Task.FromResult( equipment );
    }
}
