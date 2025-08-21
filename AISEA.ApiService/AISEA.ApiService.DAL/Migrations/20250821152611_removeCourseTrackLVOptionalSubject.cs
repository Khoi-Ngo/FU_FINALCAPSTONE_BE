using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class removeCourseTrackLVOptionalSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptionalSubjectCheckPoint");

            migrationBuilder.DropTable(
                name: "OptionalPersonalSubject");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OptionalPersonalSubject",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SemesterId = table.Column<long>(type: "bigint", nullable: false),
                    StudentProfileId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GithubRepositoryURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("optionalpersonalsubject_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "optionalpersonalsubject_semesterid_foreign",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "optionalpersonalsubject_studentprofileid_foreign",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OptionalSubjectCheckPoint",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionalPersonalSubjectId = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Link1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                name: "IX_OptionalPersonalSubject_SemesterId",
                table: "OptionalPersonalSubject",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "UX_OptionalPersonalSubject_Student_Semester_Subject",
                table: "OptionalPersonalSubject",
                columns: new[] { "StudentProfileId", "SemesterId", "SubjectCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OptionalSubjectCheckPoint_OptionalPersonalSubjectId",
                table: "OptionalSubjectCheckPoint",
                column: "OptionalPersonalSubjectId");
        }
    }
}
