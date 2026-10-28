using Application.Mediart.Commands.Order;
using Application.Mediart.Queries.Order;
using Application.TDOs.Orders;
using Domain.Enums.UserEnums;
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

        [HttpGet("GetFiltered")]
        [Authorize]
        public async Task<ActionResult<List<OrderDto>>> GetFiltered([FromQuery] GetFilteredOrdersQuery query, CancellationToken cancellationToken)
        {
            var isAdmin = User.IsInRole(nameof(UserRole.Admin));
            var filteredQuery = new GetFilteredOrdersQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortBy = query.SortBy,
                SortOrder = query.SortOrder,
                UserId = isAdmin? query.UserId : this.GetUserId(),
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo,
                TotalAmountFrom = query.TotalAmountFrom,
                TotalAmountTo = query.TotalAmountTo               
            };

            return Ok(await _mediator.Send(filteredQuery, cancellationToken));
        }
    }
}
