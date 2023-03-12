using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smoehring.Home.Data.SqlDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddedMultipleMediaNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaName",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MediaId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaName_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MediaName_Mediae_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Mediae",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaName_LanguageId",
                table: "MediaName",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaName_MediaId",
                table: "MediaName",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaName_Name",
                table: "MediaName",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaName");
        }
    }
}
