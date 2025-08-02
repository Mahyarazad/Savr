using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Savr.Application.Features.Identity.Commands;
using Savr.Application.Features.Identity.Commands.Login;
using Savr.Application.Features.Identity.Commands.Register;
using Savr.Application.Features.Identity.Queries;
using Savr.Identity.Models;
using Savr.Presentation.Helpers;


namespace Savr.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(ISender sender, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _sender = sender;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
        {

            var result = await _sender.Send(command, cancellationToken);
            if(result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(ResultErrorParser.ParseResultError(result.Errors));
            
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
        {
                var result = await _sender.Send(command, cancellationToken);
                if(result.IsSuccess)
                {
                    return Ok(result.Value);
                }
                
                return BadRequest(ResultErrorParser.ParseResultError(result.Errors));
            
        }


        //[Authorize (Roles = "Admin")]
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
                var result = await _sender.Send(new QueryAllUser(), cancellationToken);
                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }

                return BadRequest(ResultErrorParser.ParseResultError(result.Errors));
            
        }

        [HttpPost("generate-reset-password-link")]
        public async Task<IActionResult> GeneratePasswordResetLink(GeneratePasswordResetLinkCommand command, CancellationToken cancellationToken)
        {
                var result = await _sender.Send(command, cancellationToken);
                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }

                return BadRequest(ResultErrorParser.ParseResultError(result.Errors));
           
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
                var result = await _sender.Send(command, cancellationToken);
                if (result.IsSuccess)
                {
                    return Ok();
                }

                return BadRequest(ResultErrorParser.ParseResultError(result.Errors));
           
        }

    }
}
