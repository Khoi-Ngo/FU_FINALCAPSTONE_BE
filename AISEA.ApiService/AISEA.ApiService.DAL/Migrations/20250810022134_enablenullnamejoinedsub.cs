using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class enablenullnamejoinedsub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JoinedSubject_StudentProfileId_Name_SemesterId_Unique",
                table: "JoinedSubject");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "JoinedSubject",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_JoinedSubject_StudentProfileId_Name_SemesterId_Unique",
                table: "JoinedSubject",
                columns: new[] { "StudentProfileId", "Name", "SemesterId" },
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JoinedSubject_StudentProfileId_Name_SemesterId_Unique",
                table: "JoinedSubject");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "JoinedSubject",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JoinedSubject_StudentProfileId_Name_SemesterId_Unique",
                table: "JoinedSubject",
                columns: new[] { "StudentProfileId", "Name", "SemesterId" },
                unique: true);
        }
    }
}
