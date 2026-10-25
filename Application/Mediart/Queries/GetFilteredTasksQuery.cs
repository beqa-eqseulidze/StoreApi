//using Application.Common.Interfaces;
//using Application.Enums;
//using Application.TDOs;
//using Domain.Entities;
//using FluentValidation;
//using Mapster;
//using MediatR;
//using System.Linq.Expressions;

//namespace Application.Mediart.Queries
//{
//    public sealed class GetFilteredTasksQuery : IRequest<List<TaskItemDto>>
//    {
//        public const int MaxPageSize = 100;
//        public int PageIndex { get; init; } = 1;
//        public int PageSize { get; init; } = 10;
//        public TaskItemSortBy? SortBy { get; init; }
//        public SortDirection? SortOrder { get; init; }
//        public string? Title { get; init; }
//        public string? Description { get; init; }
//        public bool? IsCompleted { get; init; }
//        public DateTimeOffset? DueDateFrom { get; init; }
//        public DateTimeOffset? DueDateTo { get; init; }
//    }

//    public sealed class GetFilteredTasksQueryValidator : AbstractValidator<GetFilteredTasksQuery>
//    {
//        public GetFilteredTasksQueryValidator()
//        {
//            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(1).WithMessage("Page index must be a positive integer.");
//            RuleFor(x => x.PageSize).InclusiveBetween(1, GetFilteredTasksQuery.MaxPageSize).WithMessage($"Page size must be between 1 and {GetFilteredTasksQuery.MaxPageSize}.");
//        }
//    }

//    public sealed class GetFilteredTasksQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetFilteredTasksQuery, List<TaskItemDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork = unitOfWork;

//        public async Task<List<TaskItemDto>> Handle(GetFilteredTasksQuery request, CancellationToken cancellationToken)
//        {
//            var query = await _unitOfWork.TaskRepository.GetFilteredAsync(cancellationToken);

//            // Apply filters
//            if (!string.IsNullOrEmpty(request.Title))
//            {
//                query = query.Where(t => t.Title.Contains(request.Title));
//            }
//            if (!string.IsNullOrEmpty(request.Description))
//            {
//                query = query.Where(t => t.Description.Contains(request.Description));
//            }
//            if (request.IsCompleted.HasValue)
//            {
//                query = query.Where(t => t.IsCompleted == request.IsCompleted.Value);
//            }

//            if (request.DueDateFrom.HasValue)
//            {
//                query = query.Where(t => t.DueDate >= request.DueDateFrom.Value);
//            }

//            if (request.DueDateTo.HasValue)
//            {
//                query = query.Where(t => t.DueDate <= request.DueDateTo.Value);
//            }

//            //apply sorting
//             var sortBy = request.SortBy ?? TaskItemSortBy.Id; 
//            Expression<Func<TaskItem, object>> predicate = sortBy switch
//                {
//                    TaskItemSortBy.DueDate => t => t.DueDate,
//                    TaskItemSortBy.Title => t => t.Title,
//                    TaskItemSortBy.Description => t => t.Description,
//                    TaskItemSortBy.CreatedAt => t => t.CreatedAt,
//                    _ => t => t.Id // Default სორტირება
//                };

//                query = request.SortOrder == SortDirection.Desc
//                    ? query.OrderByDescending(predicate)
//                    : query.OrderBy(predicate);
            

//            // Apply pagination
//            query = query
//                .Skip((request.PageIndex - 1) * request.PageSize)
//                .Take(request.PageSize);


//            return query.ToList().Adapt<List<TaskItemDto>>();
//        }
//    }



//}
