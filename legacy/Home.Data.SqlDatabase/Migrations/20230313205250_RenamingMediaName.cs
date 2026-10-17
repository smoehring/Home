using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smoehring.Home.Data.SqlDatabase.Migrations
{
    /// <inheritdoc />
    public partial class RenamingMediaName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaName_Languages_LanguageId",
                table: "MediaName");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaName_Mediae_MediaId",
                table: "MediaName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaName",
                table: "MediaName");

            migrationBuilder.RenameTable(
                name: "MediaName",
                newName: "MediaNames");

            migrationBuilder.RenameIndex(
                name: "IX_MediaName_Name",
                table: "MediaNames",
                newName: "IX_MediaNames_Name");

            migrationBuilder.RenameIndex(
                name: "IX_MediaName_MediaId",
                table: "MediaNames",
                newName: "IX_MediaNames_MediaId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaName_LanguageId",
                table: "MediaNames",
                newName: "IX_MediaNames_LanguageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaNames",
                table: "MediaNames",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaNames_Languages_LanguageId",
                table: "MediaNames",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaNames_Mediae_MediaId",
                table: "MediaNames",
                column: "MediaId",
                principalTable: "Mediae",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaNames_Languages_LanguageId",
                table: "MediaNames");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaNames_Mediae_MediaId",
                table: "MediaNames");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaNames",
                table: "MediaNames");

            migrationBuilder.RenameTable(
                name: "MediaNames",
                newName: "MediaName");

            migrationBuilder.RenameIndex(
                name: "IX_MediaNames_Name",
                table: "MediaName",
                newName: "IX_MediaName_Name");

            migrationBuilder.RenameIndex(
                name: "IX_MediaNames_MediaId",
                table: "MediaName",
                newName: "IX_MediaName_MediaId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaNames_LanguageId",
                table: "MediaName",
                newName: "IX_MediaName_LanguageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaName",
                table: "MediaName",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaName_Languages_LanguageId",
                table: "MediaName",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaName_Mediae_MediaId",
                table: "MediaName",
                column: "MediaId",
                principalTable: "Mediae",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
