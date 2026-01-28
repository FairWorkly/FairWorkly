using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FairWorkly.Application.Auth.Features.Login;
using FairWorkly.Application.Auth.Features.Logout;
using FairWorkly.Application.Auth.Features.Me;
using FairWorkly.Application.Auth.Features.Refresh;
using FairWorkly.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace FairWorkly.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator, IWebHostEnvironment env) : ControllerBase
{
    [SwaggerRequestExample(
        typeof(LoginCommand),
        typeof(FairWorkly.API.SwaggerExamples.LoginCommandExample)
    )]
    [HttpPost("login")]
    [AllowAnonymous] // Allow anonymous access for the login endpoint
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);

        // Handle validation failures (from ValidationBehavior)
        if (result.Type == ResultType.ValidationFailure)
        {
            return await HandleValidationFailureAsync(result);
        }

        if (result.IsFailure)
        {
            if (result.Type == ResultType.Forbidden)
            {
                return StatusCode(403, new { message = result.ErrorMessage });
            }

            return Unauthorized(new { message = result.ErrorMessage });
        }

        // Put the RefreshToken into an HttpOnly cookie
        SetRefreshTokenCookie(result.Value!.RefreshToken, result.Value.RefreshTokenExpiration);

        // Return the AccessToken and user info (excluding the RefreshToken)
        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult> Refresh()
    {
        // Read refresh token from HttpOnly cookie
        if (
            !Request.Cookies.TryGetValue("refreshToken", out var refreshTokenPlain)
            || string.IsNullOrWhiteSpace(refreshTokenPlain)
        )
        {
            return Unauthorized();
        }

        var cmd = new RefreshCommand { RefreshTokenPlain = refreshTokenPlain };
        var result = await mediator.Send(cmd);

        if (result.IsFailure)
        {
            if (result.Type == ResultType.Forbidden)
            {
                return StatusCode(403, new { message = result.ErrorMessage });
            }

            return Unauthorized(new { message = result.ErrorMessage });
        }

        // Update cookie with new refresh token
        SetRefreshTokenCookie(result.Value!.RefreshToken, result.Value.RefreshTokenExpiration);

        // Return new access token
        return Ok(new { accessToken = result.Value.AccessToken });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> Me()
    {
        // Extract user id from claims (sub)
        var sub =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(sub, out var userId) || userId == Guid.Empty)
            return Unauthorized();

        var query = new GetCurrentUserQuery { UserId = userId };
        var result = await mediator.Send(query);

        if (result.IsFailure)
        {
            if (result.Type == ResultType.NotFound)
            {
                return NotFound();
            }
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Value);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        // Read refresh token from cookie (if any) before deleting
        Request.Cookies.TryGetValue("refreshToken", out var refreshTokenPlain);

        // Try get user id from access token (if present)
        var sub =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = null;
        if (Guid.TryParse(sub, out var parsed))
            userId = parsed;

        var cmd = new LogoutCommand { UserId = userId, RefreshTokenPlain = refreshTokenPlain };

        var result = await mediator.Send(cmd);

        // Remove cookie from client regardless of DB result (options must match original cookie)
        Response.Cookies.Delete("refreshToken", GetRefreshTokenCookieOptions(DateTime.UtcNow));

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return NoContent();
    }

    // --- Private helper: centralize cookie policy ---
    private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
    {
        Response.Cookies.Append(
            "refreshToken",
            refreshToken,
            GetRefreshTokenCookieOptions(expires)
        );
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

    /// <summary>
    /// Handle validation failures from ValidationBehavior - return ProblemDetails format
    /// matching the original GlobalExceptionHandler response format exactly
    /// </summary>
    private async Task<ActionResult> HandleValidationFailureAsync<T>(Result<T> result)
    {
        var errors =
            result
                .ValidationErrors?.GroupBy(e => e.Field)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray())
            ?? new Dictionary<string, string[]>();

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Failed",
            Detail = "One or more validation errors occurred.",
            Instance = HttpContext.Request.Path,
        };
        problemDetails.Extensions.Add("errors", errors);

        Response.StatusCode = StatusCodes.Status400BadRequest;
        await Response.WriteAsJsonAsync(problemDetails);
        return new EmptyResult();
    }
}
