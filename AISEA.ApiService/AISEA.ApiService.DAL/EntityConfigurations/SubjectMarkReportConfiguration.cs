using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class SubjectMarkReportConfiguration : IEntityTypeConfiguration<SubjectMarkReport>
    {
        public void Configure(EntityTypeBuilder<SubjectMarkReport> builder)
        {
            builder.HasKey(e => e.Id).HasName("subjectmarkreport_id_primary");

            builder.HasOne(s => s.JoinedSubject)
                   .WithMany(j => j.SubjectMarkReports)
                   .HasForeignKey(s => s.JoinedSubjectId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("subjectmarkreport_joinedsubjectid_foreign");


            builder.ToTable(t =>
           {
               t.HasTrigger("trg_CheckWeightSum_InsertUpdate");
           });
        }
    }
}
