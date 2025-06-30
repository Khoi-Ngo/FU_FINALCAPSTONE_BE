using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTextColumnsToNvarchar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
              # Update Text Columns to Nvarchar(max) for Multi-language Support

              1. Updated Tables and Columns
                - `Role` table: `Description` column (text → nvarchar(max))
                - `Message` table: `Content` column (text → nvarchar(max))
                - `Notification` table: `Content` and `Link` columns (text → nvarchar(max))
                - `StudentProfile` table: `CareerGoal` column (text → nvarchar(max))
                - `Subject` table: `Description` column (text → nvarchar(max))
                - `Syllabus` table: `Content` column (text → nvarchar(max))
                - `SyllabusAssessment` table: `CompletionCriteria` column (text → nvarchar(max))
                - `SyllabusLearningMaterial` table: `Description` column (text → nvarchar(max))
                - `SyllabusLearningOutcome` table: `Description` column (text → nvarchar(max))
                - `SyllabusSession` table: `Mission` column (text → nvarchar(max))
                - `Combo` table: `ComboDescription` column (text → nvarchar(max))

              2. Benefits
                - Better Unicode support for multi-language content
                - Improved performance compared to deprecated 'text' type
                - Consistent with modern SQL Server best practices
                - Support for Vietnamese, Chinese, Japanese, and other Unicode characters

              3. Notes
                - Using nvarchar(max) instead of ntext (deprecated)
                - All existing data will be preserved during migration
                - No application code changes required
            */

            // Update Role table
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Role",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            // Update Message table
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Message",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            // Update Notification table
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Notification",
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

            // Update StudentProfile table
            migrationBuilder.AlterColumn<string>(
                name: "CareerGoal",
                table: "StudentProfile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Update Subject table
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Update Syllabus table
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Syllabus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            // Update SyllabusAssessment table
            migrationBuilder.AlterColumn<string>(
                name: "CompletionCriteria",
                table: "SyllabusAssessment",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Update SyllabusLearningMaterial table
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SyllabusLearningMaterial",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Update SyllabusLearningOutcome table
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SyllabusLearningOutcome",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            // Update SyllabusSession table
            migrationBuilder.AlterColumn<string>(
                name: "Mission",
                table: "SyllabusSession",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Update Combo table
            migrationBuilder.AlterColumn<string>(
                name: "ComboDescription",
                table: "Combo",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert Role table
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Role",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Revert Message table
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Message",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Revert Notification table
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Notification",
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

            // Revert StudentProfile table
            migrationBuilder.AlterColumn<string>(
                name: "CareerGoal",
                table: "StudentProfile",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Revert Subject table
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Subject",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Revert Syllabus table
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Syllabus",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Revert SyllabusAssessment table
            migrationBuilder.AlterColumn<string>(
                name: "CompletionCriteria",
                table: "SyllabusAssessment",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Revert SyllabusLearningMaterial table
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SyllabusLearningMaterial",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Revert SyllabusLearningOutcome table
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SyllabusLearningOutcome",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Revert SyllabusSession table
            migrationBuilder.AlterColumn<string>(
                name: "Mission",
                table: "SyllabusSession",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Revert Combo table
            migrationBuilder.AlterColumn<string>(
                name: "ComboDescription",
                table: "Combo",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}