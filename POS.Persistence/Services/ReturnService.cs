using Microsoft.EntityFrameworkCore;
using POS.Domain.Models;
using POS.Domain.Models.Products;
using POS.Domain.Models.Returns;
using POS.Persistence.Context;

namespace POS.Persistence.Services;

public sealed class ReturnService
{
    private readonly AppDbContext _db;

    public ReturnService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ReturnDocument> CreateSaleReturnAsync(
        int invoiceId,
        int warehouseId,
        IReadOnlyDictionary<int, double> quantitiesBySaleProductId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        if (quantitiesBySaleProductId.Count == 0)
            throw new ArgumentException("Return must contain at least one line.", nameof(quantitiesBySaleProductId));

        var invoice = await _db.Invoices
            .Include(i => i.SaleProducts!)
            .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken)
            ?? throw new InvalidOperationException("Source sales invoice was not found.");

        if (invoice.Type != InvoiceType.Valid)
            throw new InvalidOperationException("Only a valid sales invoice can be returned.");

        var saleIds = quantitiesBySaleProductId.Keys.ToList();

        var alreadyReturned = await _db.ReturnDocumentLines
            .Where(x => x.SourceSaleProductId.HasValue &&
                        saleIds.Contains(x.SourceSaleProductId.Value) &&
                        x.ReturnDocument.Type == ReturnDocumentType.SaleReturn)
            .GroupBy(x => x.SourceSaleProductId!.Value)
            .Select(g => new { Id = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.Id, x => x.Quantity, cancellationToken);

        var lines = new List<ReturnDocumentLine>();

        foreach (var requested in quantitiesBySaleProductId)
        {
            var saleLine = invoice.SaleProducts!.FirstOrDefault(x => x.Id == requested.Key)
                ?? throw new InvalidOperationException($"Sale line {requested.Key} does not belong to invoice.");

            if (requested.Value <= 0)
                throw new InvalidOperationException("Return quantity must be greater than zero.");

            var returned = alreadyReturned.GetValueOrDefault(saleLine.Id);
            if (returned + requested.Value > saleLine.Quantity)
                throw new InvalidOperationException($"Return quantity exceeds the remaining quantity for product {saleLine.ProductId}.");

            var unitPrice = Convert.ToDecimal(saleLine.SalePrice);
            lines.Add(new ReturnDocumentLine
            {
                ProductId = saleLine.ProductId,
                WarehouseId = warehouseId,
                SourceSaleProductId = saleLine.Id,
                Quantity = requested.Value,
                UnitPrice = unitPrice,
                UnitCost = Convert.ToDecimal(saleLine.CostPrice),
                LineTotal = unitPrice * Convert.ToDecimal(requested.Value)
            });
        }

        var number = $"SR-{DateTime.Now:yyyyMMddHHmmssfff}";
        var document = new ReturnDocument
        {
            Number = number,
            Date = DateTime.Now,
            Type = ReturnDocumentType.SaleReturn,
            SourceInvoiceId = invoice.Id,
            CustomerId = invoice.CustomerId,
            WarehouseId = warehouseId,
            Currency = invoice.Currency,
            ExchangeRate = invoice.ExchangeRate,
            Reason = reason,
            Lines = lines,
            Subtotal = lines.Sum(x => x.LineTotal),
            Total = lines.Sum(x => x.LineTotal)
        };

        _db.ReturnDocuments.Add(document);

        foreach (var line in lines)
        {
            _db.StockMovements.Add(new StockMovement
            {
                ProductId = line.ProductId,
                WarehouseId = warehouseId,
                Quantity = line.Quantity,
                UnitCost = line.UnitCost,
                MovementType = StockMovementType.SaleReturn,
                Date = document.Date,
                Reference = document.Number,
                Notes = reason,
                InvoiceId = invoice.Id
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
        return document;
    }

    public async Task<ReturnDocument> CreatePurchaseReturnAsync(
        int purchaseId,
        int warehouseId,
        IReadOnlyDictionary<int, double> quantitiesByPurchaseProductId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        if (quantitiesByPurchaseProductId.Count == 0)
            throw new ArgumentException("Return must contain at least one line.", nameof(quantitiesByPurchaseProductId));

        var purchase = await _db.Purchases
            .Include(p => p.PurchaseProducts!)
            .FirstOrDefaultAsync(p => p.Id == purchaseId, cancellationToken)
            ?? throw new InvalidOperationException("Source purchase was not found.");

        var purchaseLines = purchase.PurchaseProducts ?? new List<PurchaseProduct>();
        var purchaseIds = quantitiesByPurchaseProductId.Keys.ToList();

        var alreadyReturned = await _db.ReturnDocumentLines
            .Where(x => x.SourcePurchaseProductId.HasValue &&
                        purchaseIds.Contains(x.SourcePurchaseProductId.Value) &&
                        x.ReturnDocument.Type == ReturnDocumentType.PurchaseReturn)
            .GroupBy(x => x.SourcePurchaseProductId!.Value)
            .Select(g => new { Id = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.Id, x => x.Quantity, cancellationToken);

        var lines = new List<ReturnDocumentLine>();

        foreach (var requested in quantitiesByPurchaseProductId)
        {
            var purchaseLine = purchaseLines.FirstOrDefault(x => x.Id == requested.Key)
                ?? throw new InvalidOperationException($"Purchase line {requested.Key} does not belong to purchase.");

            if (requested.Value <= 0)
                throw new InvalidOperationException("Return quantity must be greater than zero.");

            var returned = alreadyReturned.GetValueOrDefault(purchaseLine.Id);
            if (returned + requested.Value > purchaseLine.Quantity)
                throw new InvalidOperationException($"Return quantity exceeds the remaining quantity for product {purchaseLine.ProductId}.");

            var unitPrice = Convert.ToDecimal(purchaseLine.PurchasePrice);
            lines.Add(new ReturnDocumentLine
            {
                ProductId = purchaseLine.ProductId,
                WarehouseId = warehouseId,
                SourcePurchaseProductId = purchaseLine.Id,
                Quantity = requested.Value,
                UnitPrice = unitPrice,
                UnitCost = unitPrice,
                LineTotal = unitPrice * Convert.ToDecimal(requested.Value)
            });
        }

        var number = $"PR-{DateTime.Now:yyyyMMddHHmmssfff}";
        var document = new ReturnDocument
        {
            Number = number,
            Date = DateTime.Now,
            Type = ReturnDocumentType.PurchaseReturn,
            SourcePurchaseId = purchase.Id,
            SupplierId = purchase.SupplierId,
            WarehouseId = warehouseId,
            Currency = purchase.Currency,
            ExchangeRate = purchase.ExchangeRate,
            Reason = reason,
            Lines = lines,
            Subtotal = lines.Sum(x => x.LineTotal),
            Total = lines.Sum(x => x.LineTotal)
        };

        _db.ReturnDocuments.Add(document);

        foreach (var line in lines)
        {
            _db.StockMovements.Add(new StockMovement
            {
                ProductId = line.ProductId,
                WarehouseId = warehouseId,
                Quantity = -line.Quantity,
                UnitCost = line.UnitCost,
                MovementType = StockMovementType.PurchaseReturn,
                Date = document.Date,
                Reference = document.Number,
                Notes = reason,
                PurchaseId = purchase.Id
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
        return document;
    }
}
