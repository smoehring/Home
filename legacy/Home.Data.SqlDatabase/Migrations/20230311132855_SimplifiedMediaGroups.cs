using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smoehring.Home.Data.SqlDatabase.Migrations
{
    /// <inheritdoc />
    public partial class SimplifiedMediaGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Media2MediaGroup");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Mediae",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupOrder",
                table: "Mediae",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mediae_GroupId",
                table: "Mediae",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mediae_MediaGroups_GroupId",
                table: "Mediae",
                column: "GroupId",
                principalTable: "MediaGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mediae_MediaGroups_GroupId",
                table: "Mediae");

            migrationBuilder.DropIndex(
                name: "IX_Mediae_GroupId",
                table: "Mediae");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Mediae");

            migrationBuilder.DropColumn(
                name: "GroupOrder",
                table: "Mediae");

            migrationBuilder.CreateTable(
                name: "Media2MediaGroup",
                columns: table => new
                {
                    MediaId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media2MediaGroup", x => new { x.MediaId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_Media2MediaGroup_MediaGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "MediaGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Media2MediaGroup_Mediae_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Mediae",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Media2MediaGroup_GroupId",
                table: "Media2MediaGroup",
                column: "GroupId");
        }
    }
}
