using System;
using System.Collections.Generic;
using System.Reflection;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Persistence;

public partial class AiseaContext : DbContext
{
    #region initialization
    private readonly SqlSettings _sqlSettings;
    
    public AiseaContext(DbContextOptions<AiseaContext> options, SqlSettings sqlSettings)
        : base(options)
    {
        _sqlSettings = sqlSettings;
    }

    public AiseaContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_sqlSettings?.ConnectionString ?? 
                "Server=jkh8ing8.online,1433;Database=AISEA;User Id=sa;Password=NewYourStrong!Passw0rd;TrustServerCertificate=True;");
        }
    }
    #endregion

    #region DbSets
    // Existing tables
    public virtual DbSet<AdvisorySession1to1> AdvisorySessions1to1 { get; set; }
    public virtual DbSet<Message> Messages { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<StaffProfile> StaffProfiles { get; set; }
    public virtual DbSet<StudentProfile> StudentProfiles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    
    // New tables - will use Configuration
    public virtual DbSet<Program> Programs { get; set; }
    public virtual DbSet<Curriculum> Curricula { get; set; }
    public virtual DbSet<Subject> Subjects { get; set; }
    public virtual DbSet<Syllabus> Syllabi { get; set; }
    public virtual DbSet<Combo> Combos { get; set; }
    public virtual DbSet<CurriculumSubject> CurriculumSubjects { get; set; }
    public virtual DbSet<ComboSubject> ComboSubjects { get; set; }
    public virtual DbSet<SubjectPrerequisite> SubjectPrerequisites { get; set; }
    public virtual DbSet<SyllabusAssessment> SyllabusAssessments { get; set; }
    public virtual DbSet<SyllabusLearningMaterial> SyllabusLearningMaterials { get; set; }
    public virtual DbSet<SyllabusLearningOutcome> SyllabusLearningOutcomes { get; set; }
    public virtual DbSet<SyllabusSession> SyllabusSessions { get; set; }
    public virtual DbSet<SessionOutcomeMapping> SessionOutcomeMappings { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ===== EXISTING TABLES - Keep original configuration to avoid data loss =====
        
        modelBuilder.Entity<AdvisorySession1to1>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("advisorysession1to1_id_primary");

            entity.HasOne(d => d.Staff)
                .WithMany(p => p.AdvisorySessions1to1)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("advisorysession1to1_staffid_foreign");

            entity.HasOne(d => d.Student)
                .WithMany(p => p.AdvisorySessions1to1)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("advisorysession1to1_studentid_foreign");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("message_id_primary");

            entity.HasOne(d => d.AdvisorySession1to1)
                .WithMany(p => p.Messages)
                .HasForeignKey(d => d.AdvisorySession1to1Id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("message_advisorysession1to1id_foreign");

            entity.HasOne(d => d.Sender)
                .WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("message_senderid_foreign");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_id_primary");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("notification_userid_foreign");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_id_primary");
        });

        modelBuilder.Entity<StaffProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("staffprofile_id_primary");

            entity.HasOne(d => d.User)
                .WithOne(p => p.StaffProfile)
                .HasForeignKey<StaffProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("staffprofile_userid_foreign");
        });

        modelBuilder.Entity<StudentProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("studentprofile_id_primary");

            entity.HasOne(d => d.User)
                .WithOne(p => p.StudentProfile)
                .HasForeignKey<StudentProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("studentprofile_userid_foreign");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_id_primary");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("user_roleid_foreign");
        });

        // ===== NEW TABLES - Use Configuration pattern =====
        
        // Apply configurations only for new entities
        modelBuilder.ApplyConfiguration(new EntityConfigurations.ProgramConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfigurations.CurriculumConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfigurations.SubjectConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfigurations.SyllabusConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfigurations.ComboConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfigurations.CurriculumSubjectConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfigurations.ComboSubjectConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfigurations.SubjectPrerequisiteConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfigurations.SyllabusAssessmentConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfigurations.SyllabusLearningMaterialConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfigurations.SyllabusLearningOutcomeConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfigurations.SyllabusSessionConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfigurations.SessionOutcomeMappingConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}