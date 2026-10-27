

using Application.Common.Exeptions;
using Application.TDOs.CategoryDto;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Mediart.Queries.Category
{
    public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto>;  
    public sealed class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.CategoryRepository.GetFilteredAsync(cancellationToken);
            var category = await categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
                ?? throw new EntityNotFoundExeption($"Category with ID {request.Id} not found.");

            return new CategoryDto(category.Id, category.Name, category.Description, category.Products.ToList());
        }
    }
}
