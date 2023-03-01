using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class EliminareColoanaRestDispInvoice_OPInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rest",
                table: "PaymentOrderInvoice");

            migrationBuilder.DropColumn(
                name: "Rest",
                table: "DispositionInvoice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Rest",
                table: "PaymentOrderInvoice",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rest",
                table: "DispositionInvoice",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
