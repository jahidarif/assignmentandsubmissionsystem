using System.Reflection;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassCourses;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassSubjects;
using AssignmentSubmissionSystem.Application.Features.Admin.Enrollments;
using AssignmentSubmissionSystem.Application.Features.Admin.Overview;
using AssignmentSubmissionSystem.Application.Features.Admin.Subjects;
using AssignmentSubmissionSystem.Application.Features.Admin.TeacherAssignments;
using AssignmentSubmissionSystem.Application.Features.Admin.Users;
using AssignmentSubmissionSystem.Application.Features.Auth;
using AssignmentSubmissionSystem.Application.Features.Teacher.Assignments;
using AssignmentSubmissionSystem.Application.Features.Teacher.Submissions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AssignmentSubmissionSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IClassCourseService, ClassCourseService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IClassSubjectService, ClassSubjectService>();
        services.AddScoped<ITeacherAssignmentService, TeacherAssignmentService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        services.AddScoped<IAdminOverviewService, AdminOverviewService>();
        services.AddScoped<IAssignmentManagementService, AssignmentManagementService>();
        services.AddScoped<ISubmissionGradingService, SubmissionGradingService>();

        return services;
    }
}