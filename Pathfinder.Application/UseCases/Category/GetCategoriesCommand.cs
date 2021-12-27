using System.Collections.Generic;
using MediatR;
using Pathfinder.Application.DTO;

namespace Pathfinder.Application.UseCases.Category;

public class GetCategoriesCommand : IRequest<ICollection<CategoryDto>>
{
}