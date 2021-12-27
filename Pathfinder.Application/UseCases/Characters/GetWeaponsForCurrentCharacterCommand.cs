using System.Collections.Generic;
using MediatR;
using Pathfinder.Application.DTO.Items;

namespace Pathfinder.Application.UseCases.Characters;

public class GetWeaponsForCurrentCharacterCommand : IRequest<ICollection<WeaponItemDto>>
{
}