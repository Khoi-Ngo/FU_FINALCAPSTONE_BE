using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class BookingAvailabilityConfiguration : IEntityTypeConfiguration<BookingAvailability>
{
    public void Configure(EntityTypeBuilder<BookingAvailability> builder)
    {
        builder.HasKey(e => e.Id).HasName("bookingavailability_id_primary");

        builder.HasOne(d => d.StaffProfile)
            .WithMany(p => p.BookingAvailabilities)
            .HasForeignKey(d => d.StaffProfileId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("bookingavailability_staffprofileid_foreign");
    }
}