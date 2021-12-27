using MediatR;
using Pathfinder.Application.DTO;

namespace Pathfinder.Application.UseCases.Category;

public class CategoryByCategoryTypeCommand : IRequest<CategoryDto>
{
    public CategoryByCategoryTypeCommand(byte categoryType)
    {
        CategoryType = categoryType;
    }

    public byte CategoryType { get; }
}