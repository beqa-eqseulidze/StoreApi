using Application.Common.Exeptions;
using Application.TDOs.Orders;
using Domain.Entities;
using Domain.Enums.OredrEnums;
using Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Mediart.Commands.Order
{
    public class CreateOrderCommand : IRequest<OrderDto>
    {
        public int UserId { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }

    public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
                RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item.")
                .Must(x => x.All(item => item.ProductId > 0))
                .WithMessage("All ProductIds must be greater than 0.")
                .Must(x => x.All(item => item.Quantity > 0 && item.Quantity <= int.MaxValue))
                .WithMessage("All Quantities must be greater than 0 and less than or equal to {0}. and small then int.MaxValue" );
        }
    }

    public sealed class CreateOrderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {

            // Order შეიქმნება (ცარიელი OrderItems-ით)
            var order = GetEmptyOrder(request);

            decimal totalAmount = 0;

            // თითოეული item-ის დამუშავება
            foreach (var itemRequest in request.Items)
            {
                // Product ძებნა
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken) ??
                     throw new EntityNotFoundExeption($"Product with id {itemRequest.ProductId} not found.");
              
                // Stock ვერიფიკაცია
                if (product.StockQuantity < itemRequest.Quantity)
                    throw new InvalidOperationException($"Product '{product.Name}' has insufficient stock. Available: {product.StockQuantity}, Requested: {itemRequest.Quantity}");

                // OrderItem შექმნა
                var orderItem = new OrderItem
                {
                    ProductId = itemRequest.ProductId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = product.Price
                };

                // order-ის OrderItems Collection-ში დამატება
                order.OrderItems.Add(orderItem);

                // TotalAmount-ის გამოთვლა
                totalAmount += itemRequest.Quantity * product.Price;
            }
            
            order.TotalAmount = totalAmount;
           
            await _unitOfWork.OrderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            //  Response DTO შექმნა
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }

        private static Domain.Entities.Order GetEmptyOrder(CreateOrderCommand request)
        {
            return new Domain.Entities.Order
            {
                UserId = request.UserId,
                OrderDate = DateTimeOffset.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()
            };
        }
    }
}
