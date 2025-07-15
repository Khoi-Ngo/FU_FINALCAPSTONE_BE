using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStartEndTimeCheckConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE BookingAvailability DROP CONSTRAINT CK_BookingAvailability_EndTime;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE BookingAvailability
                ADD CONSTRAINT CK_BookingAvailability_EndTime CHECK (StartTime < EndTime);
            ");
        }
    }
}
