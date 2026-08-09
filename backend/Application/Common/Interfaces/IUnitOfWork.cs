namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IClassCourseRepository ClassCourses { get; }
    ISubjectRepository Subjects { get; }
    IClassSubjectRepository ClassSubjects { get; }
    ITeacherAssignmentRepository TeacherAssignments { get; }
    IEnrollmentRepository Enrollments { get; }
    IAssignmentRepository Assignments { get; }
    ISubmissionRepository Submissions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}