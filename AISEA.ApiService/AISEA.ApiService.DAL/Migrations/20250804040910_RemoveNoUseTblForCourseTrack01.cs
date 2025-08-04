using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNoUseTblForCourseTrack01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DelayJoinedCourse");

            migrationBuilder.DropColumn(
                name: "CurrentSemesterNumber",
                table: "StudentProfile");

            migrationBuilder.DropColumn(
                name: "IsCurrentPostponed",
                table: "StudentProfile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentSemesterNumber",
                table: "StudentProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrentPostponed",
                table: "StudentProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DelayJoinedCourse",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentProfileId = table.Column<long>(type: "bigint", nullable: false),
                    EndValidDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ReasonDelay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartValidDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubjectCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("delayjoinedcourse_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "delayjoinedcourse_studentprofileid_foreign",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DelayJoinedCourse_StudentProfileId",
                table: "DelayJoinedCourse",
                column: "StudentProfileId");
        }
    }
}
