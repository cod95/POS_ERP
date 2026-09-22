using Microsoft.EntityFrameworkCore;
using POS.Domain.Models;
using POS.Persistence.Context;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace POS.ViewModels
{
    public sealed class AccountStatementRow
    {
        public DateTime Date { get; init; }
        public string DocumentNumber { get; init; }
        public string Description { get; init; }
        public decimal Debit { get; init; }
        public decimal Credit { get; init; }
        public decimal Balance { get; init; }
        public Currency Currency { get; init; }
        public decimal ExchangeRate { get; init; }
    }

    public class AccountStatementViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _db = new();

        private Customer _selectedCustomer;
        private Supplier _selectedSupplier;

        public ObservableCollection<Customer> Customers { get; } = new();
        public ObservableCollection<Supplier> Suppliers { get; } = new();
        public ObservableCollection<AccountStatementRow> CustomerRows { get; } = new();
        public ObservableCollection<AccountStatementRow> SupplierRows { get; } = new();

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set { if (_selectedCustomer != value) { _selectedCustomer = value; OnPropertyChanged(nameof(SelectedCustomer)); LoadCustomerStatement(); } }
        }

        public Supplier SelectedSupplier
        {
            get => _selectedSupplier;
            set { if (_selectedSupplier != value) { _selectedSupplier = value; OnPropertyChanged(nameof(SelectedSupplier)); LoadSupplierStatement(); } }
        }

        public decimal CustomerBalance => CustomerRows.LastOrDefault()?.Balance ?? 0m;
        public decimal SupplierBalance => SupplierRows.LastOrDefault()?.Balance ?? 0m;

        public ICommand RefreshCommand { get; }

        public AccountStatementViewModel()
        {
            RefreshCommand = new RelayCommand(_ => LoadAccounts());
            LoadAccounts();
        }

        private void LoadAccounts()
        {
            Customers.Clear();
            Suppliers.Clear();

            foreach (var c in _db.Customers.AsNoTracking().OrderBy(x => x.Name).ToList())
                Customers.Add(c);

            foreach (var s in _db.Suppliers.AsNoTracking().OrderBy(x => x.Name).ToList())
                Suppliers.Add(s);

            SelectedCustomer = Customers.FirstOrDefault();
            SelectedSupplier = Suppliers.FirstOrDefault();
        }

        private void LoadCustomerStatement()
        {
            CustomerRows.Clear();
            if (SelectedCustomer == null) return;

            var invoices = _db.Invoices
                .AsNoTracking()
                .Where(x => x.CustomerId == SelectedCustomer.Id && x.Type == InvoiceType.Valid)
                .Include(x => x.InvoicePayments)
                .OrderBy(x => x.Date)
                .ThenBy(x => x.Id)
                .ToList();

            decimal balance = 0m;
            foreach (var invoice in invoices)
            {
                balance += ToBaseCurrency(invoice.TotalPrice, invoice.Currency, invoice.ExchangeRate);
                CustomerRows.Add(new AccountStatementRow
                {
                    Date = invoice.Date,
                    DocumentNumber = invoice.Number,
                    Description = "فاتورة مبيعات",
                    Debit = invoice.TotalPrice,
                    Currency = invoice.Currency,
                    ExchangeRate = invoice.ExchangeRate,
                    Balance = balance
                });

                foreach (var payment in invoice.InvoicePayments ?? Enumerable.Empty<POS.Domain.Models.Payments.InvoicePayment>())
                {
                    balance -= ToBaseCurrency(payment.Amount, payment.Currency, payment.ExchangeRate);
                    CustomerRows.Add(new AccountStatementRow
                    {
                        Date = payment.Date,
                        DocumentNumber = invoice.Number,
                        Description = "دفعة",
                        Credit = payment.Amount,
                        Currency = payment.Currency,
                        ExchangeRate = payment.ExchangeRate,
                        Balance = balance
                    });
                }
            }

            OnPropertyChanged(nameof(CustomerBalance));
        }

        private void LoadSupplierStatement()
        {
            SupplierRows.Clear();
            if (SelectedSupplier == null) return;

            var purchases = _db.Purchases
                .AsNoTracking()
                .Where(x => x.SupplierId == SelectedSupplier.Id)
                .Include(x => x.PurchasePayments)
                .OrderBy(x => x.Date)
                .ThenBy(x => x.Id)
                .ToList();

            decimal balance = 0m;
            foreach (var purchase in purchases)
            {
                balance += ToBaseCurrency(purchase.TotalPrice, purchase.Currency, purchase.ExchangeRate);
                SupplierRows.Add(new AccountStatementRow
                {
                    Date = purchase.Date,
                    DocumentNumber = purchase.Number,
                    Description = "فاتورة مشتريات",
                    Credit = purchase.TotalPrice,
                    Currency = purchase.Currency,
                    ExchangeRate = purchase.ExchangeRate,
                    Balance = balance
                });

                foreach (var payment in purchase.PurchasePayments ?? Enumerable.Empty<POS.Domain.Models.Payments.PurchasePayment>())
                {
                    balance -= ToBaseCurrency(payment.Amount, payment.Currency, payment.ExchangeRate);
                    SupplierRows.Add(new AccountStatementRow
                    {
                        Date = payment.Date,
                        DocumentNumber = purchase.Number,
                        Description = "دفعة للمورد",
                        Debit = payment.Amount,
                        Currency = payment.Currency,
                        ExchangeRate = payment.ExchangeRate,
                        Balance = balance
                    });
                }
            }

            OnPropertyChanged(nameof(SupplierBalance));
        }

        private static decimal ToBaseCurrency(decimal amount, Currency currency, decimal rate)
        {
            if (currency == Currency.LBP && rate > 0m)
                return amount / rate;
            return amount;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
