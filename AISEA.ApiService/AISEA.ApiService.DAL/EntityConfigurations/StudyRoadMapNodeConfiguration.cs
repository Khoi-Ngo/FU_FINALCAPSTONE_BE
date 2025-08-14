using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class StudyRoadMapNodeConfiguration : IEntityTypeConfiguration<StudyRoadMapNode>
    {
        public void Configure(EntityTypeBuilder<StudyRoadMapNode> builder)
        {
            builder.HasKey(e => e.Id)
                   .HasName("studyroadmapnode_id_primary");

            builder.HasOne(n => n.StudyRoadMap)
                   .WithMany(rm => rm.Nodes)
                   .HasForeignKey(n => n.StudyRoadMapId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("studyroadmapnode_studyroadmapid_foreign");
        }
    }
}