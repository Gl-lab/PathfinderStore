using System.Collections.Generic;
using MediatR;
using Pathfinder.Store.Application.DTO;

namespace Pathfinder.Store.Application.UseCases.Category;

public class GetCategoriesCommand : IRequest<ICollection<CategoryDto>>
{
}