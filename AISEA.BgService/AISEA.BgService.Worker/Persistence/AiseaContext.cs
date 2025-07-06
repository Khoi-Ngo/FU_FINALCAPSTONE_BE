using AISEA.BgService.Worker.Entities;
using AISEA.BgService.Worker.EntityConfigurations;
using AISEA.BgService.Worker.PropConfig;
using AISEA.BgService.Worker.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AISEA.BgService.Worker.Persistence;

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
  => optionsBuilder.UseSqlServer(_sqlSettings.ConnectionString);
    #endregion

    #region DbSets
    public virtual DbSet<AdvisorySession1to1> AdvisorySessions1to1 { get; set; }
    public virtual DbSet<Message> Messages { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<StaffProfile> StaffProfiles { get; set; }
    public virtual DbSet<StudentProfile> StudentProfiles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Entities.Program> Programs { get; set; }
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
    public virtual DbSet<AuditLog> AuditLogs { get; set; }

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
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}