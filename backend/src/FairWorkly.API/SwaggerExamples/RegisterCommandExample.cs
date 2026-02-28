using FairWorkly.Application.Auth.Features.Register;
using Swashbuckle.AspNetCore.Filters;

namespace FairWorkly.API.SwaggerExamples;

public class RegisterCommandExample : IExamplesProvider<RegisterCommand>
{
    public RegisterCommand GetExamples() =>
        new RegisterCommand
        {
            CompanyName = "Acme Pty Ltd",
            Abn = "12345678902",
            IndustryType = "Retail",
            AddressLine1 = "123 Demo St",
            AddressLine2 = "Level 1",
            Suburb = "Melbourne",
            State = "VIC",
            Postcode = "3000",
            ContactEmail = "contact@acme.com.au",
            Email = "owner@acme.com.au",
            Password = "Test1234",
            FirstName = "Owner",
            LastName = "User",
        };
}
