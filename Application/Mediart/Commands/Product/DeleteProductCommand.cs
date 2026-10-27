using Application.Common.Exeptions;
using Application.TDOs.Products;
using Domain.Interfaces;
using MediatR;

namespace Application.Mediart.Commands.Product
{
    public record DeleteProductCommand(int Id) : IRequest<ProductDto>;
    public sealed class DeleteProductCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ProductDto> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new EntityNotFoundExeption($"Product with ID {request.Id} not found.");
            await _unitOfWork.ProductRepository.DeleteAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.StockQuantity, product.CategoryId, null, null, null);
        }
    }

}
