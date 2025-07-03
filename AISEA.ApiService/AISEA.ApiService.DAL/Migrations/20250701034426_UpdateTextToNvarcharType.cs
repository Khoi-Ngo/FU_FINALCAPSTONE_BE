using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTextToNvarcharType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_SubjectPrerequisite_prerequisite_subject_id",
                table: "SubjectPrerequisite",
                newName: "IX_SubjectPrerequisite_PrerequisiteSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_SessionOutcomeMapping_outcome_id",
                table: "SessionOutcomeMapping",
                newName: "IX_SessionOutcomeMapping_OutcomeId");

            migrationBuilder.RenameIndex(
                name: "IX_CurriculumSubject_subject_id",
                table: "CurriculumSubject",
                newName: "IX_CurriculumSubject_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ComboSubject_subject_id",
                table: "ComboSubject",
                newName: "IX_ComboSubject_SubjectId");

            migrationBuilder.AlterColumn<string>(
                name: "CareerGoal",
                table: "StudentProfile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Role",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Message",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectPrerequisite_SubjectId",
                table: "SubjectPrerequisite",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "subject_code_unique",
                table: "Subject",
                column: "SubjectCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionOutcomeMapping_SessionId",
                table: "SessionOutcomeMapping",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "program_code_unique",
                table: "Program",
                column: "ProgramCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_CreatedAt",
                table: "Notification",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_IsRead",
                table: "Notification",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Message_CreatedAt",
                table: "Message",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumSubject_CurriculumId",
                table: "CurriculumSubject",
                column: "curriculum_id");

            migrationBuilder.CreateIndex(
                name: "IX_ComboSubject_ComboId",
                table: "ComboSubject",
                column: "combo_id");

            migrationBuilder.CreateIndex(
                name: "IX_AdvisorySession1to1_CreatedAt",
                table: "AdvisorySession1to1",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubjectPrerequisite_SubjectId",
                table: "SubjectPrerequisite");

            migrationBuilder.DropIndex(
                name: "subject_code_unique",
                table: "Subject");

            migrationBuilder.DropIndex(
                name: "IX_SessionOutcomeMapping_SessionId",
                table: "SessionOutcomeMapping");

            migrationBuilder.DropIndex(
                name: "program_code_unique",
                table: "Program");

            migrationBuilder.DropIndex(
                name: "IX_Notification_CreatedAt",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_IsRead",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Message_CreatedAt",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_CurriculumSubject_CurriculumId",
                table: "CurriculumSubject");

            migrationBuilder.DropIndex(
                name: "IX_ComboSubject_ComboId",
                table: "ComboSubject");

            migrationBuilder.DropIndex(
                name: "IX_AdvisorySession1to1_CreatedAt",
                table: "AdvisorySession1to1");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectPrerequisite_PrerequisiteSubjectId",
                table: "SubjectPrerequisite",
                newName: "IX_SubjectPrerequisite_prerequisite_subject_id");

            migrationBuilder.RenameIndex(
                name: "IX_SessionOutcomeMapping_OutcomeId",
                table: "SessionOutcomeMapping",
                newName: "IX_SessionOutcomeMapping_outcome_id");

            migrationBuilder.RenameIndex(
                name: "IX_CurriculumSubject_SubjectId",
                table: "CurriculumSubject",
                newName: "IX_CurriculumSubject_subject_id");

            migrationBuilder.RenameIndex(
                name: "IX_ComboSubject_SubjectId",
                table: "ComboSubject",
                newName: "IX_ComboSubject_subject_id");

            migrationBuilder.AlterColumn<string>(
                name: "CareerGoal",
                table: "StudentProfile",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Role",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "Notification",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Notification",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Message",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
