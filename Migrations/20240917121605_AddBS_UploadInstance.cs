using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityTapsBillingSync.Migrations
{
    /// <inheritdoc />
    public partial class AddBS_UploadInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BS_UploadInstance",
                columns: table => new
                {
                    UploadInstanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonthID = table.Column<long>(type: "bigint", nullable: false),
                    MonthId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Site = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BS_UploadInstance", x => x.UploadInstanceID);
                    table.ForeignKey(
                        name: "FK_BS_UploadInstance_BS_Month_MonthID",
                        column: x => x.MonthID,
                        principalTable: "BS_Month",
                        principalColumn: "MonthID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BS_UploadInstance_MonthID",
                table: "BS_UploadInstance",
                column: "MonthID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BS_UploadInstance");
        }
    }
}
