using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddedAutoOperationSearchConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExceptiiEliminareRegInventarConfig");

            migrationBuilder.DropTable(
                name: "ExceptiiRegInventarConfig");

            migrationBuilder.AddColumn<int>(
                name: "FgnOperDetailId",
                table: "PaymentOrders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AutoOperSearchConfigId",
                table: "AutoOperationConfig",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AutoOperationSearchConfigId",
                table: "AutoOperationConfig",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AutoOperationSearchConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    AutoOperType = table.Column<int>(nullable: false),
                    OperationType = table.Column<int>(nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoOperationSearchConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutoOperationSearchConfig_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_FgnOperDetailId",
                table: "PaymentOrders",
                column: "FgnOperDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoOperationConfig_AutoOperationSearchConfigId",
                table: "AutoOperationConfig",
                column: "AutoOperationSearchConfigId");

          

            migrationBuilder.CreateIndex(
                name: "IX_AutoOperationSearchConfig_DocumentTypeId",
                table: "AutoOperationSearchConfig",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AutoOperationConfig_AutoOperationSearchConfig_AutoOperationSearchConfigId",
                table: "AutoOperationConfig",
                column: "AutoOperationSearchConfigId",
                principalTable: "AutoOperationSearchConfig",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentOrders_ForeignOperationsDetails_FgnOperDetailId",
                table: "PaymentOrders",
                column: "FgnOperDetailId",
                principalTable: "ForeignOperationsDetails",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.DropForeignKey(
                name: "FK_AutoOperationConfig_AutoOperationSearchConfig_AutoOperationSearchConfigId",
                table: "AutoOperationConfig");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrders_ForeignOperationsDetails_FgnOperDetailId",
                table: "PaymentOrders");

            migrationBuilder.DropTable(
                name: "AutoOperationSearchConfig");

            migrationBuilder.DropTable(
                name: "Exchange");


            migrationBuilder.DropIndex(
                name: "IX_PaymentOrders_FgnOperDetailId",
                table: "PaymentOrders");

            migrationBuilder.DropIndex(
                name: "IX_AutoOperationConfig_AutoOperationSearchConfigId",
                table: "AutoOperationConfig");

            migrationBuilder.DropColumn(
                name: "FgnOperDetailId",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "AutoOperSearchConfigId",
                table: "AutoOperationConfig");

            migrationBuilder.DropColumn(
                name: "AutoOperationSearchConfigId",
                table: "AutoOperationConfig");
        }
    }
}
