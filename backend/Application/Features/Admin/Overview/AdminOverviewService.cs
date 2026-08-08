using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.Overview.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSubmissionSystem.Application.Features.Admin.Overview;

public class AdminOverviewService : IAdminOverviewService
{
    private const int PageSize = 10;

    private readonly IApplicationDbContext _context;

    public AdminOverviewService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminAssignmentDto>> GetAllAssignmentsAsync(int page, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;

        var query = _context.Assignments
            .Include(a => a.ClassSubject).ThenInclude(cs => cs.ClassCourse)
            .Include(a => a.ClassSubject).ThenInclude(cs => cs.Subject)
            .Include(a => a.Teacher)
            .OrderByDescending(a => a.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(a => new AdminAssignmentDto
            {
                Id = a.Id,
                Title = a.Title,
                ClassCourseName = a.ClassSubject.ClassCourse.Name,
                SubjectName = a.ClassSubject.Subject.Name,
                TeacherName = a.Teacher.FullName,
                Deadline = a.Deadline,
                Status = a.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminAssignmentDto>
        {
            Items = items,
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<AdminSubmissionDto>> GetAllSubmissionsAsync(int page, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;

        var query = _context.Submissions
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .OrderByDescending(s => s.SubmittedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(s => new AdminSubmissionDto
            {
                Id = s.Id,
                AssignmentTitle = s.Assignment.Title,
                StudentName = s.Student.FullName,
                SubmittedAt = s.SubmittedAt,
                Status = s.Status.ToString(),
                Marks = s.Marks
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminSubmissionDto>
        {
            Items = items,
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }
}