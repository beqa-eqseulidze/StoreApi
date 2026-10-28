

using Application.TDOs.Orders;
using Domain.Enums.CommonEnums;
using Domain.Enums.OredrEnums;
using Domain.Enums.UserEnums;
using Domain.Interfaces;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Mediart.Queries.Order
{
    public sealed class GetFilteredOrdersQuery() : IRequest<List<OrderDto>>
    {
        public const int MaxPageSize = 100;
        public int PageIndex { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public SortDirection? SortOrder { get; init; }
        public OrderSortFields? SortBy { get; init; }
        public int? UserId { get; init; }
        public OrderStatus? Status { get; init; }
        public DateTime? OrderDateFrom { get; init; }
        public DateTime? OrderDateTo { get; init; }
        public decimal? TotalAmountFrom { get; init; }
        public decimal? TotalAmountTo { get; init; }
    };

    public sealed class GetFilteredOrdersQueryValidator : AbstractValidator<GetFilteredOrdersQuery>
    {
        public GetFilteredOrdersQueryValidator()
        {
            RuleFor(query => query.PageIndex)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page index must be a positive integer.");

            RuleFor(query => query.PageSize)
                .InclusiveBetween(1, GetFilteredOrdersQuery.MaxPageSize)
                .WithMessage($"Page size must be between 1 and {GetFilteredOrdersQuery.MaxPageSize}.");

            RuleFor(query => query.UserId)
                .GreaterThan(0)
                .When(query => query.UserId.HasValue)
                .WithMessage("User ID must be greater than 0.");

            RuleFor(query => query.OrderDateTo)
                .GreaterThanOrEqualTo(query => query.OrderDateFrom)
                .When(query => query.OrderDateFrom.HasValue && query.OrderDateTo.HasValue)
                .WithMessage("End date must be on or after start date.");

            RuleFor(query => query.TotalAmountFrom)
                .GreaterThanOrEqualTo(0)
                .When(query => query.TotalAmountFrom.HasValue)
                .WithMessage("Minimum amount cannot be negative.");

            RuleFor(query => query.TotalAmountTo)
                .GreaterThanOrEqualTo(query => query.TotalAmountFrom)
                .When(query => query.TotalAmountFrom.HasValue && query.TotalAmountTo.HasValue)
                .WithMessage("Maximum amount must be greater than or equal to minimum amount.");
        }
    }

    public sealed class GetFilteredOrdersQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetFilteredOrdersQuery, List<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<OrderDto>> Handle(GetFilteredOrdersQuery request, CancellationToken cancellationToken)
        {
            var query = await _unitOfWork.OrderRepository.GetFilteredAsync(cancellationToken);
            query = query.AsNoTracking();

            if (request.UserId.HasValue)
            {
                query = query.Where(order => order.UserId == request.UserId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(order => order.Status == request.Status.Value);
            }

            if (request.OrderDateFrom.HasValue)
            {
                var dateFrom = new DateTimeOffset(DateTime.SpecifyKind(request.OrderDateFrom.Value, DateTimeKind.Utc));
                query = query.Where(order => order.OrderDate >= dateFrom);
            }

            if (request.OrderDateTo.HasValue)
            {
                var dateTo = new DateTimeOffset(DateTime.SpecifyKind(request.OrderDateTo.Value, DateTimeKind.Utc));
                query = query.Where(order => order.OrderDate <= dateTo);
            }

            if (request.TotalAmountFrom.HasValue)
            {
                query = query.Where(order => order.TotalAmount >= request.TotalAmountFrom.Value);
            }

            if (request.TotalAmountTo.HasValue)
            {
                query = query.Where(order => order.TotalAmount <= request.TotalAmountTo.Value);
            }

            query = ApplySorting(query, request.SortBy, request.SortOrder);
            query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            return await query
                .Select(order => new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status.ToString(),
                    OrderItems = order.OrderItems.Select(item => new OrderItemDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }

        private static IQueryable<Domain.Entities.Order> ApplySorting(
            IQueryable<Domain.Entities.Order> query,
            OrderSortFields? sortBy,
            SortDirection? sortOrder)   
        {
            var descending = sortOrder == SortDirection.Desc;

            return sortBy switch
            {
                OrderSortFields.OrderDate => descending
                    ? query.OrderByDescending(order => order.OrderDate)
                    : query.OrderBy(order => order.OrderDate),
                OrderSortFields.Amount => descending
                    ? query.OrderByDescending(order => order.TotalAmount)
                    : query.OrderBy(order => order.TotalAmount),
                OrderSortFields.Status => descending
                    ? query.OrderByDescending(order => order.Status)
                    : query.OrderBy(order => order.Status),
                _ => descending
                    ? query.OrderByDescending(order => order.Id)
                    : query.OrderBy(order => order.Id)
            };
        }
    }
}
