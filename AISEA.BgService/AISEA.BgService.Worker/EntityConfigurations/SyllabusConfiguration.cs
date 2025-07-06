using AISEA.BgService.Worker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.BgService.Worker.EntityConfigurations
{
    public class SyllabusConfiguration : IEntityTypeConfiguration<Syllabus>
    {
        public void Configure(EntityTypeBuilder<Syllabus> builder)
        {
            builder.HasKey(e => e.Id).HasName("syllabus_id_primary");

            builder.HasIndex(e => e.SubjectId).HasDatabaseName("IX_Syllabus_SubjectId");

            builder.Property(e => e.Content)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.HasOne(d => d.Subject)
                .WithMany(p => p.Syllabi)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("syllabus_subjectid_foreign");
        }
    }
}
