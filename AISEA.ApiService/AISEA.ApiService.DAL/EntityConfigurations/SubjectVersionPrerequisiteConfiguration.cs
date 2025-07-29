using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class SubjectVersionPrerequisiteConfiguration : IEntityTypeConfiguration<SubjectVersionPrerequisite>
{
    public void Configure(EntityTypeBuilder<SubjectVersionPrerequisite> builder)
    {
        builder.ToTable("SubjectVersionPrerequisite");

        // Composite primary key
        builder.HasKey(svp => new { svp.SubjectVersionId, svp.PrerequisiteSubjectVersionId })
               .HasName("subjectversionprerequisite_composite_primary");

        // Foreign key relationships
        builder.HasOne(svp => svp.SubjectVersion)
               .WithMany(sv => sv.Prerequisites)
               .HasForeignKey(svp => svp.SubjectVersionId)
               .HasConstraintName("subjectversionprerequisite_subjectversionid_foreign")
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(svp => svp.PrerequisiteSubjectVersion)
               .WithMany(sv => sv.DependentSubjectVersions)
               .HasForeignKey(svp => svp.PrerequisiteSubjectVersionId)
               .HasConstraintName("subjectversionprerequisite_prerequisitesubjectversionid_foreign")
               .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(svp => svp.SubjectVersionId)
               .HasDatabaseName("IX_SubjectVersionPrerequisite_SubjectVersionId");

        builder.HasIndex(svp => svp.PrerequisiteSubjectVersionId)
               .HasDatabaseName("IX_SubjectVersionPrerequisite_PrerequisiteSubjectVersionId");

        // Column configurations
        builder.Property(svp => svp.SubjectVersionId).HasColumnName("subject_version_id");
        builder.Property(svp => svp.PrerequisiteSubjectVersionId).HasColumnName("prerequisite_subject_version_id");
        builder.Property(svp => svp.CreatedAt).HasColumnType("datetime2");
        builder.Property(svp => svp.UpdatedAt).HasColumnType("datetime2");
        builder.Property(svp => svp.DeletedAt).HasColumnType("datetime2");
        builder.Property(svp => svp.IsDeleted).HasColumnType("bit");
    }
}
