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
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.HasKey(e => e.Id).HasName("subject_id_primary");

            builder.HasIndex(e => e.SubjectCode).IsUnique().HasDatabaseName("subject_code_unique");

            builder.Property(e => e.SubjectCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.SubjectName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Description)
                .HasColumnType("nvarchar(max)");
        }
    }
}
