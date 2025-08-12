using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addsomefiledaudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RoleId",
                table: "AuditLog",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "AuditLog",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "AuditLog");
        }
    }
}
