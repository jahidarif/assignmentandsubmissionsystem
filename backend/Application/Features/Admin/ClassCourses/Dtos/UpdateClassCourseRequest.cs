namespace AssignmentSubmissionSystem.Application.Features.Admin.ClassCourses.Dtos;

public class UpdateClassCourseRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Section { get; set; }
}