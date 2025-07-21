using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddressBookManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemovePasswordColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "AppUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
