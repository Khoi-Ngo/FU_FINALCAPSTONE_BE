using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class SyllabusConfiguration : IEntityTypeConfiguration<Syllabus>
    {
        public void Configure(EntityTypeBuilder<Syllabus> builder)
        {
            builder.HasKey(e => e.Id).HasName("syllabus_id_primary");

            builder.HasIndex(e => e.SubjectVersionId).HasDatabaseName("IX_Syllabus_SubjectVersionId");

            builder.Property(e => e.Content)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.HasOne(d => d.SubjectVersion)
                .WithMany(p => p.Syllabi)
                .HasForeignKey(d => d.SubjectVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("syllabus_subjectversionid_foreign");
        }
    }
}
