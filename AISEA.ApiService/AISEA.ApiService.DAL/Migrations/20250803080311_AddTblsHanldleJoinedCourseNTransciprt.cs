using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTblsHanldleJoinedCourseNTransciprt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "RegisteredComboCode",
                table: "StudentProfile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DelayJoinedCourse",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartValidDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndValidDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ReasonDelay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentProfileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelayJoinedCourse", x => x.id);
                    table.ForeignKey(
                        name: "FK_DelayJoinedCourse_StudentProfile_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinCourse",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    GithubRepositoryURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CourseVersionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SemesterNumber = table.Column<int>(type: "int", nullable: true),
                    SemesterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: true),
                    StudentProfileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinCourse", x => x.id);
                    table.ForeignKey(
                        name: "FK_JoinCourse_StudentProfile_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DelayJoinedCourse_StudentProfileId",
                table: "DelayJoinedCourse",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinCourse_StudentProfileId",
                table: "JoinCourse",
                column: "StudentProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DelayJoinedCourse");

            migrationBuilder.DropTable(
                name: "JoinCourse");

            migrationBuilder.DropColumn(
                name: "CurrentSemesterNumber",
                table: "StudentProfile");

            migrationBuilder.DropColumn(
                name: "IsCurrentPostponed",
                table: "StudentProfile");

            migrationBuilder.DropColumn(
                name: "RegisteredComboCode",
                table: "StudentProfile");
        }
    }
}
