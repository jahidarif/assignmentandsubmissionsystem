using AssignmentSubmissionSystem.Application.Common.Interfaces;

namespace AssignmentSubmissionSystem.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IUserRepository Users { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public IClassCourseRepository ClassCourses { get; }
    public ISubjectRepository Subjects { get; }
    public IClassSubjectRepository ClassSubjects { get; }
    public ITeacherAssignmentRepository TeacherAssignments { get; }
    public IEnrollmentRepository Enrollments { get; }
    public IAssignmentRepository Assignments { get; }
    public ISubmissionRepository Submissions { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IClassCourseRepository classCourseRepository,
        ISubjectRepository subjectRepository,
        IClassSubjectRepository classSubjectRepository,
        ITeacherAssignmentRepository teacherAssignmentRepository,
        IEnrollmentRepository enrollmentRepository,
        IAssignmentRepository assignmentRepository,
        ISubmissionRepository submissionRepository)
    {
        _context = context;
        Users = userRepository;
        RefreshTokens = refreshTokenRepository;
        ClassCourses = classCourseRepository;
        Subjects = subjectRepository;
        ClassSubjects = classSubjectRepository;
        TeacherAssignments = teacherAssignmentRepository;
        Enrollments = enrollmentRepository;
        Assignments = assignmentRepository;
        Submissions = submissionRepository;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}