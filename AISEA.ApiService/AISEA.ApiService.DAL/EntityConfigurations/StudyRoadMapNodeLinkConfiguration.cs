using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class StudyRoadMapNodeLinkConfiguration : IEntityTypeConfiguration<StudyRoadMapNodeLink>
    {
        public void Configure(EntityTypeBuilder<StudyRoadMapNodeLink> builder)
        {
            builder.HasKey(e => e.Id)
                   .HasName("studyroadmapnodelink_id_primary");

            // Each link has one "FromNode"
            builder.HasOne(link => link.FromNode)
                   .WithMany(node => node.Dependents)
                   .HasForeignKey(link => link.FromNodeId)
                   .OnDelete(DeleteBehavior.Restrict) // prevent multiple cascade paths
                   .HasConstraintName("studyroadmapnodelink_fromnodeid_foreign");

            // Each link has one "ToNode"
            builder.HasOne(link => link.ToNode)
                   .WithMany(node => node.Prerequisites)
                   .HasForeignKey(link => link.ToNodeId)
                   .OnDelete(DeleteBehavior.Restrict) // prevent multiple cascade paths
                   .HasConstraintName("studyroadmapnodelink_tonodeid_foreign");
        }
    }


}