using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smoehring.Home.Data.SqlDatabase.Migrations
{
    /// <inheritdoc />
    public partial class ChangePurchaseDatetimeoffsetToDatetime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtworkArtworkSet");

            migrationBuilder.DropTable(
                name: "ArtworkSets");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseTime",
                table: "Purchases",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "(GetDate())",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "(GetDate())");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "PurchaseTime",
                table: "Purchases",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "(GetDate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "(GetDate())");

            migrationBuilder.CreateTable(
                name: "ArtworkSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtworkSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArtworkArtworkSet",
                columns: table => new
                {
                    ArtworkSetsId = table.Column<int>(type: "int", nullable: false),
                    ArtworksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtworkArtworkSet", x => new { x.ArtworkSetsId, x.ArtworksId });
                    table.ForeignKey(
                        name: "FK_ArtworkArtworkSet_ArtworkSets_ArtworkSetsId",
                        column: x => x.ArtworkSetsId,
                        principalTable: "ArtworkSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtworkArtworkSet_Artworks_ArtworksId",
                        column: x => x.ArtworksId,
                        principalTable: "Artworks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtworkArtworkSet_ArtworksId",
                table: "ArtworkArtworkSet",
                column: "ArtworksId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtworkSets_Name",
                table: "ArtworkSets",
                column: "Name");
        }
    }
}
