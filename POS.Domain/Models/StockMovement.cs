using POS.Domain.Models.Products;

namespace POS.Domain.Models
{
    public enum StockMovementType
    {
        Purchase = 0,
        Sale = 1,
        PurchaseReturn = 2,
        SaleReturn = 3,
        AdjustmentIn = 4,
        AdjustmentOut = 5,
        TransferIn = 6,
        TransferOut = 7
    }

    public class StockMovement : BaseEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int? WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public double Quantity { get; set; }
        public double UnitCost { get; set; }
        public StockMovementType MovementType { get; set; }
        public DateTime Date { get; set; }
        public int? InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        public int? PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
    }
}
