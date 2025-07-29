using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class antispamstucancelmeeting01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop trigger if exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_BookedMeeting_AntiStudentSpamCancel') IS NOT NULL
                    DROP TRIGGER TR_BookedMeeting_AntiStudentSpamCancel;
            ");

            // Create trigger
            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_BookedMeeting_AntiStudentSpamCancel
                ON BookedMeeting
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    /*
                        Purpose:
                        - Prevent student from canceling a meeting if they already canceled 3 or more meetings
                          in the past 15 days (status = 9 => STU_CANCELED)
                    */

                    IF EXISTS (
                        SELECT 1
                        FROM inserted i
                        INNER JOIN deleted d ON i.Id = d.Id
                        WHERE i.Status = 9 -- STU_CANCELED
                          AND d.Status != 9 -- Only trigger when status is changing to STU_CANCELED
                          AND (
                                SELECT COUNT(1)
                                FROM BookedMeeting bm
                                WHERE bm.StudentProfileId = i.StudentProfileId
                                  AND bm.Status = 9 -- STU_CANCELED
                                  AND bm.CreatedAt >= DATEADD(DAY, -15, GETUTCDATE())
                              ) >= 3
                    )
                    BEGIN
                        THROW 50010, 'Student cannot cancel more than 3 meetings within the past 15 days.', 1;
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
                END;
            ");

            // Optional: Add description
            migrationBuilder.Sql(@"
                EXEC sp_addextendedproperty 
                    @name = N'MS_Description', 
                    @value = N'Prevents student from canceling more than 3 meetings within the last 15 days.', 
                    @level0type = N'SCHEMA', @level0name = N'dbo', 
                    @level1type = N'TABLE', @level1name = N'BookedMeeting', 
                    @level2type = N'TRIGGER', @level2name = N'TR_BookedMeeting_AntiStudentSpamCancel';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop trigger on rollback
            migrationBuilder.Sql(@"
                IF OBJECT_ID('TR_BookedMeeting_AntiStudentSpamCancel') IS NOT NULL
                    DROP TRIGGER TR_BookedMeeting_AntiStudentSpamCancel;
            ");
        }
    }
}
