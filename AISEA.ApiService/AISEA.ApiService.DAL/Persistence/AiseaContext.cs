using System;
using System.Collections.Generic;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.EntityConfigurations;
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
  => optionsBuilder.UseSqlServer("Server=jkh8ing8.online,1433;Database=AISEA;User Id=sa;Password=NewYourStrong!Passw0rd;TrustServerCertificate=True;");
    #endregion

    #region DbSets
    public virtual DbSet<AdvisorySession1to1> AdvisorySessions1to1 { get; set; }
    public virtual DbSet<Message> Messages { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<StaffProfile> StaffProfiles { get; set; }
    public virtual DbSet<StudentProfile> StudentProfiles { get; set; }
    public virtual DbSet<User> Users { get; set; }
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
        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new AdvisorySession1to1Configuration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new StaffProfileConfiguration());
        modelBuilder.ApplyConfiguration(new StudentProfileConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProgramConfiguration());
        modelBuilder.ApplyConfiguration(new CurriculumConfiguration());
        modelBuilder.ApplyConfiguration(new SubjectConfiguration());
        modelBuilder.ApplyConfiguration(new SyllabusConfiguration());
        modelBuilder.ApplyConfiguration(new ComboConfiguration());
        modelBuilder.ApplyConfiguration(new CurriculumSubjectConfiguration());
        modelBuilder.ApplyConfiguration(new ComboSubjectConfiguration());
        modelBuilder.ApplyConfiguration(new SubjectPrerequisiteConfiguration());
        modelBuilder.ApplyConfiguration(new SyllabusAssessmentConfiguration());
        modelBuilder.ApplyConfiguration(new SyllabusLearningMaterialConfiguration());
        modelBuilder.ApplyConfiguration(new SyllabusLearningOutcomeConfiguration());
        modelBuilder.ApplyConfiguration(new SyllabusSessionConfiguration());
        modelBuilder.ApplyConfiguration(new SessionOutcomeMappingConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}