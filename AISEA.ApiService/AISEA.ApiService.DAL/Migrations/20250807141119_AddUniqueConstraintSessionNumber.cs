using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintSessionNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // Add unique constraint for session number within each syllabus
            migrationBuilder.CreateIndex(
                name: "IX_SyllabusSession_SyllabusId_SessionNumber_Unique",
                table: "SyllabusSession",
                columns: new[] { "SyllabusId", "SessionNumber" },
                unique: true,
                filter: "IsDeleted = 0"); // Only enforce uniqueness for non-deleted records
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove unique constraint for session number
            migrationBuilder.DropIndex(
                name: "IX_SyllabusSession_SyllabusId_SessionNumber_Unique",
                table: "SyllabusSession");
        }
    }
}
