using System;
using System.Collections.Generic;
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
  => optionsBuilder.UseSqlServer(_sqlSettings.ConnectionString);
    #endregion

    #region DbSets
    public virtual DbSet<ChatSession> ChatSessions { get; set; }
    public virtual DbSet<Message> Messages { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<StaffProfile> StaffProfiles { get; set; }
    public virtual DbSet<StudentProfile> StudentProfiles { get; set; }
    public virtual DbSet<User> Users { get; set; }

    // New DbSets
    public virtual DbSet<Program> Programs { get; set; }
    public virtual DbSet<Curriculum> Curricula { get; set; }
    public virtual DbSet<Subject> Subjects { get; set; }
    public virtual DbSet<Syllabus> Syllabi { get; set; }
    public virtual DbSet<Combo> Combos { get; set; }
    public virtual DbSet<CurriculumSubject> CurriculumSubjects { get; set; }
    public virtual DbSet<ComboSubject> ComboSubjects { get; set; }
    public virtual DbSet<AdvisorProfile> AdvisorProfiles { get; set; }
    public virtual DbSet<AcademicStaffProfile> AcademicStaffProfiles { get; set; }
    public virtual DbSet<StudentEnrollment> StudentEnrollments { get; set; }
    public virtual DbSet<SubjectPrerequisite> SubjectPrerequisites { get; set; }
    public virtual DbSet<Conversation> Conversations { get; set; }
    public virtual DbSet<ConversationParticipant> ConversationParticipants { get; set; }
    public virtual DbSet<AdvisorAvailabilitySlot> AdvisorAvailabilitySlots { get; set; }
    public virtual DbSet<Meeting> Meetings { get; set; }
    public virtual DbSet<SyllabusAssessment> SyllabusAssessments { get; set; }
    public virtual DbSet<SyllabusLearningMaterial> SyllabusLearningMaterials { get; set; }
    public virtual DbSet<SyllabusLearningOutcome> SyllabusLearningOutcomes { get; set; }
    public virtual DbSet<SyllabusSession> SyllabusSessions { get; set; }
    public virtual DbSet<SessionOutcomeMapping> SessionOutcomeMappings { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Existing configurations
        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chatsession_id_primary");
            entity.HasOne(d => d.Staff).WithMany(p => p.ChatSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chatsession_staffid_foreign");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("message_id_primary");
            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("message_conversationid_foreign");
            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("message_senderid_foreign");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_id_primary");
            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notification_userid_foreign");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_id_primary");
        });

        modelBuilder.Entity<StaffProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("staffprofile_id_primary");
            entity.HasOne(d => d.User).WithMany(p => p.StaffProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("staffprofile_userid_foreign");
        });

        modelBuilder.Entity<StudentProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("studentprofile_userid_primary");
            entity.HasOne(d => d.User).WithMany(p => p.StudentProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("studentprofile_userid_foreign");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_id_primary");
            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_roleid_foreign");
        });

        // New entity configurations
        modelBuilder.Entity<Program>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("program_id_primary");
        });

        modelBuilder.Entity<Curriculum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("curriculum_id_primary");
            entity.HasOne(d => d.Program).WithMany(p => p.Curricula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("curriculum_programid_foreign");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("subject_id_primary");
        });

        modelBuilder.Entity<Syllabus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("syllabus_id_primary");
            entity.HasOne(d => d.Subject).WithMany(p => p.Syllabi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("syllabus_subjectid_foreign");
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("combo_id_primary");
        });

        modelBuilder.Entity<CurriculumSubject>(entity =>
        {
            entity.HasKey(e => new { e.CurriculumId, e.SubjectId }).HasName("curriculumsubject_composite_primary");
            entity.HasOne(d => d.Curriculum).WithMany(p => p.CurriculumSubjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("curriculumsubject_curriculumid_foreign");
            entity.HasOne(d => d.Subject).WithMany(p => p.CurriculumSubjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("curriculumsubject_subjectid_foreign");
        });

        modelBuilder.Entity<ComboSubject>(entity =>
        {
            entity.HasKey(e => new { e.ComboId, e.SubjectId }).HasName("combosubject_composite_primary");
            entity.HasOne(d => d.Combo).WithMany(p => p.ComboSubjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("combosubject_comboid_foreign");
            entity.HasOne(d => d.Subject).WithMany(p => p.ComboSubjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("combosubject_subjectid_foreign");
        });

        modelBuilder.Entity<AdvisorProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("advisorprofile_userid_primary");
            entity.HasOne(d => d.User).WithMany(p => p.AdvisorProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("advisorprofile_userid_foreign");
        });

        modelBuilder.Entity<AcademicStaffProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("academicstaffprofile_userid_primary");
            entity.HasOne(d => d.User).WithMany(p => p.AcademicStaffProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("academicstaffprofile_userid_foreign");
        });

        modelBuilder.Entity<StudentEnrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("studentenrollment_id_primary");
            entity.HasOne(d => d.User).WithMany(p => p.StudentEnrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("studentenrollment_userid_foreign");
            entity.HasOne(d => d.Subject).WithMany(p => p.StudentEnrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("studentenrollment_subjectid_foreign");
        });

        modelBuilder.Entity<SubjectPrerequisite>(entity =>
        {
            entity.HasKey(e => new { e.SubjectId, e.PrerequisiteSubjectId }).HasName("subjectprerequisite_composite_primary");
            entity.HasOne(d => d.Subject).WithMany(p => p.Prerequisites)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("subjectprerequisite_subjectid_foreign");
            entity.HasOne(d => d.PrerequisiteSubject).WithMany(p => p.DependentSubjects)
                .HasForeignKey(d => d.PrerequisiteSubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("subjectprerequisite_prerequisitesubjectid_foreign");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("conversation_id_primary");
        });

        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ConversationId }).HasName("conversationparticipant_composite_primary");
            entity.HasOne(d => d.User).WithMany(p => p.ConversationParticipants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("conversationparticipant_userid_foreign");
            entity.HasOne(d => d.Conversation).WithMany(p => p.ConversationParticipants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("conversationparticipant_conversationid_foreign");
        });

        modelBuilder.Entity<AdvisorAvailabilitySlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("advisoravailabilityslot_id_primary");
            entity.HasOne(d => d.Advisor).WithMany(p => p.AdvisorAvailabilitySlots)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("advisoravailabilityslot_advisorid_foreign");
        });

        modelBuilder.Entity<Meeting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("meeting_id_primary");
            entity.HasOne(d => d.Student).WithMany(p => p.StudentMeetings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("meeting_studentid_foreign");
            entity.HasOne(d => d.Slot).WithMany(p => p.Meetings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("meeting_slotid_foreign");
        });

        modelBuilder.Entity<SyllabusAssessment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("syllabusassessment_id_primary");
            entity.HasOne(d => d.Syllabus).WithMany(p => p.SyllabusAssessments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("syllabusassessment_syllabusid_foreign");
        });

        modelBuilder.Entity<SyllabusLearningMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("syllabuslearningmaterial_id_primary");
            entity.HasOne(d => d.Syllabus).WithMany(p => p.SyllabusLearningMaterials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("syllabuslearningmaterial_syllabusid_foreign");
        });

        modelBuilder.Entity<SyllabusLearningOutcome>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("syllabuslearningoutcome_id_primary");
            entity.HasOne(d => d.Syllabus).WithMany(p => p.SyllabusLearningOutcomes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("syllabuslearningoutcome_syllabusid_foreign");
        });

        modelBuilder.Entity<SyllabusSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("syllabussession_id_primary");
            entity.HasOne(d => d.Syllabus).WithMany(p => p.SyllabusSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("syllabussession_syllabusid_foreign");
        });

        modelBuilder.Entity<SessionOutcomeMapping>(entity =>
        {
            entity.HasKey(e => new { e.SessionId, e.OutcomeId }).HasName("sessionoutcomemapping_composite_primary");
            entity.HasOne(d => d.Session).WithMany(p => p.SessionOutcomeMappings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sessionoutcomemapping_sessionid_foreign");
            entity.HasOne(d => d.Outcome).WithMany(p => p.SessionOutcomeMappings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sessionoutcomemapping_outcomeid_foreign");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}