using FairWorkly.Application.Auth.Features.Login;
using Swashbuckle.AspNetCore.Filters;

namespace FairWorkly.API.SwaggerExamples;

public class LoginCommandExample : IExamplesProvider<LoginCommand>
{
    public LoginCommand GetExamples() =>
        new LoginCommand { Email = "admin@fairworkly.com.au", Password = "fairworkly123" };
}
