using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers;

/// <summary>
/// Base controller for all API controllers.
/// Provides <see cref="RespondResult{T}"/> to map <c>Result&lt;T&gt;</c> to HTTP responses
/// with a unified <c>{ code, msg, data }</c> JSON format.
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Maps a <c>Result&lt;T&gt;</c> to the appropriate HTTP response.
    /// </summary>
    /// <remarks>
    /// <para>Three response shapes:</para>
    /// <list type="bullet">
    ///   <item><b>2xx success</b>: <c>{ code, msg, data }</c> (204 returns no body)</item>
    ///   <item><b>4xx with errors</b> (400, 422): <c>{ code, msg, data: { errors } }</c></item>
    ///   <item><b>4xx without errors</b> (401, 403, 404, 409): <c>{ code, msg }</c></item>
    /// </list>
    /// </remarks>
    /// <param name="result">The Result from the Handler.</param>
    /// <param name="successMessage">Message for 2xx responses (default: "Success").</param>
    protected IActionResult RespondResult<T>(Result<T> result, string successMessage = "Success")
    {
        // 2xx success
        if (result.IsSuccess)
        {
            if (result.Code == 204)
                return NoContent();

            return StatusCode(result.Code, new { code = result.Code, msg = successMessage, data = result.Value });
        }

        // 4xx with structured errors (400, 422)
        if (result.Errors != null)
            return StatusCode(result.Code, new { code = result.Code, msg = result.Message, data = new { errors = result.Errors } });

        // 4xx without errors (401, 403, 404, 409)
        return StatusCode(result.Code, new { code = result.Code, msg = result.Message });
    }
}
