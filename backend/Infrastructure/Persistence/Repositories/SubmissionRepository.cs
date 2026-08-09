using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSubmissionSystem.Infrastructure.Persistence.Repositories;

public class SubmissionRepository : Repository<Submission>, ISubmissionRepository
{
    public SubmissionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(List<Submission> Items, int TotalCount)> GetPagedByAssignmentAsync(Guid assignmentId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Include(s => s.Student).Where(s => s.AssignmentId == assignmentId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Submission?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<(List<Submission> Items, int TotalCount)> GetPagedByStudentAsync(Guid studentId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Include(s => s.Assignment).Where(s => s.StudentId == studentId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId, cancellationToken);
}