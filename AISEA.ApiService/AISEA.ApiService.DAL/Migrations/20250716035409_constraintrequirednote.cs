using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    public partial class constraintrequirednote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ----------------------------------------
            // 1. Drop existing trigger to avoid error
            // ----------------------------------------
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_LeaveSchedule_CheckConstraints') IS NOT NULL
                    DROP TRIGGER TR_LeaveSchedule_CheckConstraints;
            ");

            // ----------------------------------------
            // 2. Update existing null Note values to ''
            // ----------------------------------------
            migrationBuilder.Sql("UPDATE LeaveSchedule SET Note = '' WHERE Note IS NULL;");

            // ----------------------------------------
            // 3. Alter Note column to be non-nullable with default ''
            // ----------------------------------------
            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "LeaveSchedule",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // ----------------------------------------
            // 4. Recreate trigger with early exit logic
            // ----------------------------------------
            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_LeaveSchedule_CheckConstraints
                ON LeaveSchedule
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Exit trigger early if UPDATE only changes Note (skip constraint checks)
                    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
                    BEGIN
                        IF NOT EXISTS (
                            SELECT 1
                            FROM inserted i
                            JOIN deleted d ON i.Id = d.Id
                            WHERE ISNULL(i.StaffProfileId, '') != ISNULL(d.StaffProfileId, '')
                               OR ISNULL(i.StartDateTime, '') != ISNULL(d.StartDateTime, '')
                               OR ISNULL(i.EndDateTime, '') != ISNULL(d.EndDateTime, '')
                        )
                            RETURN;
                    END

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
                            AND bm.Status IN (1, 2)
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

            // ----------------------------------------
            // 5. Add description for the trigger
            // ----------------------------------------
            migrationBuilder.Sql(@"
                EXEC sp_addextendedproperty 
                    @name = N'MS_Description', 
                    @value = N'Trigger to check overlaps, booking availability, and booked meetings (with early exit if only Note updated)', 
                    @level0type = N'SCHEMA', @level0name = N'dbo', 
                    @level1type = N'TABLE', @level1name = N'LeaveSchedule', 
                    @level2type = N'TRIGGER', @level2name = N'TR_LeaveSchedule_CheckConstraints';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert Note column back to nullable without default
            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "LeaveSchedule",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Drop trigger
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_LeaveSchedule_CheckConstraints') IS NOT NULL
                    DROP TRIGGER TR_LeaveSchedule_CheckConstraints;
            ");
        }
    }
}
