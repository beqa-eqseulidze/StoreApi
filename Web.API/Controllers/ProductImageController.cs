using Application.Mediart.Commands.ProductImage;
using Application.Mediart.Queries.ProductImage;
using Application.TDOs.ProductImage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductImageController(ISender mediator) : ControllerBase
    {
        private readonly ISender _mediator = mediator;

        [HttpPost("Upload")]
        [Consumes("multipart/form-data")]
        //[Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<int>> Upload([FromForm] UploadProductImageCommand productImage, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(productImage, cancellationToken));

        [HttpDelete("Delete/{id}")]
        //[Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult> DeleteFile(int id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new DeleteProductImageCommand(id), cancellationToken));
        [HttpGet("GetByProductId/{productId}")]
        public async Task<ActionResult<List<ProductImageDto>>> GetByProductId([FromRoute] int productId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetProductImagesByProductIdQuery(productId), cancellationToken));
    }
}