using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class LeaveScheduleConfiguration : IEntityTypeConfiguration<LeaveSchedule>
{
    public void Configure(EntityTypeBuilder<LeaveSchedule> builder)
    {
        builder.HasKey(e => e.Id).HasName("leaveschedule_id_primary");

        // Unique index for StaffProfileId, StartDateTime, EndDateTime
        builder.HasIndex(e => new { e.StaffProfileId, e.StartDateTime, e.EndDateTime })
               .IsUnique()
               .HasDatabaseName("IX_LeaveSchedule_UniqueSchedule");

        builder.HasOne(d => d.StaffProfile)
            .WithMany(p => p.LeaveSchedules)
            .HasForeignKey(d => d.StaffProfileId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("leaveschedule_staffprofileid_foreign");

        // Inform EF Core of the trigger
        builder.ToTable(t =>
        {
            t.HasTrigger("TR_LeaveSchedule_CheckConstraints");
        });
    }
}