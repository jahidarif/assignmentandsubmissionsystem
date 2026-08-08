namespace AssignmentSubmissionSystem.Application.Features.Admin.TeacherAssignments.Dtos;

public class TeacherAssignmentDto
{
    public Guid Id { get; set; }
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string TeacherEmail { get; set; } = string.Empty;
    public Guid ClassSubjectId { get; set; }
    public string ClassCourseName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
}

public class CreateTeacherAssignmentRequest
{
    public Guid TeacherId { get; set; }
    public Guid ClassSubjectId { get; set; }
}