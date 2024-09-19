using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityTapsBillingSync.Migrations
{
    /// <inheritdoc />
    public partial class tblBS_UploadInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BS_UploadInstance_BS_Month_MonthID",
                table: "BS_UploadInstance");

            migrationBuilder.DropTable(
                name: "BS_DebtorCityTap");

            migrationBuilder.DropTable(
                name: "BS_Month");

            migrationBuilder.DropTable(
                name: "BS_WaterReadingExport");

            migrationBuilder.DropTable(
                name: "BS_WaterReadingExportData");

            migrationBuilder.DropTable(
                name: "CTReading");

            migrationBuilder.DropIndex(
                name: "IX_BS_UploadInstance_MonthID",
                table: "BS_UploadInstance");

            migrationBuilder.DropColumn(
                name: "MonthID",
                table: "BS_UploadInstance");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MonthID",
                table: "BS_UploadInstance",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BS_DebtorCityTap",
                columns: table => new
                {
                    DebtorCityTapID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CUSTNMBR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CUSTNMBR_CITYTAP = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BS_DebtorCityTap", x => x.DebtorCityTapID);
                });

            migrationBuilder.CreateTable(
                name: "BS_Month",
                columns: table => new
                {
                    MonthID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonthName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BS_Month", x => x.MonthID);
                });

            migrationBuilder.CreateTable(
                name: "BS_WaterReadingExport",
                columns: table => new
                {
                    WaterReadingExportID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastReadings = table.Column<bool>(type: "bit", nullable: true),
                    MonthID = table.Column<long>(type: "bigint", nullable: false),
                    SALSTERR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BS_WaterReadingExport", x => x.WaterReadingExportID);
                });

            migrationBuilder.CreateTable(
                name: "BS_WaterReadingExportData",
                columns: table => new
                {
                    WaterReadingExportDataID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AREA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CURRENT_READING = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CUSTOMER_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CUSTOMER_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CUSTOMER_ZONING = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ERF_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCityTab = table.Column<bool>(type: "bit", nullable: true),
                    METER_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    METER_READER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeterImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    MonthID = table.Column<long>(type: "bigint", nullable: true),
                    PREVIOUS_READING = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReadingDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteNumber = table.Column<long>(type: "bigint", nullable: true),
                    WaterReadingExportID = table.Column<long>(type: "bigint", nullable: false),
                    Year = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BS_WaterReadingExportData", x => x.WaterReadingExportDataID);
                });

            migrationBuilder.CreateTable(
                name: "CTReading",
                columns: table => new
                {
                    meterPointId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    customerNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isMeterActive = table.Column<bool>(type: "bit", nullable: false),
                    meterSerial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reading = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTReading", x => x.meterPointId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BS_UploadInstance_MonthID",
                table: "BS_UploadInstance",
                column: "MonthID");

            migrationBuilder.AddForeignKey(
                name: "FK_BS_UploadInstance_BS_Month_MonthID",
                table: "BS_UploadInstance",
                column: "MonthID",
                principalTable: "BS_Month",
                principalColumn: "MonthID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
