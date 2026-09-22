using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace POS.Persistence.Migrations
{
    public partial class add_return_documents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReturnDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceInvoiceId = table.Column<int>(type: "INTEGER", nullable: true),
                    SourcePurchaseId = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: true),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: true),
                    WarehouseId = table.Column<int>(type: "INTEGER", nullable: true),
                    Currency = table.Column<int>(type: "INTEGER", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18, 6)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnDocuments", x => x.Id);
                    table.ForeignKey("FK_ReturnDocuments_Invoices_SourceInvoiceId", x => x.SourceInvoiceId, "Invoices", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_ReturnDocuments_Purchases_SourcePurchaseId", x => x.SourcePurchaseId, "Purchases", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_ReturnDocuments_Customers_CustomerId", x => x.CustomerId, "Customers", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_ReturnDocuments_Suppliers_SupplierId", x => x.SupplierId, "Suppliers", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_ReturnDocuments_Warehouses_WarehouseId", x => x.WarehouseId, "Warehouses", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReturnDocumentLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                    ReturnDocumentId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    WarehouseId = table.Column<int>(type: "INTEGER", nullable: true),
                    SourceSaleProductId = table.Column<int>(type: "INTEGER", nullable: true),
                    SourcePurchaseProductId = table.Column<int>(type: "INTEGER", nullable: true),
                    Quantity = table.Column<double>(type: "REAL", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnDocumentLines", x => x.Id);
                    table.ForeignKey("FK_ReturnDocumentLines_ReturnDocuments_ReturnDocumentId", x => x.ReturnDocumentId, "ReturnDocuments", "Id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_ReturnDocumentLines_Products_ProductId", x => x.ProductId, "Products", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_ReturnDocumentLines_Warehouses_WarehouseId", x => x.WarehouseId, "Warehouses", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_ReturnDocumentLines_SaleProducts_SourceSaleProductId", x => x.SourceSaleProductId, "SaleProducts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_ReturnDocumentLines_PurchaseProducts_SourcePurchaseProductId", x => x.SourcePurchaseProductId, "PurchaseProducts", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex("IX_ReturnDocuments_Number", "ReturnDocuments", "Number", unique: true);
            migrationBuilder.CreateIndex("IX_ReturnDocuments_SourceInvoiceId", "ReturnDocuments", "SourceInvoiceId");
            migrationBuilder.CreateIndex("IX_ReturnDocuments_SourcePurchaseId", "ReturnDocuments", "SourcePurchaseId");
            migrationBuilder.CreateIndex("IX_ReturnDocuments_CustomerId", "ReturnDocuments", "CustomerId");
            migrationBuilder.CreateIndex("IX_ReturnDocuments_SupplierId", "ReturnDocuments", "SupplierId");
            migrationBuilder.CreateIndex("IX_ReturnDocuments_WarehouseId", "ReturnDocuments", "WarehouseId");

            migrationBuilder.CreateIndex("IX_ReturnDocumentLines_ReturnDocumentId", "ReturnDocumentLines", "ReturnDocumentId");
            migrationBuilder.CreateIndex("IX_ReturnDocumentLines_ProductId", "ReturnDocumentLines", "ProductId");
            migrationBuilder.CreateIndex("IX_ReturnDocumentLines_WarehouseId", "ReturnDocumentLines", "WarehouseId");
            migrationBuilder.CreateIndex("IX_ReturnDocumentLines_SourceSaleProductId", "ReturnDocumentLines", "SourceSaleProductId");
            migrationBuilder.CreateIndex("IX_ReturnDocumentLines_SourcePurchaseProductId", "ReturnDocumentLines", "SourcePurchaseProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("ReturnDocumentLines");
            migrationBuilder.DropTable("ReturnDocuments");
        }
    }
}