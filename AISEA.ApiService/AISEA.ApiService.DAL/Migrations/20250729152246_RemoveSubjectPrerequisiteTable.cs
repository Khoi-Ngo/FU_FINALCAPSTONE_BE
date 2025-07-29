using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSubjectPrerequisiteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectPrerequisite");

            migrationBuilder.CreateTable(
                name: "SubjectVersionPrerequisite",
                columns: table => new
                {
                    subject_version_id = table.Column<long>(type: "bigint", nullable: false),
                    prerequisite_subject_version_id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subjectversionprerequisite_composite_primary", x => new { x.subject_version_id, x.prerequisite_subject_version_id });
                    table.ForeignKey(
                        name: "subjectversionprerequisite_prerequisitesubjectversionid_foreign",
                        column: x => x.prerequisite_subject_version_id,
                        principalTable: "SubjectVersion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "subjectversionprerequisite_subjectversionid_foreign",
                        column: x => x.subject_version_id,
                        principalTable: "SubjectVersion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVersionPrerequisite_PrerequisiteSubjectVersionId",
                table: "SubjectVersionPrerequisite",
                column: "prerequisite_subject_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVersionPrerequisite_SubjectVersionId",
                table: "SubjectVersionPrerequisite",
                column: "subject_version_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectVersionPrerequisite");

            migrationBuilder.CreateTable(
                name: "SubjectPrerequisite",
                columns: table => new
                {
                    subject_id = table.Column<long>(type: "bigint", nullable: false),
                    prerequisite_subject_id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subjectprerequisite_composite_primary", x => new { x.subject_id, x.prerequisite_subject_id });
                    table.ForeignKey(
                        name: "subjectprerequisite_prerequisitesubjectid_foreign",
                        column: x => x.prerequisite_subject_id,
                        principalTable: "Subject",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "subjectprerequisite_subjectid_foreign",
                        column: x => x.subject_id,
                        principalTable: "Subject",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectPrerequisite_PrerequisiteSubjectId",
                table: "SubjectPrerequisite",
                column: "prerequisite_subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectPrerequisite_SubjectId",
                table: "SubjectPrerequisite",
                column: "subject_id");
        }
    }
}
