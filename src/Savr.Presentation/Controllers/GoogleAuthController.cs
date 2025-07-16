using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Savr.Application.Abstractions.Identity;
using Savr.Identity.Models;
using Serilog;
using System.Data;
using System.Security.Claims;


namespace Savr.Presentation.Controllers
{
    [ApiController]
    [Route("auth")]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public GoogleAuthController(IAuthService authService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("google-login")]
        public IActionResult LoginWithGoogle()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback", "GoogleAuth")
            };

            return Challenge(props, "Google");
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return Unauthorized("Google authentication failed.");

            var googleClaims = result.Principal.Claims;

            var email = googleClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = googleClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleId = googleClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(googleId))
                return BadRequest("Email or Google ID missing.");

            var firstName = name?.Split(' ').FirstOrDefault() ?? "";
            var lastName = name?.Split(' ').Skip(1).FirstOrDefault() ?? "";

            // ✅ Try to find the user by external login
            var user = await _userManager.FindByLoginAsync("Google", googleId);

            if (user == null)
            {
                // ✅ Fallback: find by email
                user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    // Check if already linked
                    var logins = await _userManager.GetLoginsAsync(user);
                    if (!logins.Any(l => l.LoginProvider == "Google"))
                    {
                        var addLoginResult = await _userManager.AddLoginAsync(
                            user,
                            new UserLoginInfo("Google", googleId, "Google")
                        );

                        if (!addLoginResult.Succeeded)
                            return BadRequest("Failed to link Google login.");
                    }
                }
                else
                {
                    // ✅ Create new user
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        Firstname = firstName,
                        Lastname = lastName,
                        EmailConfirmed = true
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                        return BadRequest("Failed to create user.");

                    // ✅ Add external login after creating user
                    var addLoginResult = await _userManager.AddLoginAsync(
                        user,
                        new UserLoginInfo("Google", googleId, "Google")
                    );

                    if (!addLoginResult.Succeeded)
                        return BadRequest("Failed to link Google login.");
                }

                // ✅ Optional: Add default role
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
            }

            // ✅ Generate JWT token
            var userRoles = await _userManager.GetRolesAsync(user);
            //var token = await _authService.GenerateJWTToken(user, userRoles);

            return Ok();
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
