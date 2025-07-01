using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class SyllabusLearningMaterialConfiguration : IEntityTypeConfiguration<SyllabusLearningMaterial>
    {
        public void Configure(EntityTypeBuilder<SyllabusLearningMaterial> builder)
        {
            builder.HasKey(e => e.Id).HasName("syllabuslearningmaterial_id_primary");

            builder.HasIndex(e => e.SyllabusId).HasDatabaseName("IX_SyllabusLearningMaterial_SyllabusId");

            builder.Property(e => e.MaterialName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.AuthorName)
                .HasMaxLength(255);

            builder.Property(e => e.Description)
                .HasColumnType("text");

            builder.Property(e => e.FilepathOrUrl)
                .HasMaxLength(500);

            builder.HasOne(d => d.Syllabus)
                .WithMany(p => p.SyllabusLearningMaterials)
                .HasForeignKey(d => d.SyllabusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("syllabuslearningmaterial_syllabusid_foreign");
        }
    }
}
