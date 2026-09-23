using Microsoft.EntityFrameworkCore;
using POS.Domain.Models;
using POS.Domain.Models.Products;
using POS.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace POS.ViewModels
{
    public class MovingProductsViewModel : BaseProductsViewModel
    {
        public class MoveItem
        {
            public int ProductId { get; set; }
            public string Name { get; set; } = string.Empty;
            public double Quantity { get; set; }
            public string? Details { get; set; }
            public double UnitCost { get; set; }
            public int? SourceWarehouseId { get; set; }
        }

        private ObservableCollection<MoveItem> _cartItemsList = new();
        public ObservableCollection<MoveItem> CartItemsList
        {
            get => _cartItemsList;
            set
            {
                if (_cartItemsList != value)
                {
                    _cartItemsList = value;
                    OnPropertyChanged(nameof(CartItemsList));
                }
            }
        }

        private MoveItem? _selectedCartItem;
        public MoveItem? SelectedCartItem
        {
            get => _selectedCartItem;
            set
            {
                if (_selectedCartItem != value)
                {
                    _selectedCartItem = value;
                    OnPropertyChanged(nameof(SelectedCartItem));
                }
            }
        }

        public ICommand AcceptCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand MoveGoodsCommand { get; }

        public MovingProductsViewModel() : base()
        {
            AcceptCommand = new RelayCommand(ExecuteAcceptCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand);
            MoveGoodsCommand = new RelayCommand(ExecuteMoveGoodsCommand);
        }

        private void ExecuteAcceptCommand(object? parameter)
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show("يرجى اختيار المنتج.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedWarehouse == null)
            {
                MessageBox.Show("يرجى اختيار المستودع المصدر.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedWarehouseToTransferTo == null)
            {
                MessageBox.Show("يرجى اختيار المستودع الوجهة.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedWarehouse.Id == SelectedWarehouseToTransferTo.Id)
            {
                MessageBox.Show("لا يمكن النقل إلى نفس المستودع.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Quantity <= 0)
            {
                MessageBox.Show("يرجى إدخال كمية صحيحة.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var available = SelectedProduct.Quantity(SelectedWarehouse.Id);
            if (Quantity > available)
            {
                MessageBox.Show("الكمية المطلوبة أكبر من الكمية المتاحة في المستودع المصدر.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existing = CartItemsList.FirstOrDefault(x =>
                x.ProductId == SelectedProduct.Id &&
                x.SourceWarehouseId == SelectedWarehouse.Id);

            if (existing != null)
            {
                existing.Quantity += Quantity;
            }
            else
            {
                var unitCost = _dbContext.StockMovements
                    .Where(m => m.ProductId == SelectedProduct.Id &&
                                m.WarehouseId == SelectedWarehouse.Id &&
                                m.Quantity > 0)
                    .OrderByDescending(m => m.Date)
                    .ThenByDescending(m => m.Id)
                    .Select(m => (double?)m.UnitCost)
                    .FirstOrDefault()
                    ?? SelectedProduct.GetLastPurchasePrice(SelectedWarehouse.Id)
                    ?? 0;

                CartItemsList.Add(new MoveItem
                {
                    ProductId = SelectedProduct.Id,
                    Name = SelectedProduct.Name,
                    Quantity = Quantity,
                    Details = Notes,
                    UnitCost = unitCost,
                    SourceWarehouseId = SelectedWarehouse.Id
                });
            }

            Quantity = 0;
            Notes = null;
            SelectedProduct = null;
        }

        private void ExecuteCancelCommand(object? parameter)
        {
            CartItemsList.Clear();
            SelectedCartItem = null;
        }

        private void ExecuteDeleteCommand(object? parameter)
        {
            if (SelectedCartItem != null)
            {
                CartItemsList.Remove(SelectedCartItem);
                SelectedCartItem = null;
            }
        }

        private void ExecuteMoveGoodsCommand(object? parameter)
        {
            if (SelectedWarehouse == null || SelectedWarehouseToTransferTo == null)
            {
                MessageBox.Show("يرجى اختيار المستودع المصدر والوجهة.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedWarehouse.Id == SelectedWarehouseToTransferTo.Id)
            {
                MessageBox.Show("لا يمكن النقل إلى نفس المستودع.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CartItemsList.Count == 0)
            {
                MessageBox.Show("لم تتم إضافة أي منتجات للنقل.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var item in CartItemsList)
            {
                var currentQuantity = _dbContext.StockMovements
                    .Where(m => m.ProductId == item.ProductId && m.WarehouseId == SelectedWarehouse.Id)
                    .Sum(m => m.Quantity);

                if (item.Quantity <= 0 || item.Quantity > currentQuantity)
                {
                    MessageBox.Show($"الكمية المطلوبة للمنتج {item.Name} غير متاحة في المستودع المصدر.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var reference = $"TR-{DateTime.Now:yyyyMMddHHmmssfff}";

            foreach (var item in CartItemsList)
            {
                _dbContext.StockMovements.Add(new StockMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = SelectedWarehouse.Id,
                    Quantity = -item.Quantity,
                    UnitCost = item.UnitCost,
                    MovementType = StockMovementType.TransferOut,
                    Date = DateTime.Now,
                    Reference = reference,
                    Notes = item.Details
                });

                _dbContext.StockMovements.Add(new StockMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = SelectedWarehouseToTransferTo.Id,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    MovementType = StockMovementType.TransferIn,
                    Date = DateTime.Now,
                    Reference = reference,
                    Notes = item.Details
                });
            }

            _dbContext.SaveChanges();

            CartItemsList.Clear();
            SelectedCartItem = null;
            MessageBox.Show($"تم نقل المنتجات بنجاح. رقم الحركة: {reference}", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
