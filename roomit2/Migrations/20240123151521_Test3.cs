using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace roomit2.Migrations
{
    public partial class Test3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GastKundeID",
                table: "TblBestellung",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GastKundeID",
                table: "TblBestellung");
        }
    }
}
