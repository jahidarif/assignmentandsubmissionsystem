namespace AssignmentSubmissionSystem.Application.Features.Admin.ClassSubjects.Dtos;

public class ClassSubjectDto
{
    public Guid Id { get; set; }
    public Guid ClassCourseId { get; set; }
    public string ClassCourseName { get; set; } = string.Empty;
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
}

public class CreateClassSubjectRequest
{
    public Guid ClassCourseId { get; set; }
    public Guid SubjectId { get; set; }
}