using MediatR;
using Microsoft.AspNetCore.Mvc;
using Savr.Application.Features.Identity.Commands.Login;
using Savr.Application.Features.Identity.Commands.Register;
using Savr.Presentation.Helpers;
using Serilog;

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

            try
            {
                var result = await _sender.Send(command, cancellationToken);
                if(result.IsSuccess)
                {
                    return Ok(result.Value);
                }

                return BadRequest(ResultErrorParser.ParseResultError(result.Errors));
                
            }
            catch (Exception ex)
            {
                
                Log.Error(ex, "Login failed unexpectedly.");

                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _sender.Send(command, cancellationToken);
                if(result.IsSuccess)
                {
                    return Ok(result.Value);
                }
                
                return BadRequest(ResultErrorParser.ParseResultError(result.Errors));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Registration failed unexpectedly.");

                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }
    }
}
