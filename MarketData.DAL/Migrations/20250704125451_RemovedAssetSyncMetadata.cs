using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketData.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemovedAssetSyncMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetSyncMetadata");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetSyncMetadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSyncMetadata", x => x.Id);
                });
        }
    }
}
