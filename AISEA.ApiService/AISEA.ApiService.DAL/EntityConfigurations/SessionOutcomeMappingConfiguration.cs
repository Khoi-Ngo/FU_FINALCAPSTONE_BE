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
    public class SessionOutcomeMappingConfiguration : IEntityTypeConfiguration<SessionOutcomeMapping>
    {
        public void Configure(EntityTypeBuilder<SessionOutcomeMapping> builder)
        {
            builder.HasKey(e => new { e.SessionId, e.OutcomeId })
                .HasName("sessionoutcomemapping_composite_primary");

            builder.HasIndex(e => e.SessionId).HasDatabaseName("IX_SessionOutcomeMapping_SessionId");
            builder.HasIndex(e => e.OutcomeId).HasDatabaseName("IX_SessionOutcomeMapping_OutcomeId");

            builder.HasOne(d => d.Session)
                .WithMany(p => p.SessionOutcomeMappings)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sessionoutcomemapping_sessionid_foreign");

            builder.HasOne(d => d.Outcome)
                .WithMany(p => p.SessionOutcomeMappings)
                .HasForeignKey(d => d.OutcomeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sessionoutcomemapping_outcomeid_foreign");
        }
    }
}
