using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class removefieldSemNameJoinSub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SemesterName",
                table: "JoinedSubject");

            // Drop the old trigger if it exists
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS tr_JoinedSubject_BeforeInsert");

            // Create the new trigger
            migrationBuilder.Sql(@"
                CREATE TRIGGER tr_JoinedSubject_BeforeInsert
                ON JoinedSubject
                INSTEAD OF INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Temporary table to store processed rows
                    WITH DuplicateCounts AS (
                        SELECT 
                            i.Name,
                            i.SubjectName,
                            i.SubjectCode,
                            i.SubjectVersionCode,
                            i.CreatedByUserName,
                            i.StudentProfileId,
                            i.SemesterId,
                            i.GithubRepositoryURL,
                            i.Credits,
                            i.IsPassed,
                            i.IsCompleted,
                            i.IsActive,
                            i.CreatedAt,
                            -- Calculate duplicate count for each row based on Name and StudentProfileId
                            COUNT(*) OVER (
                                PARTITION BY i.StudentProfileId, i.Name
                                ORDER BY i.CreatedAt
                                ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                            ) - 1 AS DuplicateRank
                        FROM inserted i
                        LEFT JOIN JoinedSubject js
                            ON js.StudentProfileId = i.StudentProfileId
                            AND (
                                js.Name = i.Name
                                OR js.Name LIKE i.Name + ' ([0-9]%)'
                            )
                        GROUP BY 
                            i.Name,
                            i.SubjectName,
                            i.SubjectCode,
                            i.SubjectVersionCode,
                            i.CreatedByUserName,
                            i.StudentProfileId,
                            i.SemesterId,
                            i.GithubRepositoryURL,
                            i.Credits,
                            i.IsPassed,
                            i.IsCompleted,
                            i.IsActive,
                            i.CreatedAt
                    ),
                    FinalNames AS (
                        SELECT 
                            SubjectName,
                            SubjectCode,
                            SubjectVersionCode,
                            CreatedByUserName,
                            StudentProfileId,
                            SemesterId,
                            GithubRepositoryURL,
                            Credits,
                            IsPassed,
                            IsCompleted,
                            IsActive,
                            ISNULL(CreatedAt, GETDATE()) AS CreatedAt,
                            CASE 
                                WHEN DuplicateRank = 0 THEN Name
                                ELSE Name + ' (' + CAST(DuplicateRank AS nvarchar) + ')'
                            END AS FinalName
                        FROM DuplicateCounts
                    )
                    INSERT INTO JoinedSubject (
                        SubjectName,
                        SubjectCode,
                        SubjectVersionCode,
                        CreatedByUserName,
                        StudentProfileId,
                        SemesterId,
                        GithubRepositoryURL,
                        Credits,
                        IsPassed,
                        IsCompleted,
                        IsActive,
                        CreatedAt,
                        Name
                    )
                    SELECT 
                        SubjectName,
                        SubjectCode,
                        SubjectVersionCode,
                        CreatedByUserName,
                        StudentProfileId,
                        SemesterId,
                        GithubRepositoryURL,
                        Credits,
                        IsPassed,
                        IsCompleted,
                        IsActive,
                        CreatedAt,
                        FinalName
                    FROM FinalNames;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the new trigger
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS tr_JoinedSubject_BeforeInsert");

            migrationBuilder.AddColumn<string>(
                name: "SemesterName",
                table: "JoinedSubject",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // Recreate the old trigger
            migrationBuilder.Sql(@"
                CREATE TRIGGER tr_JoinedSubject_BeforeInsert
                ON JoinedSubject
                INSTEAD OF INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Temporary table to store processed rows
                    WITH DuplicateCounts AS (
                        SELECT 
                            i.SubjectName,
                            i.SemesterName,
                            i.SubjectCode,
                            i.SubjectVersionCode,
                            i.CreatedByUserName,
                            i.StudentProfileId,
                            i.SemesterId,
                            i.GithubRepositoryURL,
                            i.Credits,
                            i.IsPassed,
                            i.IsCompleted,
                            i.IsActive,
                            i.CreatedAt,
                            i.Name,
                            -- Calculate duplicate count for each row
                            COUNT(*) OVER (
                                PARTITION BY i.StudentProfileId, i.SemesterName, i.Name
                                ORDER BY i.CreatedAt
                                ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                            ) - 1 AS DuplicateRank
                        FROM inserted i
                        LEFT JOIN JoinedSubject js
                            ON js.StudentProfileId = i.StudentProfileId
                            AND js.SemesterName = i.SemesterName
                            AND (
                                js.Name = i.Name
                                OR js.Name LIKE i.Name + ' ([0-9]%)'
                            )
                        GROUP BY 
                            i.SubjectName,
                            i.SemesterName,
                            i.SubjectCode,
                            i.SubjectVersionCode,
                            i.CreatedByUserName,
                            i.StudentProfileId,
                            i.SemesterId,
                            i.GithubRepositoryURL,
                            i.Credits,
                            i.IsPassed,
                            i.IsCompleted,
                            i.IsActive,
                            i.CreatedAt,
                            i.Name
                    ),
                    FinalNames AS (
                        SELECT 
                            SubjectName,
                            SemesterName,
                            SubjectCode,
                            SubjectVersionCode,
                            CreatedByUserName,
                            StudentProfileId,
                            SemesterId,
                            GithubRepositoryURL,
                            Credits,
                            IsPassed,
                            IsCompleted,
                            IsActive,
                            ISNULL(CreatedAt, GETDATE()) AS CreatedAt,
                            CASE 
                                WHEN DuplicateRank = 0 THEN Name
                                ELSE Name + ' (' + CAST(DuplicateRank AS nvarchar) + ')'
                            END AS FinalName
                        FROM DuplicateCounts
                    )
                    INSERT INTO JoinedSubject (
                        SubjectName,
                        SemesterName,
                        SubjectCode,
                        SubjectVersionCode,
                        CreatedByUserName,
                        StudentProfileId,
                        SemesterId,
                        GithubRepositoryURL,
                        Credits,
                        IsPassed,
                        IsCompleted,
                        IsActive,
                        CreatedAt,
                        Name
                    )
                    SELECT 
                        SubjectName,
                        SemesterName,
                        SubjectCode,
                        SubjectVersionCode,
                        CreatedByUserName,
                        StudentProfileId,
                        SemesterId,
                        GithubRepositoryURL,
                        Credits,
                        IsPassed,
                        IsCompleted,
                        IsActive,
                        CreatedAt,
                        FinalName
                    FROM FinalNames;
                END
            ");
        }
    }
}