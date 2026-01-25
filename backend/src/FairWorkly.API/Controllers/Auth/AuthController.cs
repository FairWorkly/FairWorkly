using FairWorkly.Application.Auth.Features.Login;
using FairWorkly.Application.Auth.Features.Refresh;
using FairWorkly.Application.Auth.Features.Me;
using FairWorkly.Application.Auth.Features.Logout;

using MediatR;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator, IWebHostEnvironment env) : ControllerBase
{
    [SwaggerRequestExample(typeof(LoginCommand), typeof(FairWorkly.API.SwaggerExamples.LoginCommandExample))]
    [HttpPost("login")]
    [AllowAnonymous] // Allow anonymous access for the login endpoint
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);

        if (result == null || result.Response == null)
        {
            if (result?.FailureReason == LoginFailureReason.AccountDisabled)
            {
                return StatusCode(403, new { message = "Account is disabled." });
            }

            return Unauthorized(new { message = "Invalid email or password." });
        }

        // Put the RefreshToken into an HttpOnly cookie
        SetRefreshTokenCookie(result.Response.RefreshToken, result.Response.RefreshTokenExpiration);

        // Return the AccessToken and user info (excluding the RefreshToken)
        return Ok(result.Response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult> Refresh()
    {
        // Read refresh token from HttpOnly cookie
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshTokenPlain) || string.IsNullOrWhiteSpace(refreshTokenPlain))
        {
            return Unauthorized();
        }

        var cmd = new RefreshCommand { RefreshTokenPlain = refreshTokenPlain };
        var res = await mediator.Send(cmd);
        if (res == null || res.Response == null)
        {
            if (res?.FailureReason == RefreshFailureReason.AccountDisabled)
            {
                return StatusCode(403, new { message = "Account is disabled." });
            }

            if (res?.FailureReason == RefreshFailureReason.ExpiredToken)
            {
                return Unauthorized(new { message = "Refresh token expired." });
            }

            return Unauthorized(new { message = "Invalid refresh token." });
        }

        // Update cookie with new refresh token
        SetRefreshTokenCookie(res.Response.RefreshToken, res.Response.RefreshTokenExpiration);

        // Return new access token
        return Ok(new { accessToken = res.Response.AccessToken });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> Me()
    {
        // Extract user id from claims (sub)
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(sub, out var userId) || userId == Guid.Empty) return Unauthorized();

        var query = new GetCurrentUserQuery { UserId = userId };
        var user = await mediator.Send(query);
        if (user == null) return NotFound();

        return Ok(user);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        // Read refresh token from cookie (if any) before deleting
        Request.Cookies.TryGetValue("refreshToken", out var refreshTokenPlain);

        // Try get user id from access token (if present)
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = null;
        if (Guid.TryParse(sub, out var parsed)) userId = parsed;

        var cmd = new LogoutCommand
        {
            UserId = userId,
            RefreshTokenPlain = refreshTokenPlain
        };

        var result = await mediator.Send(cmd);

        // Remove cookie from client regardless of DB result (options must match original cookie)
        Response.Cookies.Delete("refreshToken", GetRefreshTokenCookieOptions(DateTime.UtcNow));

        if (!result)
        {
            return BadRequest(new { message = "Logout failed." });
        }

        return NoContent();
    }

    // --- Private helper: centralize cookie policy ---
    private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
    {
        Response.Cookies.Append("refreshToken", refreshToken, GetRefreshTokenCookieOptions(expires));
    }

    private CookieOptions GetRefreshTokenCookieOptions(DateTime expires)
    {
        return new CookieOptions
        {
            HttpOnly = true, // prevent JS access (XSS protection)
            Expires = expires,

            // Local development (http) -> Secure=false
            // Production (https) -> Secure=true
            Secure = env.IsProduction(),

            // Set cross-domain to None in production environment, and Lax in development environment
            SameSite = env.IsProduction() ? SameSiteMode.None : SameSiteMode.Lax,
        };
    }

}
