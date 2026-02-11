using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Application.Awards.Interfaces;

/// <summary>
/// Projection DTO for organization awards joined with award details
/// </summary>
public class OrganizationAwardDto
{
    public AwardType AwardType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AwardCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPrimary { get; set; }
    public int EmployeeCount { get; set; }
}
