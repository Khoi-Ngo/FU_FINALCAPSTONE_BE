using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class AdvisorySession1to1Configuration : IEntityTypeConfiguration<AdvisorySession1to1>
    {
        public void Configure(EntityTypeBuilder<AdvisorySession1to1> builder)
        {
            builder.HasKey(e => e.Id).HasName("advisorysession1to1_id_primary");

            builder.HasIndex(e => e.StaffId).HasDatabaseName("IX_AdvisorySession1to1_StaffId");
            builder.HasIndex(e => e.StudentId).HasDatabaseName("IX_AdvisorySession1to1_StudentId");
            builder.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_AdvisorySession1to1_CreatedAt");

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasOne(d => d.Staff)
                .WithMany(p => p.AdvisorySessions1to1)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("advisorysession1to1_staffid_foreign");

            builder.HasOne(d => d.Student)
                .WithMany(p => p.AdvisorySessions1to1)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("advisorysession1to1_studentid_foreign");
        }
    }
}