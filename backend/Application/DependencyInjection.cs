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

        return services;
    }
}