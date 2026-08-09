using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSubmissionSystem.Infrastructure.Persistence.Repositories;

public class AssignmentRepository : Repository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(List<Assignment> Items, int TotalCount)> GetPagedByTeacherAsync(Guid teacherId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(a => a.ClassSubject).ThenInclude(cs => cs.ClassCourse)
            .Include(a => a.ClassSubject).ThenInclude(cs => cs.Subject)
            .Where(a => a.TeacherId == teacherId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Assignment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet
            .Include(a => a.ClassSubject).ThenInclude(cs => cs.ClassCourse)
            .Include(a => a.ClassSubject).ThenInclude(cs => cs.Subject)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}