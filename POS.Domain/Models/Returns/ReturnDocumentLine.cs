using System.ComponentModel.DataAnnotations.Schema;
using POS.Domain.Models.Products;

namespace POS.Domain.Models.Returns;

public class ReturnDocumentLine : BaseEntity
{
    public int Id { get; set; }

    public int ReturnDocumentId { get; set; }
    public ReturnDocument ReturnDocument { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public int? SourceSaleProductId { get; set; }
    public SaleProduct? SourceSaleProduct { get; set; }

    public int? SourcePurchaseProductId { get; set; }
    public PurchaseProduct? SourcePurchaseProduct { get; set; }

    public double Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitCost { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal LineTotal { get; set; }
}
