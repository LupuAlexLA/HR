using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateFormRand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AlterColumn<int>(
                name: "TipRand",
                table: "BVC_FormRand",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "Bold",
                table: "BVC_FormRand",
                nullable: false,
                defaultValue: false);

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropColumn(
                name: "Bold",
                table: "BVC_FormRand");

            migrationBuilder.AlterColumn<int>(
                name: "TipRand",
                table: "BVC_FormRand",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

           
        }
    }
}
