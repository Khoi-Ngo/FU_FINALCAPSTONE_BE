using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class minorchange01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "advisorysession1to1_staffid_foreign",
                table: "AdvisorySession1to1");

            migrationBuilder.DropForeignKey(
                name: "advisorysession1to1_studentid_foreign",
                table: "AdvisorySession1to1");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StudentProfile");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "StudentProfile");

            migrationBuilder.AddForeignKey(
                name: "advisorysession1to1_staffid_foreign",
                table: "AdvisorySession1to1",
                column: "StaffId",
                principalTable: "StaffProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "advisorysession1to1_studentid_foreign",
                table: "AdvisorySession1to1",
                column: "StudentId",
                principalTable: "StudentProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "advisorysession1to1_staffid_foreign",
                table: "AdvisorySession1to1");

            migrationBuilder.DropForeignKey(
                name: "advisorysession1to1_studentid_foreign",
                table: "AdvisorySession1to1");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StudentProfile",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "StudentProfile",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "advisorysession1to1_staffid_foreign",
                table: "AdvisorySession1to1",
                column: "StaffId",
                principalTable: "StaffProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "advisorysession1to1_studentid_foreign",
                table: "AdvisorySession1to1",
                column: "StudentId",
                principalTable: "StudentProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
