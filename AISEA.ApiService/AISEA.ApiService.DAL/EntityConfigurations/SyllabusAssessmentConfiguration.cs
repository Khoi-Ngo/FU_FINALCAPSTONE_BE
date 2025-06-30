using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class SyllabusAssessmentConfiguration : IEntityTypeConfiguration<SyllabusAssessment>
    {
        public void Configure(EntityTypeBuilder<SyllabusAssessment> builder)
        {
            builder.HasKey(e => e.Id).HasName("syllabusassessment_id_primary");

            builder.HasIndex(e => e.SyllabusId).HasDatabaseName("IX_SyllabusAssessment_SyllabusId");

            builder.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Weight)
                .HasColumnType("decimal(5, 2)");

            builder.Property(e => e.CompletionCriteria)
                .HasColumnType("nvarchar(max)"); // Updated from "text" to "nvarchar(max)"

            builder.Property(e => e.QuestionType)
                .HasMaxLength(255);

            builder.HasOne(d => d.Syllabus)
                .WithMany(p => p.SyllabusAssessments)
                .HasForeignKey(d => d.SyllabusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("syllabusassessment_syllabusid_foreign");
        }
    }
}