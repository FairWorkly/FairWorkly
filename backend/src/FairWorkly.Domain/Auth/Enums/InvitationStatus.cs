namespace FairWorkly.Domain.Auth.Enums;

public enum InvitationStatus
{
    None = 0, // Self-registered or OAuth user (not invited)
    Pending = 1, // Invitation sent, not yet accepted
    Accepted = 2, // Invitation accepted, password set
}
