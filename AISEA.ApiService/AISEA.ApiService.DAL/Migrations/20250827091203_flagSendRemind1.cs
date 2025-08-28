using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class flagSendRemind1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReminderSentDays3",
                table: "JoinedSubjectCheckPoint",
                newName: "ReminderSentHours5");

            migrationBuilder.RenameColumn(
                name: "ReminderSentDays2",
                table: "JoinedSubjectCheckPoint",
                newName: "ReminderSentHours4");

            migrationBuilder.RenameColumn(
                name: "ReminderSentDays1",
                table: "JoinedSubjectCheckPoint",
                newName: "ReminderSentHours3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReminderSentHours5",
                table: "JoinedSubjectCheckPoint",
                newName: "ReminderSentDays3");

            migrationBuilder.RenameColumn(
                name: "ReminderSentHours4",
                table: "JoinedSubjectCheckPoint",
                newName: "ReminderSentDays2");

            migrationBuilder.RenameColumn(
                name: "ReminderSentHours3",
                table: "JoinedSubjectCheckPoint",
                newName: "ReminderSentDays1");
        }
    }
}
