using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeConstraint01JoinedSub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JoinedSubject_StudentProfileId_Name_SemesterId_Unique",
                table: "JoinedSubject");

            migrationBuilder.AlterColumn<string>(
                name: "SubjectCode",
                table: "JoinedSubject",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "JoinedSubject",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UX_JoinedSubject_Student_Semester_BlockType_Subject",
                table: "JoinedSubject",
                columns: new[] { "StudentProfileId", "SemesterId", "SemesterStudyBlockType", "SubjectCode" },
                unique: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_JoinedSubject_Student_Semester_BlockType_Subject",
                table: "JoinedSubject");

            migrationBuilder.AlterColumn<string>(
                name: "SubjectCode",
                table: "JoinedSubject",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "JoinedSubject",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JoinedSubject_StudentProfileId_Name_SemesterId_Unique",
                table: "JoinedSubject",
                columns: new[] { "StudentProfileId", "Name", "SemesterId" },
                unique: true,
                filter: "[Name] IS NOT NULL");

        }
    }
}
