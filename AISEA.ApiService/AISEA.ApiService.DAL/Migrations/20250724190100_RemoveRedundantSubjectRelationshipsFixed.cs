using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantSubjectRelationshipsFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create default SubjectVersion records for existing data with NULL SubjectVersionId
            // First, create default versions for subjects that don't have any versions yet
            migrationBuilder.Sql(@"
                INSERT INTO [SubjectVersion] ([SubjectId], [VersionCode], [VersionName], [Description], [IsActive], [IsDefault], [EffectiveFrom], [CreatedAt], [IsDeleted])
                SELECT DISTINCT s.[id], '1.0', 'Default Version', 'Default version created during migration', 1, 1, GETUTCDATE(), GETUTCDATE(), 0
                FROM [Subject] s
                WHERE NOT EXISTS (SELECT 1 FROM [SubjectVersion] sv WHERE sv.[SubjectId] = s.[id])
            ");

            // Step 2: Update Syllabus records with NULL SubjectVersionId to use the default version
            migrationBuilder.Sql(@"
                UPDATE s SET s.[SubjectVersionId] = sv.[id]
                FROM [Syllabus] s
                INNER JOIN [SubjectVersion] sv ON sv.[SubjectId] = s.[SubjectId] AND sv.[IsDefault] = 1
                WHERE s.[SubjectVersionId] IS NULL
            ");

            // Step 3: Update SubjectClass records with NULL SubjectVersionId to use the default version
            migrationBuilder.Sql(@"
                UPDATE sc SET sc.[SubjectVersionId] = sv.[id]
                FROM [SubjectClass] sc
                INNER JOIN [SubjectVersion] sv ON sv.[SubjectId] = sc.[SubjectId] AND sv.[IsDefault] = 1
                WHERE sc.[SubjectVersionId] IS NULL
            ");

            // Step 4: Update CurriculumSubject records with NULL SubjectVersionId to use the default version
            migrationBuilder.Sql(@"
                UPDATE cs SET cs.[SubjectVersionId] = sv.[id]
                FROM [CurriculumSubject] cs
                INNER JOIN [SubjectVersion] sv ON sv.[SubjectId] = cs.[subject_id] AND sv.[IsDefault] = 1
                WHERE cs.[SubjectVersionId] IS NULL
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_CurriculumSubject_SubjectVersion_SubjectVersionId",
                table: "CurriculumSubject");

            migrationBuilder.DropForeignKey(
                name: "curriculumsubject_subjectid_foreign",
                table: "CurriculumSubject");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectClass_SubjectVersion_SubjectVersionId",
                table: "SubjectClass");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectClass_Subject_SubjectId",
                table: "SubjectClass");

            migrationBuilder.DropForeignKey(
                name: "FK_Syllabus_SubjectVersion_SubjectVersionId",
                table: "Syllabus");

            migrationBuilder.DropForeignKey(
                name: "syllabus_subjectid_foreign",
                table: "Syllabus");

            migrationBuilder.DropIndex(
                name: "IX_Syllabus_SubjectId",
                table: "Syllabus");

            migrationBuilder.DropIndex(
                name: "IX_SubjectClass_SubjectVersionId",
                table: "SubjectClass");

            migrationBuilder.DropIndex(
                name: "IX_SubjectClass_UniqueClass",
                table: "SubjectClass");

            migrationBuilder.DropPrimaryKey(
                name: "curriculumsubject_composite_primary",
                table: "CurriculumSubject");

            migrationBuilder.DropIndex(
                name: "IX_CurriculumSubject_SubjectId",
                table: "CurriculumSubject");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "SubjectClass");

            migrationBuilder.DropColumn(
                name: "subject_id",
                table: "CurriculumSubject");

            migrationBuilder.RenameColumn(
                name: "SubjectVersionId",
                table: "CurriculumSubject",
                newName: "subject_version_id");

            migrationBuilder.AlterColumn<long>(
                name: "SubjectVersionId",
                table: "Syllabus",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "SubjectVersionId",
                table: "SubjectClass",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "subject_version_id",
                table: "CurriculumSubject",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "curriculumsubject_composite_primary",
                table: "CurriculumSubject",
                columns: new[] { "curriculum_id", "subject_version_id" });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectClass_UniqueClass",
                table: "SubjectClass",
                columns: new[] { "SubjectVersionId", "SemesterNumber", "ClassCode" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "curriculumsubject_subjectversionid_foreign",
                table: "CurriculumSubject",
                column: "subject_version_id",
                principalTable: "SubjectVersion",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectClass_SubjectVersion_SubjectVersionId",
                table: "SubjectClass",
                column: "SubjectVersionId",
                principalTable: "SubjectVersion",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "syllabus_subjectversionid_foreign",
                table: "Syllabus",
                column: "SubjectVersionId",
                principalTable: "SubjectVersion",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "curriculumsubject_subjectversionid_foreign",
                table: "CurriculumSubject");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectClass_SubjectVersion_SubjectVersionId",
                table: "SubjectClass");

            migrationBuilder.DropForeignKey(
                name: "syllabus_subjectversionid_foreign",
                table: "Syllabus");

            migrationBuilder.DropIndex(
                name: "IX_SubjectClass_UniqueClass",
                table: "SubjectClass");

            migrationBuilder.DropPrimaryKey(
                name: "curriculumsubject_composite_primary",
                table: "CurriculumSubject");

            migrationBuilder.RenameColumn(
                name: "subject_version_id",
                table: "CurriculumSubject",
                newName: "SubjectVersionId");

            migrationBuilder.AlterColumn<long>(
                name: "SubjectVersionId",
                table: "Syllabus",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "SubjectId",
                table: "Syllabus",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "SubjectVersionId",
                table: "SubjectClass",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "SubjectId",
                table: "SubjectClass",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "SubjectVersionId",
                table: "CurriculumSubject",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "subject_id",
                table: "CurriculumSubject",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "curriculumsubject_composite_primary",
                table: "CurriculumSubject",
                columns: new[] { "curriculum_id", "subject_id" });

            migrationBuilder.CreateIndex(
                name: "IX_Syllabus_SubjectId",
                table: "Syllabus",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectClass_SubjectVersionId",
                table: "SubjectClass",
                column: "SubjectVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectClass_UniqueClass",
                table: "SubjectClass",
                columns: new[] { "SubjectId", "SemesterNumber", "ClassCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumSubject_SubjectId",
                table: "CurriculumSubject",
                column: "subject_id");

            migrationBuilder.AddForeignKey(
                name: "FK_CurriculumSubject_SubjectVersion_SubjectVersionId",
                table: "CurriculumSubject",
                column: "SubjectVersionId",
                principalTable: "SubjectVersion",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "curriculumsubject_subjectid_foreign",
                table: "CurriculumSubject",
                column: "subject_id",
                principalTable: "Subject",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectClass_SubjectVersion_SubjectVersionId",
                table: "SubjectClass",
                column: "SubjectVersionId",
                principalTable: "SubjectVersion",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectClass_Subject_SubjectId",
                table: "SubjectClass",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabus_SubjectVersion_SubjectVersionId",
                table: "Syllabus",
                column: "SubjectVersionId",
                principalTable: "SubjectVersion",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "syllabus_subjectid_foreign",
                table: "Syllabus",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "id");
        }
    }
}
