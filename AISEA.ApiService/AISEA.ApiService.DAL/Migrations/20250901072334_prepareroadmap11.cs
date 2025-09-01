using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class prepareroadmap11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "StudyRoadMapNodes");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "StudyRoadMapNodes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StudyRoadMap");

            migrationBuilder.RenameColumn(
                name: "URL",
                table: "StudyRoadMapNodes",
                newName: "SubjectName");

            migrationBuilder.AlterColumn<int>(
                name: "SemesterNumber",
                table: "StudyRoadMapNodes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "StudyRoadMapNodes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsInternalSubjectData",
                table: "StudyRoadMapNodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "StudyRoadMapNodeLinks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromNodeId = table.Column<long>(type: "bigint", nullable: false),
                    ToNodeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("studyroadmapnodelink_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "studyroadmapnodelink_fromnodeid_foreign",
                        column: x => x.FromNodeId,
                        principalTable: "StudyRoadMapNodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "studyroadmapnodelink_tonodeid_foreign",
                        column: x => x.ToNodeId,
                        principalTable: "StudyRoadMapNodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyRoadMapNodeLinks_FromNodeId",
                table: "StudyRoadMapNodeLinks",
                column: "FromNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyRoadMapNodeLinks_ToNodeId",
                table: "StudyRoadMapNodeLinks",
                column: "ToNodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyRoadMapNodeLinks");

            migrationBuilder.DropColumn(
                name: "IsInternalSubjectData",
                table: "StudyRoadMapNodes");

            migrationBuilder.RenameColumn(
                name: "SubjectName",
                table: "StudyRoadMapNodes",
                newName: "URL");

            migrationBuilder.AlterColumn<int>(
                name: "SemesterNumber",
                table: "StudyRoadMapNodes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "StudyRoadMapNodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "StudyRoadMapNodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "StudyRoadMapNodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StudyRoadMap",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
