using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addjoinedsubcheckpointtbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JoinedSubjectCheckPoint",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JoinedSubjectId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("joinedsubjectcheckpoint_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "joinedsubjectcheckpoint_joinedsubjectid_foreign",
                        column: x => x.JoinedSubjectId,
                        principalTable: "JoinedSubject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JoinedSubjectCheckPoint_JoinedSubjectId",
                table: "JoinedSubjectCheckPoint",
                column: "JoinedSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoinedSubjectCheckPoint");
        }
    }
}
