using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    public partial class removNameofJoinSubTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the trigger if it exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('tr_JoinedSubject_BeforeInsert', 'TR') IS NOT NULL
                DROP TRIGGER tr_JoinedSubject_BeforeInsert;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Optionally recreate the trigger if needed
            // Example: If you don't need it back, just leave this empty
            migrationBuilder.Sql(@"
                CREATE TRIGGER tr_JoinedSubject_BeforeInsert
                ON JoinedSubject
                INSTEAD OF INSERT
                AS
                BEGIN
                    -- Your original trigger body here if you want to restore it
                END
            ");
        }
    }
}
