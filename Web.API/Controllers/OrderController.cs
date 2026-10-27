using Application.Mediart.Commands.Order;
using Application.TDOs.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Extentions;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(ISender mediator) : ControllerBase
    {
        private readonly ISender _mediator = mediator;

        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] List<CreateOrderItemRequest> orders, CancellationToken cancellationToken)
        {
            var command = new CreateOrderCommand
            {
                UserId = this.GetUserId(),
                Items = orders
            };
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
