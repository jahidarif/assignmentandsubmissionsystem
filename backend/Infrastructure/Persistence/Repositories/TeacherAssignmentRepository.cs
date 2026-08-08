using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSubmissionSystem.Infrastructure.Persistence.Repositories;

public class TeacherAssignmentRepository : Repository<TeacherAssignment>, ITeacherAssignmentRepository
{
    public TeacherAssignmentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(List<TeacherAssignment> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? teacherId, CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(ta => ta.Teacher)
            .Include(ta => ta.ClassSubject).ThenInclude(cs => cs.ClassCourse)
            .Include(ta => ta.ClassSubject).ThenInclude(cs => cs.Subject)
            .AsQueryable();

        if (teacherId.HasValue)
        {
            query = query.Where(ta => ta.TeacherId == teacherId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(ta => ta.Teacher.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid teacherId, Guid classSubjectId, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(ta => ta.TeacherId == teacherId && ta.ClassSubjectId == classSubjectId, cancellationToken);
}