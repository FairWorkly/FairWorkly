namespace FairWorkly.Infrastructure.AI.Mocks.Agents;

public static class ComplianceMock
{
    public static object Chat(object request)
    {
        return new
        {
            reply = "[MOCK AI]: According to the Fair Work Act, your employees should be paid 150% of their regular wages for weekend overtime work."
        };
    }

}