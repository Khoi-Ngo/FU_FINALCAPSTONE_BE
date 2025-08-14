using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class StudyRoadMapConfiguration : IEntityTypeConfiguration<StudyRoadMap>
    {
        public void Configure(EntityTypeBuilder<StudyRoadMap> builder)
        {
            builder.HasKey(e => e.Id).HasName("studyroadmap_id_primary");

            builder.HasOne(e => e.StudentProfile)
                   .WithOne(sp => sp.StudyRoadMap)
                   .HasForeignKey<StudyRoadMap>(e => e.StudentProfileId)
                   .OnDelete(DeleteBehavior.Cascade) // or Restrict, depending on business rules
                   .HasConstraintName("studyroadmap_studentprofileid_foreign");
        }
    }
}
