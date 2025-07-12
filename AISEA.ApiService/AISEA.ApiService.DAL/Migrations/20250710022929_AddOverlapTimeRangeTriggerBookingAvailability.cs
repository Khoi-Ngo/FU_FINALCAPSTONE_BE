using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    public partial class AddOverlapTimeRangeTriggerBookingAvailability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE TRIGGER TR_BookingAvailability_CheckOverlap
            ON BookingAvailability
            AFTER INSERT, UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;
                IF EXISTS (
                    SELECT 1
                    FROM BookingAvailability AS existing
                    INNER JOIN inserted AS new
                        ON existing.StaffProfileId = new.StaffProfileId
                        AND existing.DayInWeek = new.DayInWeek
                        AND existing.Id != new.Id
                        AND existing.StartTime < new.EndTime
                        AND existing.EndTime > new.StartTime
                )
                BEGIN
                    THROW 50001, 'The time slot overlaps with an existing slot for the same staff and day.', 1;
                    ROLLBACK TRANSACTION;
                END
            END;
        ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER TR_BookingAvailability_CheckOverlap;");
        }
    }
}
