using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RefineCheckExternalTblForMeeting01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing trigger
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_BookedMeeting_CheckExternalTables') IS NOT NULL
                    DROP TRIGGER TR_BookedMeeting_CheckExternalTables;
            ");

            // Create trigger with normal DayInWeek and exact time match
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

                    -- Check for exact match of meeting time with booking availability
                    IF NOT EXISTS (
                        SELECT 1
                        FROM BookingAvailability ba
                        INNER JOIN inserted i
                            ON ba.StaffProfileId = i.StaffProfileId
                            AND ba.DayInWeek = DATEPART(WEEKDAY, i.StartDateTime)
                            AND CAST(i.StartDateTime AS time) = ba.StartTime
                            AND CAST(i.EndDateTime AS time) = ba.EndTime
                    )
                    BEGIN
                        THROW 50008, 'The meeting time does not exactly match staff''s booking availability.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                END;
            ");

            // Optional: Update trigger description
            migrationBuilder.Sql(@"
                EXEC sp_addextendedproperty 
                    @name = N'MS_Description', 
                    @value = N'Trigger checks student ban limit, staff leave conflict, and exact match with booking availability (day and time).', 
                    @level0type = N'SCHEMA', @level0name = N'dbo', 
                    @level1type = N'TABLE', @level1name = N'BookedMeeting', 
                    @level2type = N'TRIGGER', @level2name = N'TR_BookedMeeting_CheckExternalTables';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Just drop the trigger
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_BookedMeeting_CheckExternalTables') IS NOT NULL
                    DROP TRIGGER TR_BookedMeeting_CheckExternalTables;
            ");
        }
    }
}
