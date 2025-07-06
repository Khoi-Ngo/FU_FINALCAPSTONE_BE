using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class removefieldsBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SyllabusSession");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SyllabusSession");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "SyllabusSession");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SyllabusSession");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SyllabusLearningOutcome");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SyllabusLearningOutcome");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "SyllabusLearningOutcome");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SyllabusLearningOutcome");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SyllabusLearningMaterial");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SyllabusLearningMaterial");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "SyllabusLearningMaterial");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SyllabusLearningMaterial");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SyllabusAssessment");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SyllabusAssessment");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "SyllabusAssessment");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SyllabusAssessment");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SubjectPrerequisite");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SubjectPrerequisite");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "SubjectPrerequisite");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SubjectPrerequisite");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StudentProfile");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StudentProfile");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "StudentProfile");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "StudentProfile");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StaffProfile");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StaffProfile");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "StaffProfile");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "StaffProfile");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SessionOutcomeMapping");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SessionOutcomeMapping");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "SessionOutcomeMapping");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SessionOutcomeMapping");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Program");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Program");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Program");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Program");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CurriculumSubject");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CurriculumSubject");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "CurriculumSubject");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CurriculumSubject");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Curriculum");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Curriculum");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Curriculum");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Curriculum");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ComboSubject");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ComboSubject");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "ComboSubject");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ComboSubject");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Combo");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Combo");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Combo");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Combo");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AdvisorySession1to1");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AdvisorySession1to1");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "AdvisorySession1to1");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AdvisorySession1to1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AuditLog",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AuditLog",
                type: "nvarchar(max)",
                maxLength: 20000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AuditLog");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SyllabusSession",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "SyllabusSession",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SyllabusSession",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SyllabusSession",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SyllabusLearningOutcome",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "SyllabusLearningOutcome",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SyllabusLearningOutcome",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SyllabusLearningOutcome",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SyllabusLearningMaterial",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "SyllabusLearningMaterial",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SyllabusLearningMaterial",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SyllabusLearningMaterial",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SyllabusAssessment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "SyllabusAssessment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SyllabusAssessment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SyllabusAssessment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Syllabus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Syllabus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Syllabus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Syllabus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SubjectPrerequisite",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "SubjectPrerequisite",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SubjectPrerequisite",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SubjectPrerequisite",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "StudentProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "StudentProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "StudentProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "StudentProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "StaffProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "StaffProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "StaffProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "StaffProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SessionOutcomeMapping",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "SessionOutcomeMapping",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SessionOutcomeMapping",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SessionOutcomeMapping",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Program",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Program",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Program",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Program",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Message",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Message",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Message",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Message",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CurriculumSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CurriculumSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "CurriculumSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CurriculumSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Curriculum",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Curriculum",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Curriculum",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Curriculum",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ComboSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ComboSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ComboSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ComboSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Combo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Combo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Combo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Combo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AuditLog",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AuditLog",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AuditLog",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "AuditLog",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AuditLog",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AdvisorySession1to1",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "AdvisorySession1to1",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "AdvisorySession1to1",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "AdvisorySession1to1",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
