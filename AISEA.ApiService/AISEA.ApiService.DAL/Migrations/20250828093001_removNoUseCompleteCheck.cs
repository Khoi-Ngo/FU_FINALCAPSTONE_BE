using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class removNoUseCompleteCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "JoinedSubject");

            migrationBuilder.AddColumn<string>(
                name: "SubjectDescription",
                table: "JoinedSubject",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectDescription",
                table: "JoinedSubject");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "JoinedSubject",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
