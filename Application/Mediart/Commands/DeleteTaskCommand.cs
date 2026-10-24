using Application.Common.Exeptions;
using Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Mediart.Commands
{
    public record DeleteTaskCommand(int Id) : IRequest<int>
    {
    }

    public sealed class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
    {
        public DeleteTaskCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0.");
        }
    }

    public class DeleteTaskCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTaskCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        
        public async Task<int> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.TaskRepository.DeleteAsync(request.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.Id;
        }
    }
}
