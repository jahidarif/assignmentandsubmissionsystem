namespace AssignmentSubmissionSystem.Application.Features.Admin.ClassCourses.Dtos;

public class ClassCourseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Section { get; set; }
}