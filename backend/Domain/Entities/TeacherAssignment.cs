using AssignmentSubmissionSystem.Domain.Entities.Common;

namespace AssignmentSubmissionSystem.Domain.Entities;

public class TeacherAssignment : BaseEntity
{
    public Guid TeacherId { get; set; }
    public User Teacher { get; set; } = null!;

    public Guid ClassSubjectId { get; set; }
    public ClassSubject ClassSubject { get; set; } = null!;
}