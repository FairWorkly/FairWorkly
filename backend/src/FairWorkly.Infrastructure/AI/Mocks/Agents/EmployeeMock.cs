namespace FairWorkly.Infrastructure.AI.Mocks.Agents;
public static class EmployeeMock
{
    public static object Welcome(object request)
    {
        return new { reply = "[MOCK AI] Hello! This is Employee Agent!" };
    }
}