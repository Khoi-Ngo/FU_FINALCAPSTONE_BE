using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddNewAcademicTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Combo",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComboName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ComboDescription = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("combo_id_primary", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Program",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProgramCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("program_id_primary", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubjectName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subject_id_primary", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Curriculum",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramId = table.Column<long>(type: "bigint", nullable: false),
                    CurriculumCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CurriculumName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EffectiveDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("curriculum_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "curriculum_program_foreign",
                        column: x => x.ProgramId,
                        principalTable: "Program",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ComboSubject",
                columns: table => new
                {
                    combo_id = table.Column<long>(type: "bigint", nullable: false),
                    subject_id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("combosubject_composite_primary", x => new { x.combo_id, x.subject_id });
                    table.ForeignKey(
                        name: "combosubject_comboid_foreign",
                        column: x => x.combo_id,
                        principalTable: "Combo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "combosubject_subjectid_foreign",
                        column: x => x.subject_id,
                        principalTable: "Subject",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SubjectPrerequisite",
                columns: table => new
                {
                    subject_id = table.Column<long>(type: "bigint", nullable: false),
                    prerequisite_subject_id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subjectprerequisite_composite_primary", x => new { x.subject_id, x.prerequisite_subject_id });
                    table.ForeignKey(
                        name: "subjectprerequisite_prerequisitesubjectid_foreign",
                        column: x => x.prerequisite_subject_id,
                        principalTable: "Subject",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "subjectprerequisite_subjectid_foreign",
                        column: x => x.subject_id,
                        principalTable: "Subject",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Syllabus",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("syllabus_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "syllabus_subjectid_foreign",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "CurriculumSubject",
                columns: table => new
                {
                    curriculum_id = table.Column<long>(type: "bigint", nullable: false),
                    subject_id = table.Column<long>(type: "bigint", nullable: false),
                    SemesterNumber = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("curriculumsubject_composite_primary", x => new { x.curriculum_id, x.subject_id });
                    table.ForeignKey(
                        name: "curriculumsubject_curriculumid_foreign",
                        column: x => x.curriculum_id,
                        principalTable: "Curriculum",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "curriculumsubject_subjectid_foreign",
                        column: x => x.subject_id,
                        principalTable: "Subject",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SyllabusAssessment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SyllabusId = table.Column<long>(type: "bigint", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CompletionCriteria = table.Column<string>(type: "text", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    QuestionType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("syllabusassessment_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "syllabusassessment_syllabusid_foreign",
                        column: x => x.SyllabusId,
                        principalTable: "Syllabus",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SyllabusLearningMaterial",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SyllabusId = table.Column<long>(type: "bigint", nullable: false),
                    MaterialName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PublishedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FilepathOrUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("syllabuslearningmaterial_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "syllabuslearningmaterial_syllabusid_foreign",
                        column: x => x.SyllabusId,
                        principalTable: "Syllabus",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SyllabusLearningOutcome",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SyllabusId = table.Column<long>(type: "bigint", nullable: false),
                    OutcomeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("syllabuslearningoutcome_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "syllabuslearningoutcome_syllabusid_foreign",
                        column: x => x.SyllabusId,
                        principalTable: "Syllabus",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SyllabusSession",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SyllabusId = table.Column<long>(type: "bigint", nullable: false),
                    SessionNumber = table.Column<int>(type: "int", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Mission = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("syllabussession_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "syllabussession_syllabusid_foreign",
                        column: x => x.SyllabusId,
                        principalTable: "Syllabus",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SessionOutcomeMapping",
                columns: table => new
                {
                    session_id = table.Column<long>(type: "bigint", nullable: false),
                    outcome_id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sessionoutcomemapping_composite_primary", x => new { x.session_id, x.outcome_id });
                    table.ForeignKey(
                        name: "sessionoutcomemapping_outcomeid_foreign",
                        column: x => x.outcome_id,
                        principalTable: "SyllabusLearningOutcome",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "sessionoutcomemapping_sessionid_foreign",
                        column: x => x.session_id,
                        principalTable: "SyllabusSession",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComboSubject_subject_id",
                table: "ComboSubject",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "curriculum_code_unique",
                table: "Curriculum",
                column: "CurriculumCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Curriculum_ProgramId",
                table: "Curriculum",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumSubject_subject_id",
                table: "CurriculumSubject",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_SessionOutcomeMapping_outcome_id",
                table: "SessionOutcomeMapping",
                column: "outcome_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectPrerequisite_prerequisite_subject_id",
                table: "SubjectPrerequisite",
                column: "prerequisite_subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_Syllabus_SubjectId",
                table: "Syllabus",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SyllabusAssessment_SyllabusId",
                table: "SyllabusAssessment",
                column: "SyllabusId");

            migrationBuilder.CreateIndex(
                name: "IX_SyllabusLearningMaterial_SyllabusId",
                table: "SyllabusLearningMaterial",
                column: "SyllabusId");

            migrationBuilder.CreateIndex(
                name: "IX_SyllabusLearningOutcome_SyllabusId",
                table: "SyllabusLearningOutcome",
                column: "SyllabusId");

            migrationBuilder.CreateIndex(
                name: "IX_SyllabusSession_SyllabusId",
                table: "SyllabusSession",
                column: "SyllabusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComboSubject");

            migrationBuilder.DropTable(
                name: "CurriculumSubject");

            migrationBuilder.DropTable(
                name: "SessionOutcomeMapping");

            migrationBuilder.DropTable(
                name: "SubjectPrerequisite");

            migrationBuilder.DropTable(
                name: "SyllabusAssessment");

            migrationBuilder.DropTable(
                name: "SyllabusLearningMaterial");

            migrationBuilder.DropTable(
                name: "Combo");

            migrationBuilder.DropTable(
                name: "Curriculum");

            migrationBuilder.DropTable(
                name: "SyllabusLearningOutcome");

            migrationBuilder.DropTable(
                name: "SyllabusSession");

            migrationBuilder.DropTable(
                name: "Program");

            migrationBuilder.DropTable(
                name: "Syllabus");

            migrationBuilder.DropTable(
                name: "Subject");
        }
    }
}
