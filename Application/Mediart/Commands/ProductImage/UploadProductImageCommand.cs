
using Application.Common.Exeptions;
using Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;

namespace Application.Mediart.Commands.ProductImage
{
    public record UploadProductImageCommand() : IRequest<int>
    {
        public int ProductId { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;

        public bool IsMain { get; set; } = false;
    }

    public sealed class UploadProductImageCommandValidator : AbstractValidator<UploadProductImageCommand>
    {
        public UploadProductImageCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId must be greater than 0.");
            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.");
        }
    }

    public class CreateUploadProductImageCommandHandler(IUnitOfWork unitOfWork, IWebHostEnvironment environment) : IRequestHandler<UploadProductImageCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IWebHostEnvironment _environment = environment;

        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

        public async Task<int> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
        {
            // 1. ვალიდაციები
            if (request.File == null || request.File.Length == 0)
                throw new ArgumentException("ფაილი ცარიელია.");

            if (request.File.Length > MaxFileSizeInBytes)
                throw new ArgumentException("ფაილის ზომა აჭარბებს 5 MB-ს.");

            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                throw new ArgumentException("არასწორი ფორმატი. დაშვებულია: .jpg, .jpeg, .png, .webp, .gif");

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.ProductId, cancellationToken) ?? throw new EntityNotFoundExeption($"პროდუქტი ID-ით {request.ProductId} ვერ მოიძებნა.");

            // 2. wwwroot/images საქაღალდის მომზადება
            var rootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var imagesFolder = Path.Combine(rootPath, "images");

            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            // 3. უნიკალური სახელით ფაილის შენახვა
            var storedFileName = $"{Guid.NewGuid()}";
            if (request.IsMain)
            {
                storedFileName += "_main";
            }
            var storedFileNameWithExtension = storedFileName + extension;
            var fullPath = Path.Combine(imagesFolder, storedFileNameWithExtension);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream, cancellationToken);
            }

            // 4. ბაზაში ჩანაწერი
            var productImage = new Domain.Entities.ProductImage
            {
                ProductId = request.ProductId,
                FileName = request.File.FileName,
                FilePath = $"/images/{storedFileNameWithExtension}",
                ContentType = request.File.ContentType,
                FileSize = request.File.Length,
                IsMain = request.IsMain
            };

            await _unitOfWork.ProductImageRepository.AddAsync(productImage, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return productImage.Id;
        }
    }
}