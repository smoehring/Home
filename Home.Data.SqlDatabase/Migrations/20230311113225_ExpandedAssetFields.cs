using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smoehring.Home.Data.SqlDatabase.Migrations
{
    /// <inheritdoc />
    public partial class ExpandedAssetFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Currency_Language_LanguageId",
                table: "Currency");

            migrationBuilder.DropForeignKey(
                name: "FK_Mediae_MediaType_MediaTypeId",
                table: "Mediae");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Currency_CurrencyId",
                table: "Purchases");

            migrationBuilder.DropTable(
                name: "MediaType");

            migrationBuilder.DropIndex(
                name: "IX_Mediae_MediaTypeId",
                table: "Mediae");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Language",
                table: "Language");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currency",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "MediaTypeId",
                table: "Mediae");

            migrationBuilder.RenameTable(
                name: "Language",
                newName: "Languages");

            migrationBuilder.RenameTable(
                name: "Currency",
                newName: "Currencies");

            migrationBuilder.RenameIndex(
                name: "IX_Language_LanguageCultureName",
                table: "Languages",
                newName: "IX_Languages_LanguageCultureName");

            migrationBuilder.RenameIndex(
                name: "IX_Language_DisplayName",
                table: "Languages",
                newName: "IX_Languages_DisplayName");

            migrationBuilder.RenameIndex(
                name: "IX_Currency_LanguageId",
                table: "Currencies",
                newName: "IX_Currencies_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_Currency_Code",
                table: "Currencies",
                newName: "IX_Currencies_Code");

            migrationBuilder.AddColumn<string>(
                name: "ModelNumber",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetTypeId",
                table: "Assets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "Assets",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Languages",
                table: "Languages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AssetTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetTypeId",
                table: "Assets",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_BrandId",
                table: "Assets",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_Name",
                table: "AssetTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetTypes_AssetTypeId",
                table: "Assets",
                column: "AssetTypeId",
                principalTable: "AssetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Brands_BrandId",
                table: "Assets",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Currencies_Languages_LanguageId",
                table: "Currencies",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Currencies_CurrencyId",
                table: "Purchases",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetTypes_AssetTypeId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Brands_BrandId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Currencies_Languages_LanguageId",
                table: "Currencies");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Currencies_CurrencyId",
                table: "Purchases");

            migrationBuilder.DropTable(
                name: "AssetTypes");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Assets_AssetTypeId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_BrandId",
                table: "Assets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Languages",
                table: "Languages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "ModelNumber",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "AssetTypeId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Assets");

            migrationBuilder.RenameTable(
                name: "Languages",
                newName: "Language");

            migrationBuilder.RenameTable(
                name: "Currencies",
                newName: "Currency");

            migrationBuilder.RenameIndex(
                name: "IX_Languages_LanguageCultureName",
                table: "Language",
                newName: "IX_Language_LanguageCultureName");

            migrationBuilder.RenameIndex(
                name: "IX_Languages_DisplayName",
                table: "Language",
                newName: "IX_Language_DisplayName");

            migrationBuilder.RenameIndex(
                name: "IX_Currencies_LanguageId",
                table: "Currency",
                newName: "IX_Currency_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_Currencies_Code",
                table: "Currency",
                newName: "IX_Currency_Code");

            migrationBuilder.AddColumn<int>(
                name: "MediaTypeId",
                table: "Mediae",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Language",
                table: "Language",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currency",
                table: "Currency",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MediaType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mediae_MediaTypeId",
                table: "Mediae",
                column: "MediaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaType_Name",
                table: "MediaType",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Currency_Language_LanguageId",
                table: "Currency",
                column: "LanguageId",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Mediae_MediaType_MediaTypeId",
                table: "Mediae",
                column: "MediaTypeId",
                principalTable: "MediaType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Currency_CurrencyId",
                table: "Purchases",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
