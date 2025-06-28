using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class CurriculumSubjectConfiguration : IEntityTypeConfiguration<CurriculumSubject>
    {
        public void Configure(EntityTypeBuilder<CurriculumSubject> builder)
        {
            builder.HasKey(e => new { e.CurriculumId, e.SubjectId })
                .HasName("curriculumsubject_composite_primary");

            builder.HasIndex(e => e.CurriculumId).HasDatabaseName("IX_CurriculumSubject_CurriculumId");
            builder.HasIndex(e => e.SubjectId).HasDatabaseName("IX_CurriculumSubject_SubjectId");

            builder.HasOne(d => d.Curriculum)
                .WithMany(p => p.CurriculumSubjects)
                .HasForeignKey(d => d.CurriculumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("curriculumsubject_curriculumid_foreign");

            builder.HasOne(d => d.Subject)
                .WithMany(p => p.CurriculumSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("curriculumsubject_subjectid_foreign");
        }
    }
}
