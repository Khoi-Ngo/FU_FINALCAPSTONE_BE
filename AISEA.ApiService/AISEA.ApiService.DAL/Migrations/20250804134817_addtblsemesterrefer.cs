using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addtblsemesterrefer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JoinCourse_StudentProfileId",
                table: "JoinCourse");

            migrationBuilder.AlterColumn<string>(
                name: "CourseName",
                table: "JoinCourse",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<long>(
                name: "SemesterId",
                table: "JoinCourse",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Semester",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SemesterName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("semester_id_primary", x => x.id);
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

            migrationBuilder.AddForeignKey(
                name: "joinedcourse_semesterid_foreign",
                table: "JoinCourse",
                column: "SemesterId",
                principalTable: "Semester",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "joinedcourse_semesterid_foreign",
                table: "JoinCourse");

            migrationBuilder.DropTable(
                name: "Semester");

            migrationBuilder.DropIndex(
                name: "IX_JoinCourse_SemesterId",
                table: "JoinCourse");

            migrationBuilder.DropIndex(
                name: "IX_JoinedCourse_StudentProfileId_CourseName_Unique",
                table: "JoinCourse");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "JoinCourse");

            migrationBuilder.AlterColumn<string>(
                name: "CourseName",
                table: "JoinCourse",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_JoinCourse_StudentProfileId",
                table: "JoinCourse",
                column: "StudentProfileId");
        }
    }
}
