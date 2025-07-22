using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveScheduleSingleDayConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_LeaveSchedule_WithinSingleDay",
                table: "LeaveSchedule",
                sql: "CAST([StartDateTime] AS date) = CAST([EndDateTime] AS date)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_LeaveSchedule_WithinSingleDay",
                table: "LeaveSchedule");
        }
    }
}
