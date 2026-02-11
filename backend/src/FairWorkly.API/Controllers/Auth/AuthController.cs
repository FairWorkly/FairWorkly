using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FairWorkly.Application.Auth.Features.ForgotPassword;
using FairWorkly.Application.Auth.Features.Login;
using FairWorkly.Application.Auth.Features.Logout;
using FairWorkly.Application.Auth.Features.Me;
using FairWorkly.Application.Auth.Features.Refresh;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace FairWorkly.API.Controllers.Auth;

// TODO: [Refactor] Migrate manual JWT claim extraction (User.FindFirstValue) to ICurrentUserService.
// ICurrentUserService is already registered in DI and used by RosterController.
// AuthController is special (has AllowAnonymous endpoints), so only migrate Me() and Logout().
// Other controllers needing userId/orgId should inject ICurrentUserService directly.
// See: Infrastructure/Services/CurrentUserService.cs, RosterController.cs for reference.
[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [SwaggerRequestExample(
        typeof(LoginCommand),
        typeof(FairWorkly.API.SwaggerExamples.LoginCommandExample)
    )]
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return new ObjectResult(result);

        SetRefreshTokenCookie(result.Value!.RefreshToken, result.Value.RefreshTokenExpiration);

        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
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
            return new ObjectResult(result);

        SetRefreshTokenCookie(result.Value!.RefreshToken, result.Value.RefreshTokenExpiration);

        return Ok(new { accessToken = result.Value.AccessToken });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return new ObjectResult(result);

        return Ok(new { message = "If that email exists, a reset link has been sent." });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var sub =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(sub, out var userId) || userId == Guid.Empty)
            return Unauthorized();

        var query = new GetCurrentUserQuery { UserId = userId };
        var result = await mediator.Send(query);

        return new ObjectResult(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        Request.Cookies.TryGetValue("refreshToken", out var refreshTokenPlain);

        var sub =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = null;
        if (Guid.TryParse(sub, out var parsed))
            userId = parsed;

        var cmd = new LogoutCommand { UserId = userId, RefreshTokenPlain = refreshTokenPlain };
        var result = await mediator.Send(cmd);

        Response.Cookies.Delete("refreshToken", GetRefreshTokenCookieOptions(DateTime.UtcNow));

        if (result.IsFailure)
            return new ObjectResult(result);

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
            Path = "/",

            // Use Secure cookies and SameSite=None to allow cross-site refresh in dev
            // (required for 5173 -> 7075 when using HttpOnly refresh cookies)
            Secure = true,
            SameSite = SameSiteMode.None,
        };
    }

}
