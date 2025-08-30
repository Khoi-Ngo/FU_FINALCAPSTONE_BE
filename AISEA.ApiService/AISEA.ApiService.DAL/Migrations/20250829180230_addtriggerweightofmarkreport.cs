using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    public partial class addtriggerweightofmarkreport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        CREATE TRIGGER trg_CheckWeightSum_InsertUpdate
        ON SubjectMarkReport
        AFTER INSERT, UPDATE
        AS
        BEGIN
            SET NOCOUNT ON;

            IF EXISTS (
                SELECT 1
                FROM SubjectMarkReport smr
                JOIN inserted i ON smr.JoinedSubjectId = i.JoinedSubjectId
                GROUP BY smr.JoinedSubjectId
                HAVING SUM(smr.Weight) < 0 OR SUM(smr.Weight) > 100
            )
            BEGIN
                -- Use THROW instead of RAISERROR with a custom error number
                THROW 51021, 'The total weight for a subject must be between 0 and 100.', 1;
            END
        END
    ");
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop Trigger
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_CheckWeightSum_InsertUpdate;");
        }
    }
}
