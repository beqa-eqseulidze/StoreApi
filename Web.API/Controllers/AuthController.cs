using Application.Mediart.Commands.Auth;
using Application.TDOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(ISender mediator) : ControllerBase
    {
        private readonly ISender _mediator = mediator;

        [HttpPost("Register")]
        public async Task<ActionResult<AuthResponseDto>> CreateTask([FromBody] RegisterCommand newUser, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(newUser, cancellationToken));
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginCommand loginCommand, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(loginCommand, cancellationToken));
        }
    }
}
