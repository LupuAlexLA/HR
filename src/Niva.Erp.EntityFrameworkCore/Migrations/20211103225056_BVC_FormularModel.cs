using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVC_FormularModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_BugetPrev",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    FormularId = table.Column<int>(nullable: false),
                    DataBuget = table.Column<DateTime>(nullable: false),
                    BVC_Tip = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_BugetPrev", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrev_BVC_Formular_FormularId",
                        column: x => x.FormularId,
                        principalTable: "BVC_Formular",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BVC_BugetPrevAutoValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    DepartamentId = table.Column<int>(nullable: false),
                    TipRand = table.Column<int>(nullable: false),
                    TipRandVenit = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_BugetPrevAutoValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevAutoValue_Departament_DepartamentId",
                        column: x => x.DepartamentId,
                        principalTable: "Departament",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BVC_BugetPrevRand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BugetPrevId = table.Column<int>(nullable: false),
                    FormRandId = table.Column<int>(nullable: false),
                    DepartamentId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    BVC_BugetPrevId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_BugetPrevRand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevRand_BVC_BugetPrev_BVC_BugetPrevId",
                        column: x => x.BVC_BugetPrevId,
                        principalTable: "BVC_BugetPrev",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevRand_BVC_BugetPrev_BugetPrevId",
                        column: x => x.BugetPrevId,
                        principalTable: "BVC_BugetPrev",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevRand_Departament_DepartamentId",
                        column: x => x.DepartamentId,
                        principalTable: "Departament",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevRand_BVC_FormRand_FormRandId",
                        column: x => x.FormRandId,
                        principalTable: "BVC_FormRand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BVC_BugetPrevRandStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BugetPrevRandId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StatusDate = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    BVC_BugetPrevRandId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_BugetPrevRandStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevRandStatus_BVC_BugetPrevRand_BVC_BugetPrevRandId",
                        column: x => x.BVC_BugetPrevRandId,
                        principalTable: "BVC_BugetPrevRand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevRandStatus_BVC_BugetPrevRand_BugetPrevRandId",
                        column: x => x.BugetPrevRandId,
                        principalTable: "BVC_BugetPrevRand",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BVC_BugetPrevRandValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BugetPrevRandId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ValueType = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    ActivityTypeId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    BVC_BugetPrevRandId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_BugetPrevRandValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevRandValue_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevRandValue_BVC_BugetPrevRand_BVC_BugetPrevRandId",
                        column: x => x.BVC_BugetPrevRandId,
                        principalTable: "BVC_BugetPrevRand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevRandValue_BVC_BugetPrevRand_BugetPrevRandId",
                        column: x => x.BugetPrevRandId,
                        principalTable: "BVC_BugetPrevRand",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrev_FormularId",
                table: "BVC_BugetPrev",
                column: "FormularId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevAutoValue_DepartamentId",
                table: "BVC_BugetPrevAutoValue",
                column: "DepartamentId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRand_BVC_BugetPrevId",
                table: "BVC_BugetPrevRand",
                column: "BVC_BugetPrevId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRand_BugetPrevId",
                table: "BVC_BugetPrevRand",
                column: "BugetPrevId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRand_DepartamentId",
                table: "BVC_BugetPrevRand",
                column: "DepartamentId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRand_FormRandId",
                table: "BVC_BugetPrevRand",
                column: "FormRandId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRandStatus_BVC_BugetPrevRandId",
                table: "BVC_BugetPrevRandStatus",
                column: "BVC_BugetPrevRandId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRandStatus_BugetPrevRandId",
                table: "BVC_BugetPrevRandStatus",
                column: "BugetPrevRandId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRandValue_ActivityTypeId",
                table: "BVC_BugetPrevRandValue",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRandValue_BVC_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue",
                column: "BVC_BugetPrevRandId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRandValue_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue",
                column: "BugetPrevRandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_BugetPrevAutoValue");

            migrationBuilder.DropTable(
                name: "BVC_BugetPrevRandStatus");

            migrationBuilder.DropTable(
                name: "BVC_BugetPrevRandValue");

            migrationBuilder.DropTable(
                name: "BVC_BugetPrevRand");

            migrationBuilder.DropTable(
                name: "BVC_BugetPrev");
        }
    }
}
