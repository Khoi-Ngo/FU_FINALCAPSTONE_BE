using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addtbloptionsubcheckpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OptionalSubjectCheckPoint",
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
                    OptionalPersonalSubjectId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("optionalsubjectcheckpoint_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "optionalsubjectcheckpoint_optionalpersonalsubjectid_foreign",
                        column: x => x.OptionalPersonalSubjectId,
                        principalTable: "OptionalPersonalSubject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptionalSubjectCheckPoint_OptionalPersonalSubjectId",
                table: "OptionalSubjectCheckPoint",
                column: "OptionalPersonalSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptionalSubjectCheckPoint");
        }
    }
}
