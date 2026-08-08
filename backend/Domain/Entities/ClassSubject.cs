using AssignmentSubmissionSystem.Domain.Entities.Common;

namespace AssignmentSubmissionSystem.Domain.Entities;

public class ClassSubject : BaseEntity
{
    public Guid ClassCourseId { get; set; }
    public ClassCourse ClassCourse { get; set; } = null!;

    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}