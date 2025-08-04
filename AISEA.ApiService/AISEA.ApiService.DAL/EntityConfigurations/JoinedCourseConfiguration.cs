using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class JoinedCourseConfiguration : IEntityTypeConfiguration<JoinedCourse>
{
    public void Configure(EntityTypeBuilder<JoinedCourse> builder)
    {
        builder.HasKey(e => e.Id).HasName("joinedcourse_id_primary");

        builder.HasOne(d => d.StudentProfile)
            .WithMany(p => p.JoinedCourses)
            .HasForeignKey(d => d.StudentProfileId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("joinedcourse_studentprofileid_foreign");

        // Ensure CourseName is unique per StudentProfile
        builder.HasIndex(e => new { e.StudentProfileId, e.CourseName })
            .IsUnique()
            .HasDatabaseName("IX_JoinedCourse_StudentProfileId_CourseName_Unique");
    }
}