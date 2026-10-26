using Application.Mediart.Commands.Category;
using Application.TDOs.Category;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class CategoryController(ISender mediator) : ControllerBase
    {
        private readonly ISender _mediator = mediator;

        [HttpPost("CreateCategory")]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryCommand category, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(category, cancellationToken));
        }

        //[HttpPut("UpdateTask")]
        //public async Task<ActionResult<TaskItemDto>> UpdateTask([FromBody] UpdateTaskCommand taskItem, CancellationToken cancellationToken)
        //{
        //    return Ok(await _mediator.Send(taskItem, cancellationToken));
        //}

        //[HttpGet("GetTaskById/{id}")]
        //public async Task<ActionResult<TaskItemDto>> GetTaskById([FromRoute] int id, CancellationToken cancellationToken)
        //{
        //    return Ok(await _mediator.Send( new GetByIdTaskQuery(id), cancellationToken));
        //}

        //[HttpGet("CompleteTask/{id}")]
        //public async Task<ActionResult<TaskItemDto>> CompleteTask([FromRoute] int id, CancellationToken cancellationToken)
        //{
        //    return Ok(await _mediator.Send(new CompliteTaskCommand(id), cancellationToken));
        //}

        //[HttpDelete("Delete/{id}")]
        //public async Task<ActionResult<TaskItemDto>> DeleteTask([FromRoute] int id, CancellationToken cancellationToken)
        //{
        //    return Ok(await _mediator.Send(new DeleteTaskCommand(id), cancellationToken));
        //}

        //[HttpGet("GetFilteredTasks")]
        //public async Task<ActionResult<List<TaskItemDto>>> GetFilteredTasks([FromQuery] GetFilteredTasksQuery query, CancellationToken cancellationToken)
        //{
        //    return Ok(await _mediator.Send(query, cancellationToken));
        //}

    }
} 