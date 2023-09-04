using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smoehring.Home.Data.SqlDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddedArtworksAndFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArtworkId",
                table: "Assets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Artworks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsAdult = table.Column<bool>(type: "bit", nullable: false),
                    Stage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artworks", x => x.Id);
                });

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
                name: "AssetFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StorageFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: true),
                    AssetId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetFiles_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsOwned = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArtistName",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ArtworkArtistId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistName_Artists_ArtworkArtistId",
                        column: x => x.ArtworkArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArtistProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtworkArtistId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistProfile_Artists_ArtworkArtistId",
                        column: x => x.ArtworkArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArtworkArtworkArtist",
                columns: table => new
                {
                    ArtistsId = table.Column<int>(type: "int", nullable: false),
                    ArtworksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtworkArtworkArtist", x => new { x.ArtistsId, x.ArtworksId });
                    table.ForeignKey(
                        name: "FK_ArtworkArtworkArtist_Artists_ArtistsId",
                        column: x => x.ArtistsId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtworkArtworkArtist_Artworks_ArtworksId",
                        column: x => x.ArtworksId,
                        principalTable: "Artworks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_Assets_ArtworkId",
                table: "Assets",
                column: "ArtworkId",
                unique: true,
                filter: "[ArtworkId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistName_ArtworkArtistId",
                table: "ArtistName",
                column: "ArtworkArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistName_Name",
                table: "ArtistName",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistProfile_ArtworkArtistId",
                table: "ArtistProfile",
                column: "ArtworkArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtworkArtworkArtist_ArtworksId",
                table: "ArtworkArtworkArtist",
                column: "ArtworksId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtworkArtworkCharacters_CharactersId",
                table: "ArtworkArtworkCharacters",
                column: "CharactersId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtworkArtworkSet_ArtworksId",
                table: "ArtworkArtworkSet",
                column: "ArtworksId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtworkSets_Name",
                table: "ArtworkSets",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AssetFiles_AssetId",
                table: "AssetFiles",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Name",
                table: "Characters",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Artworks_ArtworkId",
                table: "Assets",
                column: "ArtworkId",
                principalTable: "Artworks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Artworks_ArtworkId",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "ArtistName");

            migrationBuilder.DropTable(
                name: "ArtistProfile");

            migrationBuilder.DropTable(
                name: "ArtworkArtworkArtist");

            migrationBuilder.DropTable(
                name: "ArtworkArtworkCharacters");

            migrationBuilder.DropTable(
                name: "ArtworkArtworkSet");

            migrationBuilder.DropTable(
                name: "AssetFiles");

            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "ArtworkSets");

            migrationBuilder.DropTable(
                name: "Artworks");

            migrationBuilder.DropIndex(
                name: "IX_Assets_ArtworkId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "ArtworkId",
                table: "Assets");
        }
    }
}
