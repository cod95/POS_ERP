using Microsoft.EntityFrameworkCore;
using POS.Domain.Models;
using POS.Persistence.Context;
using POS.Persistence.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace POS.ViewModels
{
    public sealed class ReturnLineRow : INotifyPropertyChanged
    {
        private double _returnQuantity;

        public int SourceLineId { get; init; }
        public string ProductName { get; init; }
        public double OriginalQuantity { get; init; }
        public decimal UnitPrice { get; init; }
        public double ReturnQuantity
        {
            get => _returnQuantity;
            set
            {
                if (_returnQuantity != value)
                {
                    _returnQuantity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReturnQuantity)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ReturnsViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _db = new();
        private readonly ReturnService _service;
        private string _sourceNumber;
        private Warehouse _selectedWarehouse;
        private string _reason;
        private bool _isSalesReturn = true;
        private string _selectedReturnType = "مبيعات";

        public ObservableCollection<Warehouse> Warehouses { get; } = new();
        public ObservableCollection<ReturnLineRow> Lines { get; } = new();

        public string SourceNumber
        {
            get => _sourceNumber;
            set { if (_sourceNumber != value) { _sourceNumber = value; OnPropertyChanged(nameof(SourceNumber)); } }
        }

        public Warehouse SelectedWarehouse
        {
            get => _selectedWarehouse;
            set { if (_selectedWarehouse != value) { _selectedWarehouse = value; OnPropertyChanged(nameof(SelectedWarehouse)); } }
        }

        public string Reason
        {
            get => _reason;
            set { if (_reason != value) { _reason = value; OnPropertyChanged(nameof(Reason)); } }
        }

        public bool IsSalesReturn
        {
            get => _isSalesReturn;
            private set { if (_isSalesReturn != value) { _isSalesReturn = value; OnPropertyChanged(nameof(IsSalesReturn)); } }
        }

        public string SelectedReturnType
        {
            get => _selectedReturnType;
            set
            {
                if (_selectedReturnType != value)
                {
                    _selectedReturnType = value;
                    IsSalesReturn = value == "مبيعات";
                    OnPropertyChanged(nameof(SelectedReturnType));
                }
            }
        }

        public ICommand LoadDocumentCommand { get; }
        public ICommand CreateReturnCommand { get; }

        public ReturnsViewModel()
        {
            _service = new ReturnService(_db);
            LoadDocumentCommand = new RelayCommand(_ => LoadDocument());
            CreateReturnCommand = new RelayCommand(async _ => await CreateReturnAsync());
            foreach (var warehouse in _db.Warehouses.AsNoTracking().OrderBy(x => x.Name).ToList())
                Warehouses.Add(warehouse);
            SelectedWarehouse = Warehouses.FirstOrDefault();
        }

        private void LoadDocument()
        {
            Lines.Clear();
            if (string.IsNullOrWhiteSpace(SourceNumber))
            {
                MessageBox.Show("أدخل رقم الفاتورة أولاً.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsSalesReturn)
            {
                var invoice = _db.Invoices
                    .AsNoTracking()
                    .Include(x => x.SaleProducts!).ThenInclude(x => x.Product)
                    .FirstOrDefault(x => x.Number == SourceNumber && x.Type == InvoiceType.Valid);

                if (invoice == null)
                {
                    MessageBox.Show("لم يتم العثور على فاتورة المبيعات.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                foreach (var line in invoice.SaleProducts ?? new List<POS.Domain.Models.Products.SaleProduct>())
                {
                    Lines.Add(new ReturnLineRow
                    {
                        SourceLineId = line.Id,
                        ProductName = line.Product?.Name ?? $"#{line.ProductId}",
                        OriginalQuantity = line.Quantity,
                        UnitPrice = Convert.ToDecimal(line.SalePrice)
                    });
                }
            }
            else
            {
                var purchase = _db.Purchases
                    .AsNoTracking()
                    .Include(x => x.PurchaseProducts!).ThenInclude(x => x.Product)
                    .FirstOrDefault(x => x.Number == SourceNumber);

                if (purchase == null)
                {
                    MessageBox.Show("لم يتم العثور على فاتورة المشتريات.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                foreach (var line in purchase.PurchaseProducts ?? new List<POS.Domain.Models.Products.PurchaseProduct>())
                {
                    Lines.Add(new ReturnLineRow
                    {
                        SourceLineId = line.Id,
                        ProductName = line.Product?.Name ?? $"#{line.ProductId}",
                        OriginalQuantity = line.Quantity,
                        UnitPrice = Convert.ToDecimal(line.PurchasePrice)
                    });
                }
            }

            if (Lines.Count == 0)
                MessageBox.Show("الفاتورة لا تحتوي على أصناف.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task CreateReturnAsync()
        {
            if (SelectedWarehouse == null)
            {
                MessageBox.Show("اختر المستودع أولاً.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = Lines.Where(x => x.ReturnQuantity > 0).ToDictionary(x => x.SourceLineId, x => x.ReturnQuantity);
            if (selected.Count == 0)
            {
                MessageBox.Show("أدخل كمية الإرجاع لصنف واحد على الأقل.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var document = IsSalesReturn
                    ? await _service.CreateSaleReturnAsync(
                        _db.Invoices.First(x => x.Number == SourceNumber).Id,
                        SelectedWarehouse.Id,
                        selected,
                        Reason)
                    : await _service.CreatePurchaseReturnAsync(
                        _db.Purchases.First(x => x.Number == SourceNumber).Id,
                        SelectedWarehouse.Id,
                        selected,
                        Reason);

                MessageBox.Show($"تم إنشاء مستند الإرجاع {document.Number}.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                Lines.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "تعذر إنشاء الإرجاع", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
