using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddPersonToInvObjectOperTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonStoreInId",
                table: "InvObjectOper",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonStoreOutId",
                table: "InvObjectOper",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectOper_PersonStoreInId",
                table: "InvObjectOper",
                column: "PersonStoreInId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectOper_PersonStoreOutId",
                table: "InvObjectOper",
                column: "PersonStoreOutId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvObjectOper_Persons_PersonStoreInId",
                table: "InvObjectOper",
                column: "PersonStoreInId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvObjectOper_Persons_PersonStoreOutId",
                table: "InvObjectOper",
                column: "PersonStoreOutId",
                principalTable: "Persons",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvObjectOper_Persons_PersonStoreInId",
                table: "InvObjectOper");

            migrationBuilder.DropForeignKey(
                name: "FK_InvObjectOper_Persons_PersonStoreOutId",
                table: "InvObjectOper");

            migrationBuilder.DropIndex(
                name: "IX_InvObjectOper_PersonStoreInId",
                table: "InvObjectOper");

            migrationBuilder.DropIndex(
                name: "IX_InvObjectOper_PersonStoreOutId",
                table: "InvObjectOper");

            migrationBuilder.DropColumn(
                name: "PersonStoreInId",
                table: "InvObjectOper");

            migrationBuilder.DropColumn(
                name: "PersonStoreOutId",
                table: "InvObjectOper");
        }
    }
}
