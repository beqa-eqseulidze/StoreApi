

using Application.Common.Exeptions;
using Application.TDOs.Categories;
using Domain.Interfaces;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Mediart.Queries.Category
{
    public record GetCategoryByIdQuery(int Id) : IRequest<CategoryWithProductsDto>;
    public sealed class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCategoryByIdQuery, CategoryWithProductsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<CategoryWithProductsDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.CategoryRepository.GetFilteredAsync(cancellationToken);
            return await categories
                .Where(category => category.Id == request.Id)
                .ProjectToType<CategoryWithProductsDto>()
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new EntityNotFoundExeption($"Category with ID {request.Id} not found.");

        }
    }
}

