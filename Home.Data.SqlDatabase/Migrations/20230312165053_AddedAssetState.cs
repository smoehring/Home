using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Smoehring.Home.Data.SqlDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddedAssetState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetStateId",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "AssetStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Possession = table.Column<bool>(type: "bit", nullable: false),
                    Ownership = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetStates", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AssetStates",
                columns: new[] { "Id", "IsDefault", "Name", "Ownership", "Possession" },
                values: new object[,]
                {
                    { 1, true, "Available", true, true },
                    { 2, false, "Lost", false, false },
                    { 3, false, "Stolen", false, false },
                    { 4, false, "Broken", false, false },
                    { 5, false, "Loaned", true, false },
                    { 6, false, "Borrowed", false, true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetStateId",
                table: "Assets",
                column: "AssetStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetStates_AssetStateId",
                table: "Assets",
                column: "AssetStateId",
                principalTable: "AssetStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetStates_AssetStateId",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "AssetStates");

            migrationBuilder.DropIndex(
                name: "IX_Assets_AssetStateId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetStateId",
                table: "Assets");
        }
    }
}
