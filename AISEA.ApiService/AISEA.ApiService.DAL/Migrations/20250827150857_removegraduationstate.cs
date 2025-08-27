using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class removegraduationstate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoGraduate",
                table: "StudentProfile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DoGraduate",
                table: "StudentProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
