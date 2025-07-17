using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class MarkReportConfiguration : IEntityTypeConfiguration<MarkReport>
{
    public void Configure(EntityTypeBuilder<MarkReport> builder)
    {
        builder.HasKey(e => e.Id).HasName("markreport_id_primary");
        builder.HasIndex(e => e.StudentProfileId).HasDatabaseName("IX_MarkReport_StudentProfileId");
        builder.HasOne(e => e.StudentProfile).WithMany().HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("markreport_studentprofileid_foreign");
        builder.Property(e => e.Mark).HasColumnType("decimal(4,2)"); // Adjusted for range 0.00 to 10.00
    }
}