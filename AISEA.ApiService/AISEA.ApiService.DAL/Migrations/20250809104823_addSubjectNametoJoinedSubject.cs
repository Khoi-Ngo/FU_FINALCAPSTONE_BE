using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addSubjectNametoJoinedSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubjectName",
                table: "JoinedSubject",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // Creating optimized set-based trigger for handling duplicate Name per StudentProfileId and SemesterName
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
                                PARTITION BY i.StudentProfileId, i.SemesterName, i.SubjectName
                                ORDER BY i.CreatedAt
                                ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                            ) - 1 AS DuplicateRank
                        FROM inserted i
                        LEFT JOIN JoinedSubject js
                            ON js.StudentProfileId = i.StudentProfileId
                            AND js.SemesterName = i.SemesterName
                            AND (
                                js.Name = i.SubjectName
                                OR js.Name LIKE i.SubjectName + ' ([0-9]%)'
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
                                WHEN DuplicateRank = 0 THEN SubjectName
                                ELSE SubjectName + ' (' + CAST(DuplicateRank AS nvarchar) + ')'
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS tr_JoinedSubject_BeforeInsert");
            
            migrationBuilder.DropColumn(
                name: "SubjectName",
                table: "JoinedSubject");
        }
    }
}