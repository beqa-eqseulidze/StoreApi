using Application.Common.Exeptions;
using Application.TDOs.CategoryDto;
using Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Mediart.Commands.Category
{
    public record UpdateProductCommand(int Id, string Name, string Description) : IRequest<CategoryDto>;
    public sealed class UpdatecategoryCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdatecategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }

    public sealed class UpdatecategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<CategoryDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new EntityNotFoundExeption($"Category with ID {request.Id} not found.");
            category.Name = request.Name;
            category.Description = request.Description;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new CategoryDto(category.Id, category.Name, category.Description);                

        }
    }
}
