using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace POS.Persistence.Migrations
{
    public partial class erp_core_phase1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>("CostPrice", "SaleProducts", type: "REAL", nullable: false, defaultValue: 0.0);
            migrationBuilder.AddColumn<int>("Currency", "Invoices", type: "INTEGER", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<decimal>("ExchangeRate", "Invoices", type: "decimal(18, 6)", nullable: false, defaultValue: 1m);
            migrationBuilder.AddColumn<int>("Currency", "Purchases", type: "INTEGER", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<decimal>("ExchangeRate", "Purchases", type: "decimal(18, 6)", nullable: false, defaultValue: 1m);
            migrationBuilder.AddColumn<int>("Currency", "InvoicePayments", type: "INTEGER", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<decimal>("ExchangeRate", "InvoicePayments", type: "decimal(18, 6)", nullable: false, defaultValue: 1m);
            migrationBuilder.AddColumn<int>("Currency", "PurchasePayments", type: "INTEGER", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<decimal>("ExchangeRate", "PurchasePayments", type: "decimal(18, 6)", nullable: false, defaultValue: 1m);

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    WarehouseId = table.Column<int>(type: "INTEGER", nullable: true),
                    Quantity = table.Column<double>(type: "REAL", nullable: false),
                    UnitCost = table.Column<double>(type: "REAL", nullable: false),
                    MovementType = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: true),
                    PurchaseId = table.Column<int>(type: "INTEGER", nullable: true),
                    Reference = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.ForeignKey("FK_StockMovements_Products_ProductId", x => x.ProductId, "Products", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_StockMovements_Warehouses_WarehouseId", x => x.WarehouseId, "Warehouses", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_StockMovements_Invoices_InvoiceId", x => x.InvoiceId, "Invoices", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_StockMovements_Purchases_PurchaseId", x => x.PurchaseId, "Purchases", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex("IX_StockMovements_ProductId", "StockMovements", "ProductId");
            migrationBuilder.CreateIndex("IX_StockMovements_WarehouseId", "StockMovements", "WarehouseId");
            migrationBuilder.CreateIndex("IX_StockMovements_InvoiceId", "StockMovements", "InvoiceId");
            migrationBuilder.CreateIndex("IX_StockMovements_PurchaseId", "StockMovements", "PurchaseId");
            migrationBuilder.CreateIndex("IX_StockMovements_Date", "StockMovements", "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("StockMovements");
            migrationBuilder.DropColumn("CostPrice", "SaleProducts");
            migrationBuilder.DropColumn("Currency", "Invoices");
            migrationBuilder.DropColumn("ExchangeRate", "Invoices");
            migrationBuilder.DropColumn("Currency", "Purchases");
            migrationBuilder.DropColumn("ExchangeRate", "Purchases");
            migrationBuilder.DropColumn("Currency", "InvoicePayments");
            migrationBuilder.DropColumn("ExchangeRate", "InvoicePayments");
            migrationBuilder.DropColumn("Currency", "PurchasePayments");
            migrationBuilder.DropColumn("ExchangeRate", "PurchasePayments");
        }
    }
}