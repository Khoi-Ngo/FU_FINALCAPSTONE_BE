using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class ProgramConfiguration : IEntityTypeConfiguration<Program>
    {
        public void Configure(EntityTypeBuilder<Program> builder)
        {
            builder.HasKey(e => e.Id).HasName("program_id_primary");

            builder.HasIndex(e => e.ProgramCode).IsUnique().HasDatabaseName("program_code_unique");

            builder.Property(e => e.ProgramName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.ProgramCode)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}