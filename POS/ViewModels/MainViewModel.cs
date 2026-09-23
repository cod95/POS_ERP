using POS.CustomControl;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POS.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _storeName = "ARP-POS";
        private string _pageTitle = "لوحة التحكم";
        private object _currentView;

        public string StoreName { get => _storeName; set { if (_storeName != value) { _storeName = value; OnPropertyChanged(nameof(StoreName)); } } }
        public string PageTitle { get => _pageTitle; set { if (_pageTitle != value) { _pageTitle = value; OnPropertyChanged(nameof(PageTitle)); } } }
        public object CurrentView { get => _currentView; private set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); } }

        public ICommand DashboardCommand { get; }
        public ICommand POSCommand { get; }
        public ICommand SalesCommand { get; }
        public ICommand PurchaseCommand { get; }
        public ICommand InventoryCommand { get; }
        public ICommand MovingProductsCommand { get; }
        public ICommand CustomersCommand { get; }
        public ICommand SuppliersCommand { get; }
        public ICommand QuotationsCommand { get; }
        public ICommand UsersCommand { get; }
        public ICommand RolesCommand { get; }
        public ICommand CompanyCommand { get; }
        public ICommand CurrencyRatesCommand { get; }
        public ICommand AccountStatementsCommand { get; }
        public ICommand ReturnsCommand { get; }
        public ICommand ManufacturingCommand { get; }

        public MainViewModel()
        {
            DashboardCommand = new RelayCommand(_ => ShowDashboard());
            POSCommand = new RelayCommand(_ => OpenView("نقطة البيع", () => new POS_UserControl()));
            SalesCommand = new RelayCommand(_ => OpenView("المبيعات", () => new SalesHistory_UserControl()));
            PurchaseCommand = new RelayCommand(_ => OpenView("المشتريات", () => new Purchase_Products_UserControl()));
            InventoryCommand = new RelayCommand(_ => OpenView("الأصناف والمخزون", () => new Inventory_UserControl()));
            MovingProductsCommand = new RelayCommand(_ => OpenView("تحويل المخزون", () => new Moving_Products_UserControl()));
            CustomersCommand = new RelayCommand(_ => OpenView("العملاء", () => new Customer_Add_UserControl()));
            SuppliersCommand = new RelayCommand(_ => OpenView("الموردون", () => new Supplier_Add_UserControl()));
            QuotationsCommand = new RelayCommand(_ => OpenView("عروض الأسعار", () => new PriceQuotation_UserControl()));
            UsersCommand = new RelayCommand(_ => OpenView("المستخدمون", () => new Users_UserControl()));
            RolesCommand = new RelayCommand(_ => OpenView("الصلاحيات", () => new Roles_UserControl()));
            CompanyCommand = new RelayCommand(_ => OpenView("بيانات الشركة والإعدادات", () => new CompanyInfo_UserControl()));
            CurrencyRatesCommand = new RelayCommand(_ => OpenView("العملات وأسعار الصرف", () => new CurrencyRates_UserControl()));
            AccountStatementsCommand = new RelayCommand(_ => OpenView("حسابات العملاء والموردين", () => new AccountStatements_UserControl()));
            ReturnsCommand = new RelayCommand(_ => OpenView("مرتجعات المبيعات والمشتريات", () => new Returns_UserControl()));
            ManufacturingCommand = new RelayCommand(_ => OpenView("التصنيع", () => new Manufacturing_UserControl()));
            ShowDashboard();
        }

        private void ShowDashboard()
        {
            PageTitle = "لوحة التحكم";
            CurrentView = null;
        }

        private void OpenView(string title, Func<UserControl> factory)
        {
            try
            {
                var view = factory();
                PageTitle = title;
                CurrentView = view;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"تعذر فتح شاشة «{title}».\n\n{ex.Message}",
                    "خطأ في الشاشة",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        public event EventHandler CanExecuteChanged { add => CommandManager.RequerySuggested += value; remove => CommandManager.RequerySuggested -= value; }
        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => _execute(parameter);
    }
}