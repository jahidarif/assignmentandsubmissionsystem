using AssignmentSubmissionSystem.Domain.Entities.Common;

namespace AssignmentSubmissionSystem.Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;

    public Guid ClassCourseId { get; set; }
    public ClassCourse ClassCourse { get; set; } = null!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}