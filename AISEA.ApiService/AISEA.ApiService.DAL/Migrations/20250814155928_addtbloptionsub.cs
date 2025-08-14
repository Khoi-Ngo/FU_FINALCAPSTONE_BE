using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addtbloptionsub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OptionalPersonalSubject",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GithubRepositoryURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    StudentProfileId = table.Column<long>(type: "bigint", nullable: false),
                    SemesterId = table.Column<long>(type: "bigint", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_OptionalPersonalSubject_SemesterId",
                table: "OptionalPersonalSubject",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "UX_OptionalPersonalSubject_Student_Semester_Subject",
                table: "OptionalPersonalSubject",
                columns: new[] { "StudentProfileId", "SemesterId", "SubjectCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptionalPersonalSubject");
        }
    }
}
