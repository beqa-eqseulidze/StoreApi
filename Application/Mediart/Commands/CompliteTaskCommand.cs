

using Application.Common.Exeptions;
using Application.Common.Interfaces;
using Application.TDOs;
using FluentValidation;
using Mapster;
using MediatR;

namespace Application.Mediart.Commands
{
    public record CompliteTaskCommand(int Id) :IRequest<TaskItemDto>
    {
    }

    public sealed class CompliteTaskCommandValidator : AbstractValidator<CompliteTaskCommand>
    {
        public CompliteTaskCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Task ID must be a positive integer.");
        }
    }

    public sealed class CompliteTaskCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CompliteTaskCommand, TaskItemDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<TaskItemDto> Handle(CompliteTaskCommand request, CancellationToken cancellationToken)
        {
            var taskItem = await _unitOfWork.TaskRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new EntityNotFoundExeption($"Task with ID {request.Id} not found.");
            taskItem.IsCompleted = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return taskItem.Adapt<TaskItemDto>();
        }
    }

}
