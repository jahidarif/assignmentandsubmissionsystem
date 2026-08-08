using AssignmentSubmissionSystem.Domain.Entities.Common;
using AssignmentSubmissionSystem.Domain.Enums;

namespace AssignmentSubmissionSystem.Domain.Entities;

public class Assignment : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int MaxMarks { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;

    public Guid ClassSubjectId { get; set; }
    public ClassSubject ClassSubject { get; set; } = null!;

    public Guid TeacherId { get; set; }
    public User Teacher { get; set; } = null!;

    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}