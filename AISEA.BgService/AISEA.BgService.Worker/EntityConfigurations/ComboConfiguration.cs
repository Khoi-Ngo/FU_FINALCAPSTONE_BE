using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AISEA.BgService.Worker.Entities;

namespace AISEA.BgService.Worker.EntityConfigurations
{
    public class ComboConfiguration : IEntityTypeConfiguration<Combo>
    {
        public void Configure(EntityTypeBuilder<Combo> builder)
        {
            builder.HasKey(e => e.Id).HasName("combo_id_primary");

            builder.Property(e => e.ComboName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.ComboDescription)
                .HasColumnType("nvarchar(max)");
        }
    }
}
