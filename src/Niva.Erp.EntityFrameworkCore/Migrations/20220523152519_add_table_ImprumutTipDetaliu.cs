using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class add_table_ImprumutTipDetaliu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Account",
                table: "ImprumutTipuri");

            migrationBuilder.CreateTable(
                name: "ImprumutTipDetalii",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(nullable: true),
                    ContImprumutId = table.Column<int>(nullable: false),
                    ActivityTypeId = table.Column<int>(nullable: false),
                    ImprumutTipId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImprumutTipDetalii", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImprumutTipDetalii_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImprumutTipDetalii_Account_ContImprumutId",
                        column: x => x.ContImprumutId,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImprumutTipDetalii_ImprumutTipuri_ImprumutTipId",
                        column: x => x.ImprumutTipId,
                        principalTable: "ImprumutTipuri",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImprumutTipDetalii_ActivityTypeId",
                table: "ImprumutTipDetalii",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ImprumutTipDetalii_ContImprumutId",
                table: "ImprumutTipDetalii",
                column: "ContImprumutId");

            migrationBuilder.CreateIndex(
                name: "IX_ImprumutTipDetalii_ImprumutTipId",
                table: "ImprumutTipDetalii",
                column: "ImprumutTipId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImprumutTipDetalii");

            migrationBuilder.AddColumn<string>(
                name: "Account",
                table: "ImprumutTipuri",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
