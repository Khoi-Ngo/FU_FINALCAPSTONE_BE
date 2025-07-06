using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AISEA.BgService.Worker.EntityConfigurations
{
    public class ProgramConfiguration : IEntityTypeConfiguration<Entities.Program>
    {
        public void Configure(EntityTypeBuilder<Entities.Program> builder)
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
