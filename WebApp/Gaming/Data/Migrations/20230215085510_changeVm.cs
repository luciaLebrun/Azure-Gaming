using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gaming.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeVm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomVms",
                table: "CustomVms");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CustomVms");

            migrationBuilder.DropColumn(
                name: "Ip",
                table: "CustomVms");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomVms",
                table: "CustomVms",
                column: "Login");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomVms",
                table: "CustomVms");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CustomVms",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ip",
                table: "CustomVms",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomVms",
                table: "CustomVms",
                column: "Name");
        }
    }
}
