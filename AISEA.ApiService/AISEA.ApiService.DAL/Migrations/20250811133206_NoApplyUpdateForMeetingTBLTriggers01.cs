using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class NoApplyUpdateForMeetingTBLTriggers01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS TR_BookedMeeting_CheckExternalTables;
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_BookedMeeting_CheckExternalTables
                ON BookedMeeting
                AFTER INSERT
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

            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS TR_BookedMeeting_CheckInternalData;
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_BookedMeeting_CheckInternalData
                ON BookedMeeting
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    -- Check for overlapping active meetings for the same staff
                    IF EXISTS (
                        SELECT 1
                        FROM BookedMeeting bm
                        INNER JOIN inserted i
                            ON bm.StaffProfileId = i.StaffProfileId
                            AND bm.Id != i.Id
                            AND bm.Status IN (1, 2, 4, 5, 6) -- PENDING, CONFIRMED, COMPLETED, STUDENT_MISSED, ADVISOR_MISSED
                            AND bm.StartDateTime < i.EndDateTime
                            AND bm.EndDateTime > i.StartDateTime
                    )
                    BEGIN
                        THROW 50009, 'The staff already has an active meeting scheduled in the same time slot.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                    -- Check for overlapping active meetings for the same student
                    IF EXISTS (
                        SELECT 1
                        FROM BookedMeeting bm
                        INNER JOIN inserted i
                            ON bm.StudentProfileId = i.StudentProfileId
                            AND bm.Id != i.Id
                            AND bm.Status IN (1, 2, 4, 5, 6) -- PENDING, CONFIRMED, COMPLETED, STUDENT_MISSED, ADVISOR_MISSED
                            AND bm.StartDateTime < i.EndDateTime
                            AND bm.EndDateTime > i.StartDateTime
                    )
                    BEGIN
                        THROW 50010, 'The student already has an active meeting scheduled in the same time slot.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS TR_BookedMeeting_CheckExternalTables;
            ");

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

            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS TR_BookedMeeting_CheckInternalData;
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_BookedMeeting_CheckInternalData
                ON BookedMeeting
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    -- Check for overlapping active meetings for the same staff
                    IF EXISTS (
                        SELECT 1
                        FROM BookedMeeting bm
                        INNER JOIN inserted i
                            ON bm.StaffProfileId = i.StaffProfileId
                            AND bm.Id != i.Id
                            AND bm.Status IN (1, 2, 4, 5, 6) -- PENDING, CONFIRMED, COMPLETED, STUDENT_MISSED, ADVISOR_MISSED
                            AND bm.StartDateTime < i.EndDateTime
                            AND bm.EndDateTime > i.StartDateTime
                    )
                    BEGIN
                        THROW 50009, 'The staff already has an active meeting scheduled in the same time slot.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                    -- Check for overlapping active meetings for the same student
                    IF EXISTS (
                        SELECT 1
                        FROM BookedMeeting bm
                        INNER JOIN inserted i
                            ON bm.StudentProfileId = i.StudentProfileId
                            AND bm.Id != i.Id
                            AND bm.Status IN (1, 2, 4, 5, 6) -- PENDING, CONFIRMED, COMPLETED, STUDENT_MISSED, ADVISOR_MISSED
                            AND bm.StartDateTime < i.EndDateTime
                            AND bm.EndDateTime > i.StartDateTime
                    )
                    BEGIN
                        THROW 50010, 'The student already has an active meeting scheduled in the same time slot.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                END;
            ");
        }
    }
}