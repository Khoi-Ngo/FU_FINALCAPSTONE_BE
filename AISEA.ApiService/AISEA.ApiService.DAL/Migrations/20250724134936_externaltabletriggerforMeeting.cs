using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class externaltabletriggerforMeeting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing trigger if it exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_BookedMeeting_CheckExternalTables') IS NOT NULL
                    DROP TRIGGER TR_BookedMeeting_CheckExternalTables;
            ");

            // Create new trigger
            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_BookedMeeting_CheckExternalTables
                ON BookedMeeting
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Check if student has reached ban limit (30)
                    IF EXISTS (
                        SELECT 1
                        FROM StudentProfile sp
                        INNER JOIN inserted i
                            ON sp.Id = i.StudentProfileId
                        WHERE sp.NumberOfBan >= 30
                    )
                    BEGIN
                        THROW 50006, 'Student has reached the maximum number of bans (30). Cannot book meeting.', 1;
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

                    -- Check for exact matching booking availability
                    IF NOT EXISTS (
                        SELECT 1
                        FROM BookingAvailability ba
                        INNER JOIN inserted i
                            ON ba.StaffProfileId = i.StaffProfileId
                            AND ba.DayInWeek = DATEPART(WEEKDAY, i.StartDateTime)
                            AND ba.StartTime = CAST(i.StartDateTime AS time)
                            AND ba.EndTime = CAST(i.EndDateTime AS time)
                    )
                    BEGIN
                        THROW 50008, 'The meeting time does not exactly match staff''s booking availability.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                END;
            ");

            // Add trigger description
            migrationBuilder.Sql(@"
                EXEC sp_addextendedproperty 
                    @name = N'MS_Description', 
                    @value = N'Trigger to check student ban limit, leave schedule conflicts, and exact booking availability match for BookedMeeting.', 
                    @level0type = N'SCHEMA', @level0name = N'dbo', 
                    @level1type = N'TABLE', @level1name = N'BookedMeeting', 
                    @level2type = N'TRIGGER', @level2name = N'TR_BookedMeeting_CheckExternalTables';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop trigger if rolling back migration
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_BookedMeeting_CheckExternalTables') IS NOT NULL
                    DROP TRIGGER TR_BookedMeeting_CheckExternalTables;
            ");
        }
    }
}
