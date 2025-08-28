using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class SubjectCommentConfiguration : IEntityTypeConfiguration<SubjectComment>
    {
        public void Configure(EntityTypeBuilder<SubjectComment> builder)
        {
            builder.HasKey(e => e.Id).HasName("subjectcomment_id_primary");

            // Composite unique constraint: One comment per student per subject
            builder.HasIndex(e => new { e.StudentProfileId, e.SubjectId })
                .IsUnique()
                .HasDatabaseName("IX_SubjectComment_Student_Subject_Unique");

            // Index for performance
            builder.HasIndex(e => e.SubjectId).HasDatabaseName("IX_SubjectComment_SubjectId");
            builder.HasIndex(e => e.StudentProfileId).HasDatabaseName("IX_SubjectComment_StudentProfileId");
            builder.HasIndex(e => e.JoinedSubjectId).HasDatabaseName("IX_SubjectComment_JoinedSubjectId");
            builder.HasIndex(e => e.IsApproved).HasDatabaseName("IX_SubjectComment_IsApproved");

            // Properties
            builder.Property(e => e.Content)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(e => e.ModerationReason)
                .HasMaxLength(500);

            builder.Property(e => e.ApprovedBy)
                .HasMaxLength(255);

            // Foreign key relationships
            builder.HasOne(d => d.Subject)
                .WithMany(p => p.Comments)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("subjectcomment_subjectid_foreign");

            builder.HasOne(d => d.StudentProfile)
                .WithMany(p => p.SubjectComments)
                .HasForeignKey(d => d.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("subjectcomment_studentprofileid_foreign");

            builder.HasOne(d => d.JoinedSubject)
                .WithOne(p => p.SubjectComment)
                .HasForeignKey<SubjectComment>(d => d.JoinedSubjectId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("subjectcomment_joinedsubjectid_foreign");
        }
    }
}
