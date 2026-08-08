namespace AssignmentSubmissionSystem.Application.Features.Admin.Overview.Dtos;

public class AdminAssignmentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ClassCourseName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class AdminSubmissionDto
{
    public Guid Id { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? Marks { get; set; }
}