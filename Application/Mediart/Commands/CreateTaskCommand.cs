using Application.Common.Interfaces;
using Application.TDOs;
using Domain.Entities;
using FluentValidation;
using Mapster;
using MediatR;

namespace Application.Mediart.Commands
{
    public record CreateTaskCommand : IRequest<TaskItemDto>
    {
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public DateTimeOffset DueDate { get; init; }
    }

    public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200).WithMessage("Title is required and must be at most 200 characters long.");
            RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description must be at most 500 characters long.");
            RuleFor(x => x.DueDate)
                .NotEmpty()
                .WithMessage("Due date is required.")
                .Must(dueDate => dueDate >= DateTimeOffset.UtcNow)
                .WithMessage("Due date cannot be in the past.");
        }
    }

    public class CreateTaskCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateTaskCommand, TaskItemDto>
        {
            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            public async Task<TaskItemDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
            {
                var taskItem = new TaskItem
                {
                    Title = request.Title,
                    Description = request.Description,
                    IsCompleted = false,
                    DueDate = request.DueDate
                };
                var newTaskItem = await _unitOfWork.TaskRepository.AddAsync(taskItem, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return newTaskItem.Adapt<TaskItemDto>();
            }
        }
}
