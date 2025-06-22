using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class cascadingmsg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "message_advisorysession1to1id_foreign",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "message_senderid_foreign",
                table: "Message");

            migrationBuilder.AddForeignKey(
                name: "message_advisorysession1to1id_foreign",
                table: "Message",
                column: "AdvisorySession1to1Id",
                principalTable: "AdvisorySession1to1",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "message_senderid_foreign",
                table: "Message",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "message_advisorysession1to1id_foreign",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "message_senderid_foreign",
                table: "Message");

            migrationBuilder.AddForeignKey(
                name: "message_advisorysession1to1id_foreign",
                table: "Message",
                column: "AdvisorySession1to1Id",
                principalTable: "AdvisorySession1to1",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "message_senderid_foreign",
                table: "Message",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
