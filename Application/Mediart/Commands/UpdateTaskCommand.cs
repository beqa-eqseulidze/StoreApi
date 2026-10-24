

using Application.Common.Exeptions;
using Application.Common.Interfaces;
using Application.TDOs;
using FluentValidation;
using Mapster;
using MediatR;

namespace Application.Mediart.Commands
{
    public record UpdateTaskCommand : IRequest<TaskItemDto>
    {
        public int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool IsCompleted { get; init; }
        public DateTimeOffset DueDate { get; init; }
    }

    public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Task ID must be a positive integer.");
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200).WithMessage("Title is required and must be at most 200 characters long.");
            RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description must be at most 500 characters long.");
            RuleFor(x => x.DueDate)
                .NotEmpty()
                .WithMessage("Due date is required.")
                .Must(dueDate => dueDate >= DateTimeOffset.UtcNow)
                .WithMessage("Due date cannot be in the past.");
        }
    }

    public sealed class UpdateTaskCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateTaskCommand, TaskItemDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<TaskItemDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var taskItem = await _unitOfWork.TaskRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new EntityNotFoundExeption($"Task with ID {request.Id} not found.");
            taskItem.Title = request.Title;
            taskItem.Description = request.Description;
            taskItem.IsCompleted = request.IsCompleted;
            taskItem.DueDate = request.DueDate;              
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return taskItem.Adapt<TaskItemDto>();
        }
    }

}
