using AssignmentSubmissionSystem.Domain.Entities.Common;
using AssignmentSubmissionSystem.Domain.Enums;

namespace AssignmentSubmissionSystem.Domain.Entities;

public class Submission : BaseEntity
{
    public Guid AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;

    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;

    public string AnswerText { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;

    public int? Marks { get; set; }
    public string? Feedback { get; set; }
    public DateTime? GradedAt { get; set; }

    public Guid? GradedByTeacherId { get; set; }
    public User? GradedByTeacher { get; set; }
}