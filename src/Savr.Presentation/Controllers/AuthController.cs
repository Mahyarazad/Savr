using MediatR;
using Microsoft.AspNetCore.Mvc;
using Savr.Application.Features.Identity.Commands.Login;
using Savr.Application.Features.Identity.Commands.Register;
using Savr.Presentation.Helpers;

namespace Savr.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            if(result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(ResultErrorParser.ParseResultError(result.Errors));
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            if(result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(ResultErrorParser.ParseResultError(result.Errors));
            }
        }
    }
}
