using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class OptionalPersonalSubjectConfiguration : IEntityTypeConfiguration<OptionalPersonalSubject>
{
    public void Configure(EntityTypeBuilder<OptionalPersonalSubject> builder)
    {
        builder.HasKey(e => e.Id).HasName("optionalpersonalsubject_id_primary");

        builder.HasOne(d => d.StudentProfile)
            .WithMany(p => p.OptionalPersonalSubjects)
            .HasForeignKey(d => d.StudentProfileId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("optionalpersonalsubject_studentprofileid_foreign");

        builder.HasOne(d => d.Semester)
            .WithMany(p => p.OptionalPersonalSubjects)
            .HasForeignKey(d => d.SemesterId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("optionalpersonalsubject_semesterid_foreign");

        builder.HasIndex(e => new { e.StudentProfileId, e.SemesterId, e.SubjectCode })
            .IsUnique()
            .HasDatabaseName("UX_OptionalPersonalSubject_Student_Semester_Subject");
    }
}
