using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectVersionFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SubjectVersionId",
                table: "Syllabus",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubjectVersionId",
                table: "SubjectClass",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubjectVersionId",
                table: "CurriculumSubject",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubjectVersion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<long>(type: "bigint", nullable: false),
                    VersionCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VersionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectVersion", x => x.id);
                    table.ForeignKey(
                        name: "FK_SubjectVersion_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Syllabus_SubjectVersionId",
                table: "Syllabus",
                column: "SubjectVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectClass_SubjectVersionId",
                table: "SubjectClass",
                column: "SubjectVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumSubject_SubjectVersionId",
                table: "CurriculumSubject",
                column: "SubjectVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVersion_SubjectId_VersionCode",
                table: "SubjectVersion",
                columns: new[] { "SubjectId", "VersionCode" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CurriculumSubject_SubjectVersion_SubjectVersionId",
                table: "CurriculumSubject",
                column: "SubjectVersionId",
                principalTable: "SubjectVersion",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectClass_SubjectVersion_SubjectVersionId",
                table: "SubjectClass",
                column: "SubjectVersionId",
                principalTable: "SubjectVersion",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabus_SubjectVersion_SubjectVersionId",
                table: "Syllabus",
                column: "SubjectVersionId",
                principalTable: "SubjectVersion",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurriculumSubject_SubjectVersion_SubjectVersionId",
                table: "CurriculumSubject");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectClass_SubjectVersion_SubjectVersionId",
                table: "SubjectClass");

            migrationBuilder.DropForeignKey(
                name: "FK_Syllabus_SubjectVersion_SubjectVersionId",
                table: "Syllabus");

            migrationBuilder.DropTable(
                name: "SubjectVersion");

            migrationBuilder.DropIndex(
                name: "IX_Syllabus_SubjectVersionId",
                table: "Syllabus");

            migrationBuilder.DropIndex(
                name: "IX_SubjectClass_SubjectVersionId",
                table: "SubjectClass");

            migrationBuilder.DropIndex(
                name: "IX_CurriculumSubject_SubjectVersionId",
                table: "CurriculumSubject");

            migrationBuilder.DropColumn(
                name: "SubjectVersionId",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "SubjectVersionId",
                table: "SubjectClass");

            migrationBuilder.DropColumn(
                name: "SubjectVersionId",
                table: "CurriculumSubject");
        }
    }
}
