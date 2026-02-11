namespace FairWorkly.Application.Awards.Features.GetOrganizationAwards;

public class GetOrganizationAwardsResponse
{
    public List<OrganizationAwardItem> Awards { get; set; } = new();
}

public class OrganizationAwardItem
{
    public string AwardType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AwardCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPrimary { get; set; }
    public int EmployeeCount { get; set; }
}
