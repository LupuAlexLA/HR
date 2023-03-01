using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Update_PAAP_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_SursaFinantare_SursaFinantareId",
                table: "BVC_PAAP");

            migrationBuilder.DropTable(
                name: "BVC_Planificare");

            migrationBuilder.DropTable(
                name: "SursaFinantare");

            migrationBuilder.DropTable(
                name: "BVC_Structs");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_SursaFinantareId",
                table: "BVC_PAAP");

            migrationBuilder.DropColumn(
                name: "SursaFinantareId",
                table: "BVC_PAAP");

            migrationBuilder.AddColumn<int>(
                name: "SursaFinatare",
                table: "BVC_PAAP",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SursaFinatare",
                table: "BVC_PAAP");

            migrationBuilder.AddColumn<int>(
                name: "SursaFinantareId",
                table: "BVC_PAAP",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BVC_Structs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BVC_PAAPId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    OrderView = table.Column<int>(type: "int", nullable: false),
                    StructNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_Structs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_Structs_BVC_PAAP_BVC_PAAPId",
                        column: x => x.BVC_PAAPId,
                        principalTable: "BVC_PAAP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SursaFinantare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    OperationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SursaFinantare", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BVC_Planificare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BVC_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BVC_StructId = table.Column<int>(type: "int", nullable: false),
                    BVC_Year = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    Department = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    PaymentDate = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_Planificare", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_Planificare_BVC_Structs_BVC_StructId",
                        column: x => x.BVC_StructId,
                        principalTable: "BVC_Structs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_SursaFinantareId",
                table: "BVC_PAAP",
                column: "SursaFinantareId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_Planificare_BVC_StructId",
                table: "BVC_Planificare",
                column: "BVC_StructId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_Structs_BVC_PAAPId",
                table: "BVC_Structs",
                column: "BVC_PAAPId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_SursaFinantare_SursaFinantareId",
                table: "BVC_PAAP",
                column: "SursaFinantareId",
                principalTable: "SursaFinantare",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
