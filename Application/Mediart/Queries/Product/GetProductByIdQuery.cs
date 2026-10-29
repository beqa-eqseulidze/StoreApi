using Application.Common.Exeptions;
using Application.TDOs.Products;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Mediart.Queries.Product
{
    public record GetProductByIdQuery(int Id):IRequest<ProductDto>;
    public sealed class GetProductByIdQueryHendler(IUnitOfWork unitOfWork) : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.ProductRepository.GetFilteredAsync(cancellationToken);
            var product = await products.Include(c => c.ProductImages).FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
                ?? throw new EntityNotFoundExeption($"Product with ID {request.Id} not found.");

            return new ProductDto(
                product.Id, 
                product.Name, 
                product.Description, 
                product.Price,
                product.StockQuantity, 
                product.CategoryId,
                product.ProductImages.Select(img => img.FilePath).ToList(),       // Mapping ImageUrls
                null,
                null
                );
        }
    }
}
