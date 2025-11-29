namespace FairWorkly.Domain.Employees.Dtos;

public class CreateEmployeeRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public decimal BaseHourlyRate { get; set; }
}