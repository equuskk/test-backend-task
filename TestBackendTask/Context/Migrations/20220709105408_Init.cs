using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBackendTask.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    FromDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ToDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CountSignIn = table.Column<int>(type: "INTEGER", nullable: false),
                    ReportId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportResults_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportResults_ReportId",
                table: "ReportResults",
                column: "ReportId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportResults");

            migrationBuilder.DropTable(
                name: "Reports");
        }
    }
}
