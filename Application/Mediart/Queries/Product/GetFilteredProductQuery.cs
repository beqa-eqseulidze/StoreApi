using Application.TDOs.Products;
using Domain.Enums.CommonEnums;
using Domain.Enums.ProductEnums;
using Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Mediart.Queries.Product
{
    public record GetFilteredProductQuery() : IRequest<List<ProductDto>>
    {
            public const int MaxPageSize = 100;
            public int PageIndex { get; init; } = 1;
            public int PageSize { get; init; } = 10;
            public ProductSortFields? SortBy { get; init; }
            public SortDirection? SortOrder { get; init; }
            public string? Name { get; init; }
            public string? Description { get; init; }
            public decimal? PriceFrom { get; init; }
            public decimal? PriceTo { get; init; }
    }

    public sealed class  GetFilteredProductsValidator:AbstractValidator<GetFilteredProductQuery>
    {
        public GetFilteredProductsValidator()
        {
            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(1).WithMessage("Page index must be a positive integer.");
            RuleFor(x => x.PageSize).InclusiveBetween(1, GetFilteredProductQuery.MaxPageSize).WithMessage($"Page size must be between 1 and {GetFilteredProductQuery.MaxPageSize}.");
        }
    }

    public sealed class GetFilteredProductsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetFilteredProductQuery, List<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<List<ProductDto>> Handle(GetFilteredProductQuery request, CancellationToken cancellationToken)
        {
            var query = await _unitOfWork.ProductRepository.GetFilteredAsync(cancellationToken);
            // Apply filters
            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(p => p.Name.Contains(request.Name));
            }
            if (!string.IsNullOrEmpty(request.Description))
            {
                query = query.Where(p => p.Description.Contains(request.Description));
            }
            if (request.PriceFrom.HasValue)
            {
                query = query.Where(p => p.Price >= request.PriceFrom.Value);
            }
            if (request.PriceTo.HasValue)
            {
                query = query.Where(p => p.Price <= request.PriceTo.Value);
            }
            // Apply sorting
            if (request.SortBy.HasValue)
            {
                query = request.SortBy.Value switch
                {
                    ProductSortFields.Name => request.SortOrder == SortDirection.Desc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    ProductSortFields.Price => request.SortOrder == SortDirection.Desc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                    _ => request.SortOrder == SortDirection.Desc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
                };
            }
            // Apply pagination
            query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            return await query.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.StockQuantity,
                p.CategoryId,
                null,
                null,
                null
            )).ToListAsync();
        }
    }

}

