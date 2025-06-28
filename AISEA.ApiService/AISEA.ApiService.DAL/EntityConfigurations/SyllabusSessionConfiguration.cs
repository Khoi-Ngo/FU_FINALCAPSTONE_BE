using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class SyllabusSessionConfiguration : IEntityTypeConfiguration<SyllabusSession>
    {
        public void Configure(EntityTypeBuilder<SyllabusSession> builder)
        {
            builder.HasKey(e => e.Id).HasName("syllabussession_id_primary");

            builder.HasIndex(e => e.SyllabusId).HasDatabaseName("IX_SyllabusSession_SyllabusId");

            builder.Property(e => e.Topic)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Mission)
                .HasColumnType("text");

            builder.HasOne(d => d.Syllabus)
                .WithMany(p => p.SyllabusSessions)
                .HasForeignKey(d => d.SyllabusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("syllabussession_syllabusid_foreign");
        }
    }
}