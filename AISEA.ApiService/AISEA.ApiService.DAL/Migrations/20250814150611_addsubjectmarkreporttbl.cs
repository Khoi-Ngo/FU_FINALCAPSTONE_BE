using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addsubjectmarkreporttbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectMarkReport",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    MinScore = table.Column<double>(type: "float", nullable: false),
                    ScoreUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JoinedSubjectId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subjectmarkreport_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "subjectmarkreport_joinedsubjectid_foreign",
                        column: x => x.JoinedSubjectId,
                        principalTable: "JoinedSubject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectMarkReport_JoinedSubjectId",
                table: "SubjectMarkReport",
                column: "JoinedSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectMarkReport");
        }
    }
}
