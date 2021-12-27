using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Interfaces;

namespace Pathfinder.Application.UseCases.Category;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesCommand, ICollection<CategoryDto>>
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public GetCategoriesHandler(ICategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public async Task<ICollection<CategoryDto>> Handle(GetCategoriesCommand request,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetCategoryList();
        return _mapper.Map<ICollection<CategoryDto>>(categories);
    }
}