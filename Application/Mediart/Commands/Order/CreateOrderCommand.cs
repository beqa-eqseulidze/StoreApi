using Application.TDOs.Orders;
using Domain.Entities;
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
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item.")
                .Must(x => x.All(item => item.ProductId > 0))
                .WithMessage("All ProductIds must be greater than 0.")
                .Must(x => x.All(item => item.Quantity > 0))
                .WithMessage("All Quantities must be greater than 0.");
        }
    }

    public sealed class CreateOrderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // ვერიფიკაცია რომ User არსებობს
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken)
                        ?? throw new InvalidOperationException($"User with id {request.UserId} not found.");

            // Order შეიქმნება (ცარიელი OrderItems-ით)
            var order = new Domain.Entities.Order
            {
                UserId = request.UserId,
                OrderDate = DateTimeOffset.UtcNow,
                Status = Domain.Enums.OredrEnums.OrderStatus.Pending,
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()
            };

            decimal totalAmount = 0;

            // ✅ Step 3: თითოეული item-ის დამუშავება
            foreach (var itemRequest in request.Items)
            {
                // Product ძებნა
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken);
                if (product == null)
                    throw new InvalidOperationException($"Product with id {itemRequest.ProductId} not found.");

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

            // ✅ Step 4: TotalAmount დაყენება
            order.TotalAmount = totalAmount;

            // ✅ Step 5: Database-ში შენახვა (ერთი ტრანზაქცია)
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
    }
}
