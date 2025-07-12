using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class BookingAvailabilityConfiguration : IEntityTypeConfiguration<BookingAvailability>
{
    public void Configure(EntityTypeBuilder<BookingAvailability> builder)
    {
        builder.HasKey(e => e.Id).HasName("bookingavailability_id_primary");

        // Unique constraint on StaffProfileId, DayInWeek, StartTime, EndTime
        builder.HasIndex(e => new { e.StaffProfileId, e.DayInWeek, e.StartTime, e.EndTime })
               .IsUnique()
               .HasDatabaseName("IX_BookingAvailability_UniqueTimeSlot");

        builder.HasOne(d => d.StaffProfile)
            .WithMany(p => p.BookingAvailabilities)
            .HasForeignKey(d => d.StaffProfileId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("bookingavailability_staffprofileid_foreign");

        // Database check constraint for EndTime > StartTime
        builder.ToTable(t =>
          {
              t.HasCheckConstraint("CK_BookingAvailability_EndTime", "[EndTime] > [StartTime]");
              t.HasTrigger("TR_BookingAvailability_CheckOverlap"); // Inform EF Core of the trigger to avoid OUTPUT clause
          });
    }
}