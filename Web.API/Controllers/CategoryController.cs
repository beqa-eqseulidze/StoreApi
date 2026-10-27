using Application.Mediart.Commands.Category;
using Application.Mediart.Queries.Category;
using Application.TDOs.CategoryDto;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    public class CategoryController(ISender mediator) : ControllerBase
    {
        private readonly ISender _mediator = mediator;

        [HttpPost("Create")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryCommand category, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(category, cancellationToken));
        }

        [HttpPut("Update")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<CategoryDto>> Update([FromBody] UpdateProductCommand category, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(category, cancellationToken));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<CategoryDto>> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new DeleteProductCommand(id), cancellationToken));
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<CategoryDto>> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetCategoryByIdQuery(id), cancellationToken));
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<CategoryDto>>> GetAll( CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetAllCategoriesQuery(), cancellationToken));
        }

    }
} 