using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Domain.Models.Returns;

public enum ReturnDocumentType
{
    SaleReturn = 0,
    PurchaseReturn = 1
}

public class ReturnDocument : BaseEntity
{
    public int Id { get; set; }

    [Required]
    public string Number { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    public ReturnDocumentType Type { get; set; }

    public int? SourceInvoiceId { get; set; }
    public Invoice? SourceInvoice { get; set; }

    public int? SourcePurchaseId { get; set; }
    public Purchase? SourcePurchase { get; set; }

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public int? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public Currency Currency { get; set; } = Currency.USD;

    [Column(TypeName = "decimal(18, 6)")]
    public decimal ExchangeRate { get; set; } = 1m;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Total { get; set; }

    public string? Reason { get; set; }

    public ICollection<ReturnDocumentLine> Lines { get; set; } = new List<ReturnDocumentLine>();
}
