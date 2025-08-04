using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class constraintUniqueSemesterRefername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Semester_SemesterName_Unique",
                table: "Semester",
                column: "SemesterName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Semester_SemesterName_Unique",
                table: "Semester");
        }
    }
}
