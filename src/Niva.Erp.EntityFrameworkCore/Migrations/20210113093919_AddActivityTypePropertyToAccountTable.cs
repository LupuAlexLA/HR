using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddActivityTypePropertyToAccountTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityTypeId",
                table: "Account",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_ActivityTypeId",
                table: "Account",
                column: "ActivityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ActivityTypes_ActivityTypeId",
                table: "Account",
                column: "ActivityTypeId",
                principalTable: "ActivityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_ActivityTypes_ActivityTypeId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_ActivityTypeId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ActivityTypeId",
                table: "Account");
        }
    }
}
