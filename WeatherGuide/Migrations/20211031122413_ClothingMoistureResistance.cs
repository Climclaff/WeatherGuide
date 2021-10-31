using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherGuide.Migrations
{
    public partial class ClothingMoistureResistance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MoistureResistance",
                table: "Clothings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoistureResistance",
                table: "Clothings");
        }
    }
}
