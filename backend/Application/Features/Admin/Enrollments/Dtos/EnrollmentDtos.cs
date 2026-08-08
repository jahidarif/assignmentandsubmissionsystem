namespace AssignmentSubmissionSystem.Application.Features.Admin.Enrollments.Dtos;

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public Guid ClassCourseId { get; set; }
    public string ClassCourseName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
}

public class CreateEnrollmentRequest
{
    public Guid StudentId { get; set; }
    public Guid ClassCourseId { get; set; }
}