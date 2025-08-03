using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDbSetJoinedCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DelayJoinedCourse_StudentProfile_StudentProfileId",
                table: "DelayJoinedCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinCourse_StudentProfile_StudentProfileId",
                table: "JoinCourse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JoinCourse",
                table: "JoinCourse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DelayJoinedCourse",
                table: "DelayJoinedCourse");

            migrationBuilder.AddColumn<bool>(
                name: "IsPassed",
                table: "JoinCourse",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "joinedcourse_id_primary",
                table: "JoinCourse",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "delayjoinedcourse_id_primary",
                table: "DelayJoinedCourse",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "delayjoinedcourse_studentprofileid_foreign",
                table: "DelayJoinedCourse",
                column: "StudentProfileId",
                principalTable: "StudentProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "joinedcourse_studentprofileid_foreign",
                table: "JoinCourse",
                column: "StudentProfileId",
                principalTable: "StudentProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "delayjoinedcourse_studentprofileid_foreign",
                table: "DelayJoinedCourse");

            migrationBuilder.DropForeignKey(
                name: "joinedcourse_studentprofileid_foreign",
                table: "JoinCourse");

            migrationBuilder.DropPrimaryKey(
                name: "joinedcourse_id_primary",
                table: "JoinCourse");

            migrationBuilder.DropPrimaryKey(
                name: "delayjoinedcourse_id_primary",
                table: "DelayJoinedCourse");

            migrationBuilder.DropColumn(
                name: "IsPassed",
                table: "JoinCourse");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JoinCourse",
                table: "JoinCourse",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DelayJoinedCourse",
                table: "DelayJoinedCourse",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_DelayJoinedCourse_StudentProfile_StudentProfileId",
                table: "DelayJoinedCourse",
                column: "StudentProfileId",
                principalTable: "StudentProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinCourse_StudentProfile_StudentProfileId",
                table: "JoinCourse",
                column: "StudentProfileId",
                principalTable: "StudentProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
