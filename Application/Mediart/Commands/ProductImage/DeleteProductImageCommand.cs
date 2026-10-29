
using Application.Common.Exeptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Application.Mediart.Commands.ProductImage
{
    public record DeleteProductImageCommand(int Id) : IRequest<int>;
    public sealed class DeleteProductImageCommandHandler(IUnitOfWork unitOfWork, IWebHostEnvironment environment) : IRequestHandler<DeleteProductImageCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IWebHostEnvironment _environment = environment;
        public async Task<int> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
        {
            var productImage = await _unitOfWork.ProductImageRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundExeption($"პროდუქტის სურათი ID-ით {request.Id} ვერ მოიძებნა.");
            // Remove the image file from the server
            var rootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var imagesFolder = Path.Combine(rootPath, "images");
            var storedFileName = Path.GetFileName(productImage.FilePath.Replace('/', Path.DirectorySeparatorChar));
            var filePath = Path.Combine(imagesFolder, storedFileName);

            if (!File.Exists(filePath) && string.IsNullOrEmpty(Path.GetExtension(storedFileName)))
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                filePath = allowedExtensions
                    .Select(extension => Path.Combine(imagesFolder, storedFileName + extension))
                    .FirstOrDefault(File.Exists) ?? filePath;
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Remove the image record from the database
            await _unitOfWork.ProductImageRepository.DeleteAsync(productImage, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return request.Id;
        }
    }

}
