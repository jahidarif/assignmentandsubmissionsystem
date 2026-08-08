using AssignmentSubmissionSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssignmentSubmissionSystem.Infrastructure.Persistence.Configurations;

public class ClassSubjectConfiguration : IEntityTypeConfiguration<ClassSubject>
{
    public void Configure(EntityTypeBuilder<ClassSubject> builder)
    {
        builder.ToTable("ClassSubjects");

        builder.HasKey(cs => cs.Id);

        builder.HasIndex(cs => new { cs.ClassCourseId, cs.SubjectId }).IsUnique();

        builder.HasOne(cs => cs.ClassCourse)
            .WithMany(c => c.ClassSubjects)
            .HasForeignKey(cs => cs.ClassCourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cs => cs.Subject)
            .WithMany(s => s.ClassSubjects)
            .HasForeignKey(cs => cs.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}