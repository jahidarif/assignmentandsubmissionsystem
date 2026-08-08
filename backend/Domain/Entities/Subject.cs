using AssignmentSubmissionSystem.Domain.Entities.Common;

namespace AssignmentSubmissionSystem.Domain.Entities;

public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
}