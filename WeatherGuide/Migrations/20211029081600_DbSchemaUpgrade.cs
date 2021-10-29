using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherGuide.Migrations
{
    public partial class DbSchemaUpgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Sensors_SensorId",
                table: "Measurements");

            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_Clothings_ClothingId",
                table: "Recommendations");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropIndex(
                name: "IX_Recommendations_ClothingId",
                table: "Recommendations");

            migrationBuilder.DropIndex(
                name: "IX_Measurements_SensorId",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "ClothingId",
                table: "Recommendations");

            migrationBuilder.DropColumn(
                name: "SensorId",
                table: "Measurements");

            migrationBuilder.AddColumn<int>(
                name: "FirstClothingId",
                table: "Recommendations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondClothingId",
                table: "Recommendations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThirdClothingId",
                table: "Recommendations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Measurements",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "Measurements",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Warmth",
                table: "Clothings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_FirstClothingId",
                table: "Recommendations",
                column: "FirstClothingId");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_SecondClothingId",
                table: "Recommendations",
                column: "SecondClothingId");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_ThirdClothingId",
                table: "Recommendations",
                column: "ThirdClothingId");

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_CountryId",
                table: "Measurements",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_StateId",
                table: "Measurements",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_Countries_CountryId",
                table: "Measurements",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_States_StateId",
                table: "Measurements",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_Clothings_FirstClothingId",
                table: "Recommendations",
                column: "FirstClothingId",
                principalTable: "Clothings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_Clothings_SecondClothingId",
                table: "Recommendations",
                column: "SecondClothingId",
                principalTable: "Clothings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_Clothings_ThirdClothingId",
                table: "Recommendations",
                column: "ThirdClothingId",
                principalTable: "Clothings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Countries_CountryId",
                table: "Measurements");

            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_States_StateId",
                table: "Measurements");

            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_Clothings_FirstClothingId",
                table: "Recommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_Clothings_SecondClothingId",
                table: "Recommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_Clothings_ThirdClothingId",
                table: "Recommendations");

            migrationBuilder.DropIndex(
                name: "IX_Recommendations_FirstClothingId",
                table: "Recommendations");

            migrationBuilder.DropIndex(
                name: "IX_Recommendations_SecondClothingId",
                table: "Recommendations");

            migrationBuilder.DropIndex(
                name: "IX_Recommendations_ThirdClothingId",
                table: "Recommendations");

            migrationBuilder.DropIndex(
                name: "IX_Measurements_CountryId",
                table: "Measurements");

            migrationBuilder.DropIndex(
                name: "IX_Measurements_StateId",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "FirstClothingId",
                table: "Recommendations");

            migrationBuilder.DropColumn(
                name: "SecondClothingId",
                table: "Recommendations");

            migrationBuilder.DropColumn(
                name: "ThirdClothingId",
                table: "Recommendations");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "Warmth",
                table: "Clothings");

            migrationBuilder.AddColumn<int>(
                name: "ClothingId",
                table: "Recommendations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SensorId",
                table: "Measurements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sensors_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sensors_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_ClothingId",
                table: "Recommendations",
                column: "ClothingId");

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_SensorId",
                table: "Measurements",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_CountryId",
                table: "Sensors",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_StateId",
                table: "Sensors",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_Sensors_SensorId",
                table: "Measurements",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_Clothings_ClothingId",
                table: "Recommendations",
                column: "ClothingId",
                principalTable: "Clothings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
