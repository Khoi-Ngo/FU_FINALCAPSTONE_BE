using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class check01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "bookingavailability_staffprofileid_foreign",
                table: "BookingAvailability");

            migrationBuilder.AddForeignKey(
                name: "bookingavailability_staffprofileid_foreign",
                table: "BookingAvailability",
                column: "StaffProfileId",
                principalTable: "StaffProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "bookingavailability_staffprofileid_foreign",
                table: "BookingAvailability");

            migrationBuilder.AddForeignKey(
                name: "bookingavailability_staffprofileid_foreign",
                table: "BookingAvailability",
                column: "StaffProfileId",
                principalTable: "StaffProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
