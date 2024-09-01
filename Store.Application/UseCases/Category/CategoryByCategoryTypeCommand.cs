using MediatR;
using Pathfinder.Store.Application.DTO;

namespace Pathfinder.Store.Application.UseCases.Category;

public class CategoryByCategoryTypeCommand : IRequest<CategoryDto>
{
    public CategoryByCategoryTypeCommand(byte categoryType)
    {
        CategoryType = categoryType;
    }

    public byte CategoryType { get; }
}