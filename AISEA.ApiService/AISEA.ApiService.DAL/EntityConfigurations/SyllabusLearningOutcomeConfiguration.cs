using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class SyllabusLearningOutcomeConfiguration : IEntityTypeConfiguration<SyllabusLearningOutcome>
    {
        public void Configure(EntityTypeBuilder<SyllabusLearningOutcome> builder)
        {
            builder.HasKey(e => e.Id).HasName("syllabuslearningoutcome_id_primary");

            builder.HasIndex(e => e.SyllabusId).HasDatabaseName("IX_SyllabusLearningOutcome_SyllabusId");

            builder.Property(e => e.OutcomeCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasColumnType("nvarchar(max)"); // Updated from "text" to "nvarchar(max)"

            builder.HasOne(d => d.Syllabus)
                .WithMany(p => p.SyllabusLearningOutcomes)
                .HasForeignKey(d => d.SyllabusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("syllabuslearningoutcome_syllabusid_foreign");
        }
    }
}