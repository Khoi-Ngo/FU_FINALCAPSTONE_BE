using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectCommentSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectComment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<long>(type: "bigint", nullable: false),
                    StudentProfileId = table.Column<long>(type: "bigint", nullable: false),
                    JoinedSubjectId = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IsAnonymous = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ModerationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subjectcomment_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "subjectcomment_joinedsubjectid_foreign",
                        column: x => x.JoinedSubjectId,
                        principalTable: "JoinedSubject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "subjectcomment_studentprofileid_foreign",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "subjectcomment_subjectid_foreign",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubjectCommentReaction",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentId = table.Column<long>(type: "bigint", nullable: false),
                    StudentProfileId = table.Column<long>(type: "bigint", nullable: false),
                    ReactionType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subjectcommentreaction_id_primary", x => x.id);
                    table.ForeignKey(
                        name: "subjectcommentreaction_commentid_foreign",
                        column: x => x.CommentId,
                        principalTable: "SubjectComment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "subjectcommentreaction_studentprofileid_foreign",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectComment_IsApproved",
                table: "SubjectComment",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectComment_JoinedSubjectId",
                table: "SubjectComment",
                column: "JoinedSubjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectComment_Student_Subject_Unique",
                table: "SubjectComment",
                columns: new[] { "StudentProfileId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectComment_StudentProfileId",
                table: "SubjectComment",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectComment_SubjectId",
                table: "SubjectComment",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCommentReaction_CommentId",
                table: "SubjectCommentReaction",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCommentReaction_ReactionType",
                table: "SubjectCommentReaction",
                column: "ReactionType");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCommentReaction_Student_Comment_Unique",
                table: "SubjectCommentReaction",
                columns: new[] { "StudentProfileId", "CommentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCommentReaction_StudentProfileId",
                table: "SubjectCommentReaction",
                column: "StudentProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectCommentReaction");

            migrationBuilder.DropTable(
                name: "SubjectComment");
        }
    }
}
