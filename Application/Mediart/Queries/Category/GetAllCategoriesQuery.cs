using Application.TDOs.Categories;
using Domain.Interfaces;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Mediart.Queries.Category
{
    public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;
    public sealed class GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.CategoryRepository.GetFilteredAsync(cancellationToken);

            return await categories
                .ProjectToType<CategoryDto>()
                .ToListAsync(cancellationToken);
        }
    }
}
