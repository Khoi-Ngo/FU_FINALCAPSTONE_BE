using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    public partial class TempRemoveConstraintBadReqForLeavewithoutMathcBkAvai : Migration
    {


        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing trigger
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_LeaveSchedule_CheckConstraints') IS NOT NULL
                    DROP TRIGGER TR_LeaveSchedule_CheckConstraints;
            ");

            // Recreate the trigger WITHOUT the BookingAvailability check
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

            // Add description for the trigger
            migrationBuilder.Sql(@"
                EXEC sp_addextendedproperty 
                    @name = N'MS_Description', 
                    @value = N'Trigger to check overlapping leave schedules and booked meetings (BookingAvailability check removed)', 
                    @level0type = N'SCHEMA', @level0name = N'dbo', 
                    @level1type = N'TABLE', @level1name = N'LeaveSchedule', 
                    @level2type = N'TRIGGER', @level2name = N'TR_LeaveSchedule_CheckConstraints';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the trigger (so it can be restored if needed)
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_LeaveSchedule_CheckConstraints') IS NOT NULL
                    DROP TRIGGER TR_LeaveSchedule_CheckConstraints;
            ");

        }
    }
}
