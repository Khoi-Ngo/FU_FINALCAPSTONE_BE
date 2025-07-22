using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class trycompleteTriggerLeaveSche01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing trigger first
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_LeaveSchedule_CheckConstraints') IS NOT NULL
                    DROP TRIGGER TR_LeaveSchedule_CheckConstraints;
            ");

            // Recreate trigger with BookingAvailability check included
            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_LeaveSchedule_CheckConstraints
                ON LeaveSchedule
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Overlapping leave schedules
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

                    -- BookedMeeting status check for PENDING (1) or CONFIRMED (2)
                    IF EXISTS (
                        SELECT 1
                        FROM BookedMeeting bm
                        INNER JOIN inserted AS new
                            ON bm.StaffProfileId = new.StaffProfileId
                            AND bm.Status IN (1, 2)
                            AND bm.StartDateTime < new.EndDateTime
                            AND bm.EndDateTime > new.StartDateTime
                    )
                    BEGIN
                        THROW 50005, 'Cannot register leave due to existing PENDING or CONFIRMED meetings. Cancel those meetings first.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END

                    -- BookingAvailability matching check (time range overlaps any availability on that day)
                    IF NOT EXISTS (
                        SELECT 1
                        FROM BookingAvailability ba
                        INNER JOIN inserted AS new
                            ON ba.StaffProfileId = new.StaffProfileId
                            AND ba.DayInWeek = DATEPART(WEEKDAY, new.StartDateTime)
                            AND (
                                (CAST(new.StartDateTime AS time) < ba.EndTime)
                                AND (CAST(new.EndDateTime AS time) > ba.StartTime)
                            )
                    )
                    BEGIN
                        THROW 50004, 'No matching booking availability found for the specified staff and time range.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                END;
            ");

            // Add trigger description
            migrationBuilder.Sql(@"
                EXEC sp_addextendedproperty 
                    @name = N'MS_Description', 
                    @value = N'Trigger to check overlapping leave schedules, booked meetings, and matching booking availability.', 
                    @level0type = N'SCHEMA', @level0name = N'dbo', 
                    @level1type = N'TABLE', @level1name = N'LeaveSchedule', 
                    @level2type = N'TRIGGER', @level2name = N'TR_LeaveSchedule_CheckConstraints';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop trigger if rolling back migration
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_LeaveSchedule_CheckConstraints') IS NOT NULL
                    DROP TRIGGER TR_LeaveSchedule_CheckConstraints;
            ");
        }
    }
}
