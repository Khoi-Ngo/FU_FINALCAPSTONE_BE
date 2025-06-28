using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(e => e.Id).HasName("notification_id_primary");

            builder.HasIndex(e => e.UserId).HasDatabaseName("IX_Notification_UserId");
            builder.HasIndex(e => e.IsRead).HasDatabaseName("IX_Notification_IsRead");
            builder.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_Notification_CreatedAt");

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Content)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(e => e.Link)
                .HasColumnType("text");

            builder.HasOne(d => d.User)
                .WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("notification_userid_foreign");
        }
    }
}