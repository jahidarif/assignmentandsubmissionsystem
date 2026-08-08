using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSubmissionSystem.Infrastructure.Persistence.Repositories;

public class ClassSubjectRepository : Repository<ClassSubject>, IClassSubjectRepository
{
    public ClassSubjectRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(List<ClassSubject> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? classCourseId, CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(cs => cs.ClassCourse)
            .Include(cs => cs.Subject)
            .AsQueryable();

        if (classCourseId.HasValue)
        {
            query = query.Where(cs => cs.ClassCourseId == classCourseId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(cs => cs.ClassCourse.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid classCourseId, Guid subjectId, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(cs => cs.ClassCourseId == classCourseId && cs.SubjectId == subjectId, cancellationToken);

    public async Task<List<ClassSubject>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        => await DbSet
            .Include(cs => cs.ClassCourse)
            .Include(cs => cs.Subject)
            .OrderBy(cs => cs.ClassCourse.Name)
            .ToListAsync(cancellationToken);
}