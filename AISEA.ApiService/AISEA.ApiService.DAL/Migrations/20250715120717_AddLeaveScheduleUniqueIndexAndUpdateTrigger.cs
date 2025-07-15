using Microsoft.EntityFrameworkCore.Migrations;

namespace AISEA.ApiService.DAL.Migrations
{
    public partial class AddLeaveScheduleUniqueIndexAndUpdateTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing index
            migrationBuilder.DropIndex(
                name: "IX_LeaveSchedule_StaffProfileId",
                table: "LeaveSchedule");

            // Add the Note column
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "LeaveSchedule",
                type: "nvarchar(max)",
                nullable: true);

            // Add unique index for StaffProfileId, StartDateTime, EndDateTime
            migrationBuilder.CreateIndex(
                name: "IX_LeaveSchedule_UniqueSchedule",
                table: "LeaveSchedule",
                columns: new[] { "StaffProfileId", "StartDateTime", "EndDateTime" },
                unique: true);

            // Drop the old trigger if it exists
            migrationBuilder.Sql("IF OBJECT_ID('TR_LeaveSchedule_CheckConstraints') IS NOT NULL DROP TRIGGER TR_LeaveSchedule_CheckConstraints;");

            // Create the updated trigger for overlap, booking availability, and booked meetings
            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_LeaveSchedule_CheckConstraints
                ON LeaveSchedule
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Check for overlapping time ranges
                    IF EXISTS (
                        SELECT 1
                        FROM LeaveSchedule AS existing
                        INNER JOIN inserted AS new
                            ON existing.StaffProfileId = new.StaffProfileId
                            AND existing.Id != new.Id
                            AND existing.StartDateTime < new.EndDateTime
                            AND existing.EndDateTime > new.StartDateTime
                    )
                    BEGIN
                        THROW 50003, 'The leave schedule overlaps with an existing leave schedule for the same staff.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END

                    -- Check for matching BookingAvailability
                    IF NOT EXISTS (
                        SELECT 1
                        FROM BookingAvailability ba
                        INNER JOIN inserted AS new
                            ON ba.StaffProfileId = new.StaffProfileId
                            AND ba.DayInWeek = DATEPART(WEEKDAY, new.StartDateTime)
                            AND ba.StartTime <= CAST(new.StartDateTime AS TIME)
                            AND ba.EndTime >= CAST(new.EndDateTime AS TIME)
                    )
                    BEGIN
                        THROW 50004, 'No matching booking availability found for the specified staff and time range.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END

                    -- Check for booked meetings with PENDING (1) or CONFIRMED (2) status
                    IF EXISTS (
                        SELECT 1
                        FROM BookedMeeting bm
                        INNER JOIN inserted AS new
                            ON bm.StaffProfileId = new.StaffProfileId
                            AND bm.Status IN (1, 2) -- PENDING or CONFIRMED
                            AND bm.StartDateTime < new.EndDateTime
                            AND bm.EndDateTime > new.StartDateTime
                    )
                    BEGIN
                        THROW 50005, 'Cannot register leave due to existing PENDING or CONFIRMED meetings. Cancel those meetings first.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                END;
            ");

            // Add description for the trigger
            migrationBuilder.Sql(@"
                EXEC sp_addextendedproperty 
                    @name = N'MS_Description', 
                    @value = N'Trigger to check overlaps, booking availability, and booked meetings', 
                    @level0type = N'SCHEMA', @level0name = N'dbo', 
                    @level1type = N'TABLE', @level1name = N'LeaveSchedule', 
                    @level2type = N'TRIGGER', @level2name = N'TR_LeaveSchedule_CheckConstraints';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the trigger
            migrationBuilder.Sql("IF OBJECT_ID('TR_LeaveSchedule_CheckConstraints') IS NOT NULL DROP TRIGGER TR_LeaveSchedule_CheckConstraints;");

            // Drop the unique index
            migrationBuilder.DropIndex(
                name: "IX_LeaveSchedule_UniqueSchedule",
                table: "LeaveSchedule");

            // Drop the Note column
            migrationBuilder.DropColumn(
                name: "Note",
                table: "LeaveSchedule");

            // Recreate the original index
            migrationBuilder.CreateIndex(
                name: "IX_LeaveSchedule_StaffProfileId",
                table: "LeaveSchedule",
                column: "StaffProfileId");
        }
    }
}