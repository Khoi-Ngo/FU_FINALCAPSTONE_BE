using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class refineUniqLogicNameoFJoinSub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JoinedSubject_StudentProfileId_Name_Unique",
                table: "JoinedSubject");

            migrationBuilder.CreateIndex(
                name: "IX_JoinedSubject_StudentProfileId_Name_SemesterId_Unique",
                table: "JoinedSubject",
                columns: new[] { "StudentProfileId", "Name", "SemesterId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JoinedSubject_StudentProfileId_Name_SemesterId_Unique",
                table: "JoinedSubject");

            migrationBuilder.CreateIndex(
                name: "IX_JoinedSubject_StudentProfileId_Name_Unique",
                table: "JoinedSubject",
                columns: new[] { "StudentProfileId", "Name" },
                unique: true);
        }
    }
}
