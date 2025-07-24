using AISEA.ApiService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEA.ApiService.DAL.EntityConfigurations;

public class SubjectClassConfiguration : IEntityTypeConfiguration<SubjectClass>
{
    public void Configure(EntityTypeBuilder<SubjectClass> builder)
    {
        builder.HasKey(e => e.Id).HasName("subjectclass_id_primary");
        builder.HasIndex(e => new { e.SubjectVersionId, e.SemesterNumber, e.ClassCode }).IsUnique().HasDatabaseName("IX_SubjectClass_UniqueClass");
        builder.HasOne(e => e.SubjectVersion).WithMany(e => e.SubjectClasses).HasForeignKey(e => e.SubjectVersionId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(e => e.Id).HasName("schedule_id_primary");
        builder.HasIndex(e => new { e.SubjectClassId, e.ClassDate }).IsUnique().HasDatabaseName("IX_Schedule_UniqueClassDate");
        builder.HasOne(e => e.SubjectClass).WithMany(e => e.Schedules).HasForeignKey(e => e.SubjectClassId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class AttendanceChecklistConfiguration : IEntityTypeConfiguration<AttendanceChecklist>
{
    public void Configure(EntityTypeBuilder<AttendanceChecklist> builder)
    {
        builder.HasKey(e => e.Id).HasName("attendancechecklist_id_primary");
        builder.HasIndex(e => e.Username).HasDatabaseName("IX_AttendanceChecklist_Username");
        builder.HasOne(e => e.Schedule).WithMany(e => e.AttendanceChecklists).HasForeignKey(e => e.ScheduleId).OnDelete(DeleteBehavior.Restrict);
    }
}