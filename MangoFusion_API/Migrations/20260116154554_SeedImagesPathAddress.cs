using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangoFusion_API.Migrations
{
    /// <inheritdoc />
    public partial class SeedImagesPathAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 1,
                column: "image",
                value: "Images/spring roll.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 2,
                column: "image",
                value: "Images/idli.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 3,
                column: "image",
                value: "Images/pani puri.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 4,
                column: "image",
                value: "Images/hakka noodles.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 5,
                column: "image",
                value: "Images/malai kofta.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 6,
                column: "image",
                value: "Images/paneer pizza.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 7,
                column: "image",
                value: "Images/paneer tikka.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 8,
                column: "image",
                value: "Images/carrot love.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 9,
                column: "image",
                value: "Images/rasmalai.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 10,
                column: "image",
                value: "Images/sweet rolls.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 1,
                column: "image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 2,
                column: "image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 3,
                column: "image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 4,
                column: "image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 5,
                column: "image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 6,
                column: "image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 7,
                column: "image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 8,
                column: "image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 9,
                column: "image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "id",
                keyValue: 10,
                column: "image",
                value: "");
        }
    }
}
