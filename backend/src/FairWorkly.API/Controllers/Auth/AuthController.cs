using FairWorkly.Application.Auth.Features.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator, IWebHostEnvironment env) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous] // Allow anonymous access for the login endpoint
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);

        if (result == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        // Put the RefreshToken into an HttpOnly cookie
        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);

        // Return the AccessToken and user info (excluding the RefreshToken)
        return Ok(result);
    }

    // --- Private helper: centralize cookie policy ---
    private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // prevent JS access (XSS protection)
            Expires = expires,

            // Local development (http) -> Secure=false
            // Production (https) -> Secure=true
            Secure = env.IsProduction(),

            // Set cross-domain to None in production environment, and Lax in development environment
            SameSite = env.IsProduction() ? SameSiteMode.None : SameSiteMode.Lax,
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
