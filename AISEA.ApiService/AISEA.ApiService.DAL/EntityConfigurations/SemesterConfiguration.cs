using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
    public void Configure(EntityTypeBuilder<Semester> builder)
    {
        builder.HasKey(e => e.Id).HasName("semester_id_primary");

        builder.Property(e => e.SemesterName)
            .IsRequired()
            .HasMaxLength(50); // Adjust max length as needed


        // Configure the relationship with JoinedCourse
        builder.HasMany(e => e.JoinedCourses)
            .WithOne(e => e.Semester)
            .HasForeignKey(e => e.SemesterId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("joinedcourse_semesterid_foreign");
    }
}