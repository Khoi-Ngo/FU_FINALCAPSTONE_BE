using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TempRemoveCheckBookingAvaiInMeetingTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //TODO: Have to comeback to check booking avai later || write worker service to compare then disable due to not fitting with the BookingAvai
            // Drop existing trigger if it exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_BookedMeeting_CheckExternalTables') IS NOT NULL
                    DROP TRIGGER TR_BookedMeeting_CheckExternalTables;
            ");

            // Create updated trigger without booking availability check
            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_BookedMeeting_CheckExternalTables
                ON BookedMeeting
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Check if student has reached ban limit (10)
                    IF EXISTS (
                        SELECT 1
                        FROM StudentProfile sp
                        INNER JOIN inserted i
                            ON sp.Id = i.StudentProfileId
                        WHERE sp.NumberOfBan >= 10
                    )
                    BEGIN
                        THROW 50006, 'Student has reached the maximum number of bans (10). Cannot book meeting.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END

                    -- Check for overlapping leave schedules
                    IF EXISTS (
                        SELECT 1
                        FROM LeaveSchedule ls
                        INNER JOIN inserted i
                            ON ls.StaffProfileId = i.StaffProfileId
                            AND ls.StartDateTime < i.EndDateTime
                            AND ls.EndDateTime > i.StartDateTime
                    )
                    BEGIN
                        THROW 50007, 'The meeting time conflicts with staff''s leave schedule.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                END;
            ");

            // Update trigger description
            migrationBuilder.Sql(@"
                EXEC sp_addextendedproperty 
                    @name = N'MS_Description', 
                    @value = N'Trigger to check student ban limit (10) and leave schedule conflicts for BookedMeeting.', 
                    @level0type = N'SCHEMA', @level0name = N'dbo', 
                    @level1type = N'TABLE', @level1name = N'BookedMeeting', 
                    @level2type = N'TRIGGER', @level2name = N'TR_BookedMeeting_CheckExternalTables';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop existing trigger if it exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_BookedMeeting_CheckExternalTables') IS NOT NULL
                    DROP TRIGGER TR_BookedMeeting_CheckExternalTables;
            ");

            // Revert to previous trigger with booking availability check
            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_BookedMeeting_CheckExternalTables
                ON BookedMeeting
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Check if student has reached ban limit (10)
                    IF EXISTS (
                        SELECT 1
                        FROM StudentProfile sp
                        INNER JOIN inserted i
                            ON sp.Id = i.StudentProfileId
                        WHERE sp.NumberOfBan >= 10
                    )
                    BEGIN
                        THROW 50006, 'Student has reached the maximum number of bans (10). Cannot book meeting.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END

                    -- Check for overlapping leave schedules
                    IF EXISTS (
                        SELECT 1
                        FROM LeaveSchedule ls
                        INNER JOIN inserted i
                            ON ls.StaffProfileId = i.StaffProfileId
                            AND ls.StartDateTime < i.EndDateTime
                            AND ls.EndDateTime > i.StartDateTime
                    )
                    BEGIN
                        THROW 50007, 'The meeting time conflicts with staff''s leave schedule.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END

                    -- Check if meeting time falls within booking availability
                    IF NOT EXISTS (
                        SELECT 1
                        FROM BookingAvailability ba
                        INNER JOIN inserted i
                            ON ba.StaffProfileId = i.StaffProfileId
                            AND ba.DayInWeek = ((DATEPART(WEEKDAY, i.StartDateTime) + @@DATEFIRST - 1) % 7 + 1)
                            AND CAST(i.StartDateTime AS time) >= ba.StartTime
                            AND CAST(i.EndDateTime AS time) <= ba.EndTime
                    )
                    BEGIN
                        THROW 50008, 'The meeting time does not fall within staff''s booking availability.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                END;
            ");

            // Revert trigger description
            migrationBuilder.Sql(@"
                EXEC sp_addextendedproperty 
                    @name = N'MS_Description', 
                    @value = N'Trigger to check student ban limit (10), leave schedule conflicts, and whether meeting time falls within booking availability (aligned with DayOfWeekAISEA enum) for BookedMeeting.', 
                    @level0type = N'SCHEMA', @level0name = N'dbo', 
                    @level1type = N'TABLE', @level1name = N'BookedMeeting', 
                    @level2type = N'TRIGGER', @level2name = N'TR_BookedMeeting_CheckExternalTables';
            ");
        }
    }
}
