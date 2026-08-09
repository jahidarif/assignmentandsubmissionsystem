using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSubmissionSystem.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(List<Enrollment> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? classCourseId, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Include(e => e.Student).Include(e => e.ClassCourse).AsQueryable();

        if (classCourseId.HasValue)
        {
            query = query.Where(e => e.ClassCourseId == classCourseId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(e => e.ClassCourse.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid studentId, Guid classCourseId, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(e => e.StudentId == studentId && e.ClassCourseId == classCourseId, cancellationToken);

    public async Task<bool> IsStudentEnrolledInAssignmentClassAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(e =>
            e.StudentId == studentId &&
            Context.Set<Assignment>().Any(a => a.Id == assignmentId && a.ClassSubject.ClassCourseId == e.ClassCourseId),
            cancellationToken);
}