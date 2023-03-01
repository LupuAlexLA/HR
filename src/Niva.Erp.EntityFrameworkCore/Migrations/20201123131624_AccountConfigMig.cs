using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AccountConfigMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Symbol = table.Column<string>(maxLength: 1000, nullable: true),
                    AccountRad = table.Column<string>(maxLength: 1000, nullable: true),
                    Sufix = table.Column<string>(maxLength: 1000, nullable: true),
                    IncludPunct = table.Column<bool>(nullable: false),
                    IncludId1 = table.Column<bool>(nullable: false),
                    ExactAccount = table.Column<bool>(nullable: false),
                    ValabilityDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ImoAssetStorageId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountConfig_ImoAssetStorage_ImoAssetStorageId",
                        column: x => x.ImoAssetStorageId,
                        principalTable: "ImoAssetStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountConfig_ImoAssetStorageId",
                table: "AccountConfig",
                column: "ImoAssetStorageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountConfig");
        }
    }
}
