using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smoehring.Home.Data.SqlDatabase.Migrations
{
    /// <inheritdoc />
    public partial class RenamingCharactersTableToSingular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistName_Artists_ArtworkArtistId",
                table: "ArtistName");

            migrationBuilder.DropTable(
                name: "ArtworkArtworkCharacters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArtistName",
                table: "ArtistName");

            migrationBuilder.DropIndex(
                name: "IX_ArtistName_Name",
                table: "ArtistName");

            migrationBuilder.RenameTable(
                name: "ArtistName",
                newName: "ArtistNames");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistName_ArtworkArtistId",
                table: "ArtistNames",
                newName: "IX_ArtistNames_ArtworkArtistId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArtistNames",
                table: "ArtistNames",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ArtworkArtworkCharacter",
                columns: table => new
                {
                    ArtworksId = table.Column<int>(type: "int", nullable: false),
                    CharactersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtworkArtworkCharacter", x => new { x.ArtworksId, x.CharactersId });
                    table.ForeignKey(
                        name: "FK_ArtworkArtworkCharacter_Artworks_ArtworksId",
                        column: x => x.ArtworksId,
                        principalTable: "Artworks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtworkArtworkCharacter_Characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistNames_Name",
                table: "ArtistNames",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArtworkArtworkCharacter_CharactersId",
                table: "ArtworkArtworkCharacter",
                column: "CharactersId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistNames_Artists_ArtworkArtistId",
                table: "ArtistNames",
                column: "ArtworkArtistId",
                principalTable: "Artists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistNames_Artists_ArtworkArtistId",
                table: "ArtistNames");

            migrationBuilder.DropTable(
                name: "ArtworkArtworkCharacter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArtistNames",
                table: "ArtistNames");

            migrationBuilder.DropIndex(
                name: "IX_ArtistNames_Name",
                table: "ArtistNames");

            migrationBuilder.RenameTable(
                name: "ArtistNames",
                newName: "ArtistName");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistNames_ArtworkArtistId",
                table: "ArtistName",
                newName: "IX_ArtistName_ArtworkArtistId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArtistName",
                table: "ArtistName",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ArtworkArtworkCharacters",
                columns: table => new
                {
                    ArtworksId = table.Column<int>(type: "int", nullable: false),
                    CharactersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtworkArtworkCharacters", x => new { x.ArtworksId, x.CharactersId });
                    table.ForeignKey(
                        name: "FK_ArtworkArtworkCharacters_Artworks_ArtworksId",
                        column: x => x.ArtworksId,
                        principalTable: "Artworks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtworkArtworkCharacters_Characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistName_Name",
                table: "ArtistName",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ArtworkArtworkCharacters_CharactersId",
                table: "ArtworkArtworkCharacters",
                column: "CharactersId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistName_Artists_ArtworkArtistId",
                table: "ArtistName",
                column: "ArtworkArtistId",
                principalTable: "Artists",
                principalColumn: "Id");
        }
    }
}
