using FairWorkly.API.Filters;
using FairWorkly.Domain.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace FairWorkly.UnitTests.Unit;

public class ResultMappingFilterTests
{
    private readonly ResultMappingFilter _filter = new();

    // ── Success mapping ──

    [Fact]
    public async Task OnActionExecution_SuccessResult_ReturnsOkObjectResultWithUnwrappedValue()
    {
        var dto = new TestDto { Name = "test" };
        var result = Result<TestDto>.Success(dto);
        var (ctx, next) = CreateFilterContexts(new ObjectResult(result));

        await _filter.OnActionExecutionAsync(ctx, next);

        var executed = await next();
        executed.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)executed.Result!;
        okResult.Value.Should().Be(dto);
        okResult.StatusCode.Should().Be(200);
    }

    // ── Failure mapping ──

    [Fact]
    public async Task OnActionExecution_ValidationFailure_Returns400WithProblemDetailsAndErrors()
    {
        var errors = new List<ValidationError>
        {
            new() { Field = "Email", Message = "Email is required" },
            new() { Field = "Email", Message = "Email must be valid" },
            new() { Field = "Password", Message = "Password is required" },
        };
        var result = Result<TestDto>.ValidationFailure(errors);
        var (ctx, next) = CreateFilterContexts(new ObjectResult(result));

        await _filter.OnActionExecutionAsync(ctx, next);

        var executed = await next();
        executed.Result.Should().BeOfType<ObjectResult>();
        var objResult = (ObjectResult)executed.Result!;
        objResult.StatusCode.Should().Be(400);

        var pd = objResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        pd.Title.Should().Be("Validation Failed");
        pd.Detail.Should().Be("One or more validation errors occurred.");
        pd.Instance.Should().Be("/api/test");
        pd.Extensions.Should().ContainKey("errors");

        var groupedErrors = pd.Extensions["errors"] as Dictionary<string, string[]>;
        groupedErrors.Should().NotBeNull();
        groupedErrors!["Email"].Should().HaveCount(2);
        groupedErrors["Password"].Should().HaveCount(1);
    }

    [Fact]
    public async Task OnActionExecution_BusinessFailure_Returns400WithProblemDetails()
    {
        var result = Result<TestDto>.Failure("Something went wrong");
        var (ctx, next) = CreateFilterContexts(new ObjectResult(result));

        await _filter.OnActionExecutionAsync(ctx, next);

        var executed = await next();
        AssertProblemDetails(executed.Result!, 400, "Bad Request", "Something went wrong");
    }

    [Fact]
    public async Task OnActionExecution_NotFound_Returns404WithProblemDetails()
    {
        var result = Result<TestDto>.NotFound("Resource not found");
        var (ctx, next) = CreateFilterContexts(new ObjectResult(result));

        await _filter.OnActionExecutionAsync(ctx, next);

        var executed = await next();
        AssertProblemDetails(executed.Result!, 404, "Resource Not Found", "Resource not found");
    }

    [Fact]
    public async Task OnActionExecution_Unauthorized_Returns401WithProblemDetails()
    {
        var result = Result<TestDto>.Unauthorized("Invalid credentials");
        var (ctx, next) = CreateFilterContexts(new ObjectResult(result));

        await _filter.OnActionExecutionAsync(ctx, next);

        var executed = await next();
        AssertProblemDetails(executed.Result!, 401, "Unauthorized", "Invalid credentials");
    }

    [Fact]
    public async Task OnActionExecution_Forbidden_Returns403WithProblemDetails()
    {
        var result = Result<TestDto>.Forbidden("Access denied");
        var (ctx, next) = CreateFilterContexts(new ObjectResult(result));

        await _filter.OnActionExecutionAsync(ctx, next);

        var executed = await next();
        AssertProblemDetails(executed.Result!, 403, "Forbidden", "Access denied");
    }

    // ── Passthrough ──

    [Fact]
    public async Task OnActionExecution_NonResultObjectResult_PassesThrough()
    {
        var original = new ObjectResult(new { foo = "bar" });
        var (ctx, next) = CreateFilterContexts(original);

        await _filter.OnActionExecutionAsync(ctx, next);

        var executed = await next();
        executed.Result.Should().BeSameAs(original);
    }

    [Fact]
    public async Task OnActionExecution_FileResult_PassesThrough()
    {
        var original = new FileContentResult(new byte[] { 1, 2, 3 }, "application/octet-stream");
        var (ctx, next) = CreateFilterContexts(original);

        await _filter.OnActionExecutionAsync(ctx, next);

        var executed = await next();
        executed.Result.Should().BeSameAs(original);
    }

    // ── Helpers ──

    private static (ActionExecutingContext ctx, ActionExecutionDelegate next) CreateFilterContexts(
        IActionResult actionResult,
        string requestPath = "/api/test")
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = requestPath;

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var executingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller: null!);

        var executedContext = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            controller: null!)
        {
            Result = actionResult,
        };

        ActionExecutionDelegate next = () => Task.FromResult(executedContext);

        return (executingContext, next);
    }

    private static void AssertProblemDetails(
        IActionResult actionResult,
        int expectedStatus,
        string expectedTitle,
        string expectedDetail)
    {
        actionResult.Should().BeOfType<ObjectResult>();
        var objResult = (ObjectResult)actionResult;
        objResult.StatusCode.Should().Be(expectedStatus);

        var pd = objResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        pd.Title.Should().Be(expectedTitle);
        pd.Detail.Should().Be(expectedDetail);
        pd.Instance.Should().Be("/api/test");
    }

    private class TestDto
    {
        public string Name { get; set; } = string.Empty;
    }
}
