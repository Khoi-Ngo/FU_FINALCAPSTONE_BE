using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class SubjectCommentReactionConfiguration : IEntityTypeConfiguration<SubjectCommentReaction>
    {
        public void Configure(EntityTypeBuilder<SubjectCommentReaction> builder)
        {
            builder.HasKey(e => e.Id).HasName("subjectcommentreaction_id_primary");

            // Composite unique constraint: One reaction per student per comment
            builder.HasIndex(e => new { e.StudentProfileId, e.CommentId })
                .IsUnique()
                .HasDatabaseName("IX_SubjectCommentReaction_Student_Comment_Unique");

            // Index for performance
            builder.HasIndex(e => e.CommentId).HasDatabaseName("IX_SubjectCommentReaction_CommentId");
            builder.HasIndex(e => e.StudentProfileId).HasDatabaseName("IX_SubjectCommentReaction_StudentProfileId");
            builder.HasIndex(e => e.ReactionType).HasDatabaseName("IX_SubjectCommentReaction_ReactionType");

            // Foreign key relationships
            builder.HasOne(d => d.Comment)
                .WithMany(p => p.Reactions)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("subjectcommentreaction_commentid_foreign");

            builder.HasOne(d => d.StudentProfile)
                .WithMany()
                .HasForeignKey(d => d.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("subjectcommentreaction_studentprofileid_foreign");
        }
    }
}
