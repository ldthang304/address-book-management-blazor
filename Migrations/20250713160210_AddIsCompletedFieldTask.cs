using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddressBookManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCompletedFieldTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "TodoTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "TodoTasks");
        }
    }
}
