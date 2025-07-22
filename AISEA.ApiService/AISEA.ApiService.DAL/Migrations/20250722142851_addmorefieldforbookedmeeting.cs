using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addmorefieldforbookedmeeting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckinCode",
                table: "BookedMeeting",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmCheckinCode",
                table: "BookedMeeting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentIssue",
                table: "BookedMeeting",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SuggestionFromAdvisor",
                table: "BookedMeeting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleStudentIssue",
                table: "BookedMeeting",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckinCode",
                table: "BookedMeeting");

            migrationBuilder.DropColumn(
                name: "ConfirmCheckinCode",
                table: "BookedMeeting");

            migrationBuilder.DropColumn(
                name: "ContentIssue",
                table: "BookedMeeting");

            migrationBuilder.DropColumn(
                name: "SuggestionFromAdvisor",
                table: "BookedMeeting");

            migrationBuilder.DropColumn(
                name: "TitleStudentIssue",
                table: "BookedMeeting");
        }
    }
}
