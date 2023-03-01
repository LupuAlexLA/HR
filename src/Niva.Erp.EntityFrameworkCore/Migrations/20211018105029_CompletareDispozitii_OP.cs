using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class CompletareDispozitii_OP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Rest",
                table: "PaymentOrderInvoice",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rest",
                table: "DispositionInvoice",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rest",
                table: "PaymentOrderInvoice");

            migrationBuilder.DropColumn(
                name: "Rest",
                table: "DispositionInvoice");
        }
    }
}
