using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNousefieldMeeting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmCheckinCode",
                table: "BookedMeeting");

            migrationBuilder.RenameColumn(
                name: "CheckinCode",
                table: "BookedMeeting",
                newName: "CheckInCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckInCode",
                table: "BookedMeeting",
                newName: "CheckinCode");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmCheckinCode",
                table: "BookedMeeting",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
