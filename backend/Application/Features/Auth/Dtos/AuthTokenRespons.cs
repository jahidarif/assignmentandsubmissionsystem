namespace AssignmentSubmissionSystem.Application.Features.Auth.Dtos;

public class AuthTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }

    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>"Admin", "Teacher", or "Student" — single value, since a
    /// user has exactly one role in this schema.</summary>
    public string Role { get; set; } = string.Empty;
}