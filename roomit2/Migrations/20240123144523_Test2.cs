using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace roomit2.Migrations
{
    public partial class Test2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblBestellung_TblGastkunden_GastKundeID",
                table: "TblBestellung");

            migrationBuilder.DropIndex(
                name: "IX_TblBestellung_GastKundeID",
                table: "TblBestellung");

            migrationBuilder.DropColumn(
                name: "GastKundeID",
                table: "TblBestellung");

            migrationBuilder.CreateIndex(
                name: "IX_TblGastkunden_BestellungID",
                table: "TblGastkunden",
                column: "BestellungID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TblGastkunden_TblBestellung_BestellungID",
                table: "TblGastkunden",
                column: "BestellungID",
                principalTable: "TblBestellung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblGastkunden_TblBestellung_BestellungID",
                table: "TblGastkunden");

            migrationBuilder.DropIndex(
                name: "IX_TblGastkunden_BestellungID",
                table: "TblGastkunden");

            migrationBuilder.AddColumn<int>(
                name: "GastKundeID",
                table: "TblBestellung",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TblBestellung_GastKundeID",
                table: "TblBestellung",
                column: "GastKundeID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TblBestellung_TblGastkunden_GastKundeID",
                table: "TblBestellung",
                column: "GastKundeID",
                principalTable: "TblGastkunden",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
