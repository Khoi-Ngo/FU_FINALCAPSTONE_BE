using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class internaltabletriggerforMeeting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing trigger if it exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_BookedMeeting_CheckInternalData') IS NOT NULL
                    DROP TRIGGER TR_BookedMeeting_CheckInternalData;
            ");

            // Create new trigger
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

            // Add trigger description
            migrationBuilder.Sql(@"
                EXEC sp_addextendedproperty 
                    @name = N'MS_Description', 
                    @value = N'Trigger to prevent overlapping active meetings for the same staff or student in BookedMeeting.', 
                    @level0type = N'SCHEMA', @level0name = N'dbo', 
                    @level1type = N'TABLE', @level1name = N'BookedMeeting', 
                    @level2type = N'TRIGGER', @level2name = N'TR_BookedMeeting_CheckInternalData';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop trigger if rolling back migration
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_BookedMeeting_CheckInternalData') IS NOT NULL
                    DROP TRIGGER TR_BookedMeeting_CheckInternalData;
            ");
        }
    }
}
