using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
    {
        public void Configure(EntityTypeBuilder<StudentProfile> builder)
        {
            builder.HasKey(e => e.Id).HasName("studentprofile_id_primary");

            builder.HasIndex(e => e.UserId).IsUnique().HasDatabaseName("IX_StudentProfile_UserId");

            builder.Property(e => e.CareerGoal)
                .HasColumnType("text");

            builder.HasOne(d => d.User)
                .WithOne(p => p.StudentProfile)
                .HasForeignKey<StudentProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("studentprofile_userid_foreign");
        }
    }
}