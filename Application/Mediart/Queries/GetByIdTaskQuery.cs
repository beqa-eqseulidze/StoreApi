//using Application.Common.Exeptions;
//using Application.Common.Interfaces;
//using Application.TDOs;
//using FluentValidation;
//using Mapster;
//using MediatR;

//namespace Application.Mediart.Queries
//{
//    public record GetByIdTaskQuery(int Id) : IRequest<TaskItemDto>;

//    public sealed class GetByIdTaskQueryValidator : AbstractValidator<GetByIdTaskQuery>
//    {
//        public GetByIdTaskQueryValidator()
//        {
//            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Task ID must be a positive integer.");
//        }
//    }

//    public sealed class GetByIdTaskQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetByIdTaskQuery, TaskItemDto>
//    {
//        private readonly IUnitOfWork _unitOfWork = unitOfWork;
//        public async Task<TaskItemDto> Handle(GetByIdTaskQuery request, CancellationToken cancellationToken)
//        {
//            var taskItem = await _unitOfWork.TaskRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new EntityNotFoundExeption($"Task with ID {request.Id} not found.");
//            return taskItem.Adapt<TaskItemDto>();
//        }
//    }

//}
