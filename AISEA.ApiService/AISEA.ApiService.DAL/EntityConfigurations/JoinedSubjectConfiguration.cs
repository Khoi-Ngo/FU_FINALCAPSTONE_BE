using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class JoinedSubjectConfiguration : IEntityTypeConfiguration<JoinedSubject>
{
    public void Configure(EntityTypeBuilder<JoinedSubject> builder)
    {
        builder.HasKey(e => e.Id).HasName("joinedsubject_id_primary");

        builder.HasOne(d => d.StudentProfile)
            .WithMany(p => p.JoinedCourses)
            .HasForeignKey(d => d.StudentProfileId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("joinedsubject_studentprofileid_foreign");

        // Ensure Name is unique per StudentProfile + Semester
        // builder.HasIndex(e => new { e.StudentProfileId, e.Name, e.SemesterId })
        //     .IsUnique()
        //     .HasDatabaseName("IX_JoinedSubject_StudentProfileId_Name_SemesterId_Unique");

        // Relationship with Semester
        builder.HasOne(d => d.Semester)
            .WithMany(p => p.JoinedCourses)
            .HasForeignKey(d => d.SemesterId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("joinedsubject_semesterid_foreign");
            
        builder.ToTable(t =>
        {
            t.HasTrigger("trg_JoinedSubject_SubjectCode_Limit");
        });

        //In the same sem, each student will have no duplicate pair SubjectCode - BlockType
        builder.HasIndex(e => new { e.StudentProfileId, e.SemesterId, e.SemesterStudyBlockType, e.SubjectCode })
          .IsUnique()
          .HasDatabaseName("UX_JoinedSubject_Student_Semester_BlockType_Subject");
    }
}