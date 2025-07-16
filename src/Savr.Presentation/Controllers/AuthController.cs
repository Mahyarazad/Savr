using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Savr.Application.Abstractions.Identity;
using Savr.Application.Features.Identity.Commands.Login;
using Savr.Application.Features.Identity.Commands.Register;
using Savr.Identity.Models;
using Savr.Presentation.Helpers;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


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

        [HttpGet("signin-google")]
        public IActionResult GoogleLogin(string? returnUrl = "/")
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { returnUrl });
            var props = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(props, "Google");
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = "/success")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Redirect("/error");

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return Redirect("/error");

                var loginResult = await _userManager.AddLoginAsync(user, info);
                if (!loginResult.Succeeded)
                    return Redirect("/error");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);


            return Ok();
            //var token = await GenerateJWTToken(user, await _userManager.GetRolesAsync(user));

            //return Ok(new
            //{
            //    user.Id,
            //    user.Email,
            //    token = new JwtSecurityTokenHandler().WriteToken(token)
            //});
        }


        [HttpGet("login-failed")]
        public IActionResult LoginFailed([FromQuery] string? error = "Unknown error")
        {
            Log.Warning("Google login failed: {Error}", error);

            return BadRequest(new
            {
                message = "Google sign-in failed",
                error = error
            });
        }


    }
}
