using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class SubjectVersionConfiguration : IEntityTypeConfiguration<SubjectVersion>
    {
        public void Configure(EntityTypeBuilder<SubjectVersion> builder)
        {
            builder.ToTable("SubjectVersion");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.VersionCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.VersionName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Description)
                .HasColumnType("text");

            builder.Property(e => e.EffectiveFrom)
                .IsRequired();

            builder.HasIndex(e => new { e.SubjectId, e.VersionCode })
                .IsUnique();

            builder.HasOne(e => e.Subject)
                .WithMany(e => e.SubjectVersions)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
