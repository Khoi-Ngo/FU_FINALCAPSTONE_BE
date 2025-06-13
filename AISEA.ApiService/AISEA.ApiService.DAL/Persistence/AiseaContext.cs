using System;
using System.Collections.Generic;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Persistence;
//? Whether need empty constructor or not?


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
  => optionsBuilder.UseSqlServer("Server=jkh8ing8.online,1433;Database=AISEA;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;");
    #endregion

    #region DbSets
    public virtual DbSet<ChatSession> ChatSessions { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StaffProfile> StaffProfiles { get; set; }

    public virtual DbSet<StudentProfile> StudentProfiles { get; set; }

    public virtual DbSet<User> Users { get; set; }
    #endregion


    //only including the key and constraints relations
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

            entity.HasOne(d => d.ChatSession).WithMany(p => p.Messages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("message_chatsessionid_foreign");

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
            entity.HasKey(e => e.Id).HasName("studentprofile_id_primary");

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
