namespace AssignmentSubmissionSystem.Application.Features.Student.Dtos;

public class StudentAssignmentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int MaxMarks { get; set; }
    public string ClassCourseName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public bool HasSubmitted { get; set; }
    public bool IsPastDeadline { get; set; }
}

public class StudentSubmissionDto
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public DateTime AssignmentDeadline { get; set; }
    public int AssignmentMaxMarks { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? Marks { get; set; }
    public string? Feedback { get; set; }
    public DateTime? GradedAt { get; set; }
    public bool CanUpdate { get; set; }
}

public class SubmitAssignmentRequest
{
    public string AnswerText { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
}

public class UpdateSubmissionRequest
{
    public string AnswerText { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
}