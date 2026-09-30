using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace roomit2.Data.Migrations
{
    public partial class ApplicationUsers4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Anrede",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GeschäftsbedingungenAkzeptiert",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "KundenArt",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KundenType",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LieferAdresseID",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Nachname",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RechnungAdresseID",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Vorname",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "hasTowAdresse",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Anrede",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GeschäftsbedingungenAkzeptiert",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "KundenArt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "KundenType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LieferAdresseID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nachname",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RechnungAdresseID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Vorname",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "hasTowAdresse",
                table: "AspNetUsers");
        }
    }
}
