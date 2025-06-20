using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixUserProfilesRelate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentProfile_UserId",
                table: "StudentProfile");

            migrationBuilder.DropIndex(
                name: "IX_StaffProfile_UserId",
                table: "StaffProfile");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfile_UserId",
                table: "StudentProfile",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaffProfile_UserId",
                table: "StaffProfile",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentProfile_UserId",
                table: "StudentProfile");

            migrationBuilder.DropIndex(
                name: "IX_StaffProfile_UserId",
                table: "StaffProfile");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfile_UserId",
                table: "StudentProfile",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffProfile_UserId",
                table: "StaffProfile",
                column: "UserId");
        }
    }
}
