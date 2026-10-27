using Application.Mediart.Commands.Product;
using Application.Mediart.Queries.Product;
using Application.TDOs.Products;
using Domain.Enums.UserEnums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController(ISender mediator) : ControllerBase
    {
        private readonly ISender _mediator = mediator;


        [HttpPost("Create")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductCommand product, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(product, cancellationToken));
        }

        [HttpPut("Update")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<ProductDto>> Update([FromBody] UpdateProductCommand product, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(product, cancellationToken));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<ProductDto>> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new DeleteProductCommand(id), cancellationToken));
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ProductDto>> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetProductByIdQuery(id), cancellationToken));
        }

        [HttpGet("GetFiltered")]
        public async Task<ActionResult<List<ProductDto>>> GetFiltered([FromQuery] GetFilteredProductQuery query, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }

    }
} 