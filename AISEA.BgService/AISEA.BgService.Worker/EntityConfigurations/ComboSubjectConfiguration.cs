using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AISEA.BgService.Worker.Entities;

namespace AISEA.BgService.Worker.EntityConfigurations
{
    public class ComboSubjectConfiguration : IEntityTypeConfiguration<ComboSubject>
    {
        public void Configure(EntityTypeBuilder<ComboSubject> builder)
        {
            builder.HasKey(e => new { e.ComboId, e.SubjectId })
                .HasName("combosubject_composite_primary");

            builder.HasIndex(e => e.ComboId).HasDatabaseName("IX_ComboSubject_ComboId");
            builder.HasIndex(e => e.SubjectId).HasDatabaseName("IX_ComboSubject_SubjectId");

            builder.HasOne(d => d.Combo)
                .WithMany(p => p.ComboSubjects)
                .HasForeignKey(d => d.ComboId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("combosubject_comboid_foreign");

            builder.HasOne(d => d.Subject)
                .WithMany(p => p.ComboSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("combosubject_subjectid_foreign");
        }
    }
}
