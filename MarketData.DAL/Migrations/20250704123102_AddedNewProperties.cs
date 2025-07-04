using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketData.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Assets",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Exchange",
                table: "Assets",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Exchange",
                table: "Assets");
        }
    }
}
