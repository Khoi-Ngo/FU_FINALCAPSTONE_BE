using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class DelayJoinedCourseConfiguration : IEntityTypeConfiguration<DelayJoinedCourse>
{
    public void Configure(EntityTypeBuilder<DelayJoinedCourse> builder)
    {
        builder.HasKey(e => e.Id).HasName("delayjoinedcourse_id_primary");

        builder.HasOne(d => d.StudentProfile)
            .WithMany(p => p.DelayJoinedCourses)
            .HasForeignKey(d => d.StudentProfileId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("delayjoinedcourse_studentprofileid_foreign");
    }
}