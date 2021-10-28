using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherGuide.Migrations
{
    public partial class DBExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Clothings");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Clothings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clothings_CategoryId",
                table: "Clothings",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clothings_Categories_CategoryId",
                table: "Clothings",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clothings_Categories_CategoryId",
                table: "Clothings");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Clothings_CategoryId",
                table: "Clothings");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Clothings");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Clothings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
