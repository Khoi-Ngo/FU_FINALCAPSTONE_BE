using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class JoinedSubjectCheckPointConfiguration : IEntityTypeConfiguration<JoinedSubjectCheckPoint>
    {
        public void Configure(EntityTypeBuilder<JoinedSubjectCheckPoint> builder)
        {
            builder.HasKey(e => e.Id)
                   .HasName("joinedsubjectcheckpoint_id_primary");

            builder.HasOne(cp => cp.JoinedSubject)
                   .WithMany(js => js.JoinedSubjectCheckPoints)
                   .HasForeignKey(cp => cp.JoinedSubjectId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("joinedsubjectcheckpoint_joinedsubjectid_foreign");

            builder.ToTable("JoinedSubjectCheckPoint");
        }
    }
}
