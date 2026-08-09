using AssignmentSubmissionSystem.Application.Common.Interfaces;
using Moq;

namespace Tests.Common;

public class MockUnitOfWorkBuilder
{
    public Mock<IUserRepository> Users { get; } = new();
    public Mock<IRefreshTokenRepository> RefreshTokens { get; } = new();
    public Mock<IClassCourseRepository> ClassCourses { get; } = new();
    public Mock<ISubjectRepository> Subjects { get; } = new();
    public Mock<IClassSubjectRepository> ClassSubjects { get; } = new();
    public Mock<ITeacherAssignmentRepository> TeacherAssignments { get; } = new();
    public Mock<IEnrollmentRepository> Enrollments { get; } = new();
    public Mock<IAssignmentRepository> Assignments { get; } = new();
    public Mock<ISubmissionRepository> Submissions { get; } = new();

    public Mock<IUnitOfWork> Build()
    {
        var unitOfWork = new Mock<IUnitOfWork>();

        unitOfWork.SetupGet(u => u.Users).Returns(Users.Object);
        unitOfWork.SetupGet(u => u.RefreshTokens).Returns(RefreshTokens.Object);
        unitOfWork.SetupGet(u => u.ClassCourses).Returns(ClassCourses.Object);
        unitOfWork.SetupGet(u => u.Subjects).Returns(Subjects.Object);
        unitOfWork.SetupGet(u => u.ClassSubjects).Returns(ClassSubjects.Object);
        unitOfWork.SetupGet(u => u.TeacherAssignments).Returns(TeacherAssignments.Object);
        unitOfWork.SetupGet(u => u.Enrollments).Returns(Enrollments.Object);
        unitOfWork.SetupGet(u => u.Assignments).Returns(Assignments.Object);
        unitOfWork.SetupGet(u => u.Submissions).Returns(Submissions.Object);
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        return unitOfWork;
    }
}