using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(e => e.Id).HasName("message_id_primary");

        builder.HasIndex(e => e.AdvisorySession1to1Id).HasDatabaseName("IX_Message_AdvisorySession1to1Id");
        builder.HasIndex(e => e.SenderId).HasDatabaseName("IX_Message_SenderId");
        builder.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_Message_CreatedAt");

        builder.Property(e => e.Content)
            .IsRequired()
            .HasColumnType("nvarchar(max)"); // Updated from "text" to "nvarchar(max)"

        builder.HasOne(d => d.AdvisorySession1to1)
            .WithMany(p => p.Messages)
            .HasForeignKey(d => d.AdvisorySession1to1Id)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("message_advisorysession1to1id_foreign");

        builder.HasOne(d => d.Sender)
            .WithMany(p => p.Messages)
            .HasForeignKey(d => d.SenderId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("message_senderid_foreign");
    }
}