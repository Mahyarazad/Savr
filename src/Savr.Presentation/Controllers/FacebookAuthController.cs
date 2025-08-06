using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Savr.Identity.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Savr.Presentation.Controllers
{
    [ApiController]
    [Route("auth")]
    public class FacebookAuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FacebookAuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("facebook-login")]
        public IActionResult LoginWithFacebook()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("FacebookCallback", "FacebookAuth")
            };

            return Challenge(props, "Facebook");
        }

        [HttpGet("facebook-callback")]
        public async Task<IActionResult> FacebookCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return Unauthorized("Facebook authentication failed.");

            var fbClaims = result.Principal.Claims;

            var email = fbClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = fbClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var facebookId = fbClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(facebookId))
                return BadRequest("Email or Facebook ID missing.");

            var firstName = name?.Split(' ').FirstOrDefault() ?? "";
            var lastName = name?.Split(' ').Skip(1).FirstOrDefault() ?? "";

            // Try to find the user by external login
            var user = await _userManager.FindByLoginAsync("Facebook", facebookId);

            if (user == null)
            {
                // Fallback: find by email
                user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var logins = await _userManager.GetLoginsAsync(user);
                    if (!logins.Any(l => l.LoginProvider == "Facebook"))
                    {
                        var addLoginResult = await _userManager.AddLoginAsync(
                            user,
                            new UserLoginInfo("Facebook", facebookId, "Facebook")
                        );

                        if (!addLoginResult.Succeeded)
                            return BadRequest("Failed to link Facebook login.");
                    }
                }
                else
                {
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

                    var addLoginResult = await _userManager.AddLoginAsync(
                        user,
                        new UserLoginInfo("Facebook", facebookId, "Facebook")
                    );

                    if (!addLoginResult.Succeeded)
                        return BadRequest("Failed to link Facebook login.");
                }

                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = GenerateJWTToken(user, userRoles);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            HttpContext.Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return Redirect("https://localhost:3000/login-success");
        }

        [HttpGet("facebook-login-failed")]
        public IActionResult LoginFailed([FromQuery] string? error = "Unknown error")
        {
            Log.Warning("Facebook login failed: {Error}", error);
            return BadRequest(new
            {
                message = "Facebook sign-in failed",
                error = error
            });
        }

        private JwtSecurityToken GenerateJWTToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("Uid", user.Id),
                new Claim("Name", user.Firstname ?? "")
            };

            if (roles.Count > 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, roles[0]));
            }

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signingCredentials
            );
        }
    }
}
