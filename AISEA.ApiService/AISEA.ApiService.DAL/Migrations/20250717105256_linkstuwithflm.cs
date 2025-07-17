using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class linkstuwithflm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProgramId",
                table: "StudentProfile",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfile_ProgramId",
                table: "StudentProfile",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "studentprofile_programid_foreign",
                table: "StudentProfile",
                column: "ProgramId",
                principalTable: "Program",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "studentprofile_programid_foreign",
                table: "StudentProfile");

            migrationBuilder.DropIndex(
                name: "IX_StudentProfile_ProgramId",
                table: "StudentProfile");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "StudentProfile");
        }
    }
}
