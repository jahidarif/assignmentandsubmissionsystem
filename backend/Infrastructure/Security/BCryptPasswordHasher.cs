using AssignmentSubmissionSystem.Application.Common.Interfaces;

namespace AssignmentSubmissionSystem.Infrastructure.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string plainTextPassword)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword))
            throw new ArgumentException("Password cannot be empty.", nameof(plainTextPassword));

        return BCrypt.Net.BCrypt.HashPassword(plainTextPassword, workFactor: WorkFactor);
    }

    public bool Verify(string plainTextPassword, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        return BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
    }
}