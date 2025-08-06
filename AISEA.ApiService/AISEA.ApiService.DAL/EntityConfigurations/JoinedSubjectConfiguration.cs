using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class JoinedSubjectConfiguration : IEntityTypeConfiguration<JoinedSubject>
{
    public void Configure(EntityTypeBuilder<JoinedSubject> builder)
    {
        builder.HasKey(e => e.Id).HasName("joinedsubject_id_primary");

        builder.HasOne(d => d.StudentProfile)
            .WithMany(p => p.JoinedCourses)
            .HasForeignKey(d => d.StudentProfileId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("joinedsubject_studentprofileid_foreign");

        // Ensure Name is unique per StudentProfile
        builder.HasIndex(e => new { e.StudentProfileId, e.Name })
            .IsUnique()
            .HasDatabaseName("IX_JoinedSubject_StudentProfileId_Name_Unique");

        // Relationship with Semester
        builder.HasOne(d => d.Semester)
            .WithMany(p => p.JoinedCourses)
            .HasForeignKey(d => d.SemesterId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("joinedsubject_semesterid_foreign");
    }
}