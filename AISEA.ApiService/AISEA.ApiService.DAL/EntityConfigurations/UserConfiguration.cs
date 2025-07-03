using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id).HasName("user_id_primary");

            builder.HasIndex(e => e.Email).IsUnique().HasDatabaseName("user_email_unique");
            builder.HasIndex(e => e.Username).IsUnique().HasDatabaseName("user_username_unique");
            builder.HasIndex(e => e.RoleId).HasDatabaseName("IX_User_RoleId");

            builder.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.AvatarUrl)
                .HasMaxLength(255);

            builder.HasOne(d => d.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("user_roleid_foreign");
        }
    }
}