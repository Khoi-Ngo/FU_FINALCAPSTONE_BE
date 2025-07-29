using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class limitNoOfPendingForStudentMeeting01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing trigger if it exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_BookedMeeting_PreventStudentTooManyPending') IS NOT NULL
                    DROP TRIGGER TR_BookedMeeting_PreventStudentTooManyPending;
            ");

            // Create trigger to prevent student from having too many pending meetings
            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_BookedMeeting_PreventStudentTooManyPending
                ON BookedMeeting
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    /*
                      Logic:
                      - For each inserted/updated row, count PENDING meetings (status = 1)
                      - If student has >= 3 other PENDING meetings, throw error
                    */

                    IF EXISTS (
                        SELECT 1
                        FROM inserted i
                        WHERE i.Status = 1
                          AND (
                                SELECT COUNT(1)
                                FROM BookedMeeting bm
                                WHERE bm.StudentProfileId = i.StudentProfileId
                                  AND bm.Status = 1 -- PENDING
                                  AND bm.Id NOT IN (SELECT Id FROM deleted) -- Exclude old values during UPDATE
                            ) >= 3
                    )
                    BEGIN
                        THROW 50009, 'Student cannot have more than 3 pending meetings.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                END;
            ");

            // Optional: add description
            migrationBuilder.Sql(@"
                EXEC sp_addextendedproperty 
                    @name = N'MS_Description', 
                    @value = N'Prevents students from having more than 3 PENDING meetings.', 
                    @level0type = N'SCHEMA', @level0name = N'dbo', 
                    @level1type = N'TABLE', @level1name = N'BookedMeeting', 
                    @level2type = N'TRIGGER', @level2name = N'TR_BookedMeeting_PreventStudentTooManyPending';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_BookedMeeting_PreventStudentTooManyPending') IS NOT NULL
                    DROP TRIGGER TR_BookedMeeting_PreventStudentTooManyPending;
            ");
        }
    }
}
