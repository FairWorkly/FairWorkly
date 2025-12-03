namespace FairWorkly.Infrastructure.AI.Mocks;

public static class MockAiRouter
{
    public static object Dispatch(string route, object request)
    {
        return route switch
        {
            _ => throw new NotImplementedException($"Mock route '{route}' is not configured."),
        };
    }
}
