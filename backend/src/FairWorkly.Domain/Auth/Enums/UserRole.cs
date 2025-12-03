namespace FairWorkly.Domain.Auth.Enums;

/// <summary>
/// User roles - simplified for MVP
/// </summary>
public enum UserRole
{
    Admin = 1, // Business owner or HR - full system access
    Manager = 2, // Department/store manager - team management only
    Employee = 3, // Regular employee - self-service portal only
}
