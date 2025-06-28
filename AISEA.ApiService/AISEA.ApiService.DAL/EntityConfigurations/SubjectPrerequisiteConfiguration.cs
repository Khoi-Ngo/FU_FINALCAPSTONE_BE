using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class SubjectPrerequisiteConfiguration : IEntityTypeConfiguration<SubjectPrerequisite>
    {
        public void Configure(EntityTypeBuilder<SubjectPrerequisite> builder)
        {
            builder.HasKey(e => new { e.SubjectId, e.PrerequisiteSubjectId })
                .HasName("subjectprerequisite_composite_primary");

            builder.HasIndex(e => e.SubjectId).HasDatabaseName("IX_SubjectPrerequisite_SubjectId");
            builder.HasIndex(e => e.PrerequisiteSubjectId).HasDatabaseName("IX_SubjectPrerequisite_PrerequisiteSubjectId");

            // Một môn học (Subject) có thể có nhiều môn học tiên quyết (Prerequisites)
            builder.HasOne(d => d.Subject)
                .WithMany(p => p.DependentSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("subjectprerequisite_subjectid_foreign");

            // Một môn học (Subject) có thể là điều kiện tiên quyết của nhiều môn học khác (DependentSubjects)
            builder.HasOne(d => d.PrerequisiteSubject)
                .WithMany(p => p.Prerequisites)
                .HasForeignKey(d => d.PrerequisiteSubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("subjectprerequisite_prerequisitesubjectid_foreign");
        }
    }
}