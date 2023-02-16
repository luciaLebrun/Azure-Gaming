using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gaming.Data.Migrations
{
    /// <inheritdoc />
    public partial class isStartedVm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ip",
                table: "CustomVms",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsStarted",
                table: "CustomVms",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CustomVms",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ip",
                table: "CustomVms");

            migrationBuilder.DropColumn(
                name: "IsStarted",
                table: "CustomVms");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CustomVms");
        }
    }
}
