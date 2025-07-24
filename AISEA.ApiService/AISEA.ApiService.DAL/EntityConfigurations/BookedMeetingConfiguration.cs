using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class BookedMeetingConfiguration : IEntityTypeConfiguration<BookedMeeting>
{
    public void Configure(EntityTypeBuilder<BookedMeeting> builder)
    {
        builder.HasKey(e => e.Id).HasName("bookedmeeting_id_primary");

        builder.HasOne(d => d.StaffProfile)
            .WithMany(p => p.BookedMeetings)
            .HasForeignKey(d => d.StaffProfileId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("bookedmeeting_staffprofileid_foreign");

        builder.HasOne(d => d.StudentProfile)
            .WithMany(p => p.BookedMeetings)
            .HasForeignKey(d => d.StudentProfileId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("bookedmeeting_studentprofileid_foreign");
            
        builder.ToTable(t =>
                {
                    t.HasTrigger("TR_BookedMeeting_CheckExternalTables");
                    t.HasTrigger("TR_BookedMeeting_CheckInternalData");
                });
    }
}