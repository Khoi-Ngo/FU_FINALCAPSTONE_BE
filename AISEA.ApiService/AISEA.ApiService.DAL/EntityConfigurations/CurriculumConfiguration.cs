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
    public class CurriculumConfiguration : IEntityTypeConfiguration<Curriculum>
    {
        public void Configure(EntityTypeBuilder<Curriculum> builder)
        {
            builder.HasKey(e => e.Id).HasName("curriculum_id_primary");

            builder.HasIndex(e => e.CurriculumCode).IsUnique().HasDatabaseName("curriculum_code_unique");
            builder.HasIndex(e => e.ProgramId).HasDatabaseName("IX_Curriculum_ProgramId");

            builder.Property(e => e.CurriculumCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.CurriculumName)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasOne(d => d.Program)
                .WithMany(p => p.Curricula)
                .HasForeignKey(d => d.ProgramId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("curriculum_program_foreign");
        }
    }
}
