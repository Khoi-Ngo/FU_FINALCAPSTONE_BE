using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailAndFullNameToSubjectComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectComment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<long>(type: "bigint", nullable: false),
                    StudentProfileId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    LikedByStudentIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnlikedByStudentIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subjectcomment_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "subjectcomment_studentprofileid_foreign",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "subjectcomment_subjectid_foreign",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectComment_CreatedAt",
                table: "SubjectComment",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectComment_Student_Subject_Unique",
                table: "SubjectComment",
                columns: new[] { "StudentProfileId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectComment_StudentProfileId",
                table: "SubjectComment",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectComment_SubjectId",
                table: "SubjectComment",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectComment");
        }
    }
}
