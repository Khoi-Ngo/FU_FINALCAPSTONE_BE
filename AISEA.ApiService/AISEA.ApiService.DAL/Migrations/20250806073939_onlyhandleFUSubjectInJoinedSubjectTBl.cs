using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class onlyhandleFUSubjectInJoinedSubjectTBl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoinCourse");

            migrationBuilder.CreateTable(
                name: "JoinedSubject",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GithubRepositoryURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubjectVersionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SemesterNumber = table.Column<int>(type: "int", nullable: false),
                    SemesterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: true),
                    StudentProfileId = table.Column<long>(type: "bigint", nullable: false),
                    SemesterId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("joinedsubject_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "joinedcourse_semesterid_foreign",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "joinedsubject_studentprofileid_foreign",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JoinedSubject_SemesterId",
                table: "JoinedSubject",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinedSubject_StudentProfileId_Name_Unique",
                table: "JoinedSubject",
                columns: new[] { "StudentProfileId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoinedSubject");

            migrationBuilder.CreateTable(
                name: "JoinCourse",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SemesterId = table.Column<long>(type: "bigint", nullable: false),
                    StudentProfileId = table.Column<long>(type: "bigint", nullable: false),
                    CourseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CourseName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseVersionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: true),
                    GithubRepositoryURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    SemesterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SemesterNumber = table.Column<int>(type: "int", nullable: true),
                    type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("joinedcourse_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "joinedcourse_semesterid_foreign",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "joinedcourse_studentprofileid_foreign",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JoinCourse_SemesterId",
                table: "JoinCourse",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinedCourse_StudentProfileId_CourseName_Unique",
                table: "JoinCourse",
                columns: new[] { "StudentProfileId", "CourseName" },
                unique: true);
        }
    }
}
