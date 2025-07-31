using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEA.ApiService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByFieldToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Syllabus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Syllabus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Syllabus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Syllabus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Syllabus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Subject",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Subject",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Curriculum",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Curriculum",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Curriculum",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Curriculum",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Curriculum",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Combo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Combo",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Combo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Combo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Combo",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Curriculum");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Curriculum");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Curriculum");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Curriculum");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Curriculum");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Combo");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Combo");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Combo");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Combo");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Combo");
        }
    }
}
