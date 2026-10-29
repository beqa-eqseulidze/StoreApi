using Application.TDOs.ProductImage;
using Domain.Interfaces;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Mediart.Queries.ProductImage
{
    public record GetProductImagesByProductIdQuery(int Id) : IRequest<List<ProductImageDto>>;
    public sealed class GetProductImagesByProductIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetProductImagesByProductIdQuery, List<ProductImageDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<List<ProductImageDto>> Handle(GetProductImagesByProductIdQuery request, CancellationToken cancellationToken)
        {
            var productImages = await _unitOfWork.ProductImageRepository.GetFilteredAsync(cancellationToken);
            return await productImages
                .Where(img => img.ProductId == request.Id && !img.IsDeleted)
                .ProjectToType<ProductImageDto>()
                .ToListAsync(cancellationToken);
        }
    }

}
