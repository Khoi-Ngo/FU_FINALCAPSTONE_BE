using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class OptionalSubjectCheckpointConfiguration : IEntityTypeConfiguration<OptionalSubjectCheckPoint>
{
    public void Configure(EntityTypeBuilder<OptionalSubjectCheckPoint> builder)
    {
        builder.HasKey(e => e.Id)
            .HasName("optionalsubjectcheckpoint_id_primary");

        builder.HasOne(d => d.OptionalPersonalSubject)
            .WithMany(p => p.Checkpoints)
            .HasForeignKey(d => d.OptionalPersonalSubjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("optionalsubjectcheckpoint_optionalpersonalsubjectid_foreign");
    }
}
