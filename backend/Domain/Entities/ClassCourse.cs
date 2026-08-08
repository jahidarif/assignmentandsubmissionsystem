using AssignmentSubmissionSystem.Domain.Entities.Common;

namespace AssignmentSubmissionSystem.Domain.Entities;

public class ClassCourse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Section { get; set; }

    public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}