using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AccConfigInvElem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExpenseAmortizAccount",
                table: "InvoiceElementsDetails",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ThirdPartyAccount",
                table: "AccountConfig",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpenseAmortizAccount",
                table: "InvoiceElementsDetails");

            migrationBuilder.DropColumn(
                name: "ThirdPartyAccount",
                table: "AccountConfig");
        }
    }
}
