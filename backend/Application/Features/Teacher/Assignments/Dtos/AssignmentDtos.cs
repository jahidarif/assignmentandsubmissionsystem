using AssignmentSubmissionSystem.Domain.Enums;

namespace AssignmentSubmissionSystem.Application.Features.Teacher.Assignments.Dtos;

public class TeacherAssignmentListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int MaxMarks { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid ClassSubjectId { get; set; }
    public string ClassCourseName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
}

public class CreateAssignmentRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int MaxMarks { get; set; }
    public Guid ClassSubjectId { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;
}

public class UpdateAssignmentRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int MaxMarks { get; set; }
}

public class TeacherClassSubjectDto
{
    public Guid Id { get; set; }
    public string ClassCourseName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
}