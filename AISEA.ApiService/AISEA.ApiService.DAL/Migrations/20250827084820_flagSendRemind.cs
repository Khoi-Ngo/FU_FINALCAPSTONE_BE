using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class flagSendRemind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReminderSentDays1",
                table: "JoinedSubjectCheckPoint",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReminderSentDays2",
                table: "JoinedSubjectCheckPoint",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReminderSentDays3",
                table: "JoinedSubjectCheckPoint",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReminderSentHours1",
                table: "JoinedSubjectCheckPoint",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReminderSentHours2",
                table: "JoinedSubjectCheckPoint",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderSentDays1",
                table: "JoinedSubjectCheckPoint");

            migrationBuilder.DropColumn(
                name: "ReminderSentDays2",
                table: "JoinedSubjectCheckPoint");

            migrationBuilder.DropColumn(
                name: "ReminderSentDays3",
                table: "JoinedSubjectCheckPoint");

            migrationBuilder.DropColumn(
                name: "ReminderSentHours1",
                table: "JoinedSubjectCheckPoint");

            migrationBuilder.DropColumn(
                name: "ReminderSentHours2",
                table: "JoinedSubjectCheckPoint");
        }
    }
}
