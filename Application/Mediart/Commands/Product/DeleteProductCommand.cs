using Application.Common.Exeptions;
using Application.TDOs.CategoryDto;
using Domain.Interfaces;
using MediatR;

namespace Application.Mediart.Commands.Category
{
    //public record DeleteProductCommand(int Id) : IRequest<CategoryDto> { }
    //public sealed class DeleteCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteProductCommand, CategoryDto>
    //{
    //    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    //    public async Task<CategoryDto> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    //    {
    //        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new EntityNotFoundExeption($"Category with ID {request.Id} not found.");
    //        await _unitOfWork.CategoryRepository.DeleteAsync(category, cancellationToken);
    //        await _unitOfWork.SaveChangesAsync(cancellationToken);
    //        return new CategoryDto(category.Id, category.Name, category.Description);
    //    }
    //}
}
