using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddressBookManagement.Migrations
{
    /// <inheritdoc />
    public partial class NoteExtendCommon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Notes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Notes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeleteFlag",
                table: "Notes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordVersion",
                table: "Notes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Notes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Notes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "DeleteFlag",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "RecordVersion",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Notes");
        }
    }
}
