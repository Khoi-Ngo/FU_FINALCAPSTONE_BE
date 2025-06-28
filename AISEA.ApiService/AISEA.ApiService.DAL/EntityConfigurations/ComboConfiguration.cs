using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
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
                .HasColumnType("text");
        }
    }
}