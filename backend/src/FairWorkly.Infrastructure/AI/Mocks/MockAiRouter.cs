using FairWorkly.Infrastructure.AI.Mocks.Agents;

namespace FairWorkly.Infrastructure.AI.Mocks;

public static class MockAiRouter
{
    public static object Dispatch(string route, object request)
    {
        return route switch
        {
            // Compliance Agent
            "/chat" => ComplianceMock.Chat(request),

            // Employee Agent
            "/employee/welcome" => EmployeeMock.Welcome(request),

            _ => throw new NotImplementedException($"Mock route '{route}' is not configured.")
        };
    }
}