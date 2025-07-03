using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
      public class StaffProfileConfiguration : IEntityTypeConfiguration<StaffProfile>
      {
          public void Configure(EntityTypeBuilder<StaffProfile> builder)
          {
              builder.HasKey(e => e.Id).HasName("staffprofile_id_primary");
  
              builder.HasIndex(e => e.UserId).IsUnique().HasDatabaseName("IX_StaffProfile_UserId");
  
              builder.Property(e => e.Campus)
                  .IsRequired()
                  .HasMaxLength(255);
  
              builder.Property(e => e.Department)
                  .IsRequired()
                  .HasMaxLength(255);
  
              builder.Property(e => e.Position)
                  .IsRequired()
                  .HasMaxLength(255);
  
              builder.HasOne(d => d.User)
                  .WithOne(p => p.StaffProfile)
                  .HasForeignKey<StaffProfile>(d => d.UserId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("staffprofile_userid_foreign");
          }
      }
}