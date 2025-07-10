using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addconstraintbookingavai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookingAvailability_StaffProfileId",
                table: "BookingAvailability");

            migrationBuilder.CreateIndex(
                name: "IX_BookingAvailability_UniqueTimeSlot",
                table: "BookingAvailability",
                columns: new[] { "StaffProfileId", "DayInWeek", "StartTime", "EndTime" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_BookingAvailability_EndTime",
                table: "BookingAvailability",
                sql: "[EndTime] > [StartTime]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookingAvailability_UniqueTimeSlot",
                table: "BookingAvailability");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BookingAvailability_EndTime",
                table: "BookingAvailability");

            migrationBuilder.CreateIndex(
                name: "IX_BookingAvailability_StaffProfileId",
                table: "BookingAvailability",
                column: "StaffProfileId");
        }
    }
}
