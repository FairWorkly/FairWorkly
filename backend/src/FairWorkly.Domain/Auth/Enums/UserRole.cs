namespace FairWorkly.Domain.Auth.Enums;

public enum UserRole
{
    Unknown = 0, // Default — must never be persisted; catch-all for validation
    Admin = 1, // Business owner - full system access
    Manager = 2, // Department/store manager - team management only
}
