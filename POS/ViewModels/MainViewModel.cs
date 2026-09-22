using POS.CustomControl;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace POS.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _storeName = "ARP-POS";
        private string _pageTitle = "لوحة التحكم";
        private object _currentView;

        public string StoreName
        {
            get => _storeName;
            set { if (_storeName != value) { _storeName = value; OnPropertyChanged(nameof(StoreName)); } }
        }

        public string PageTitle
        {
            get => _pageTitle;
            set { if (_pageTitle != value) { _pageTitle = value; OnPropertyChanged(nameof(PageTitle)); } }
        }

        public object CurrentView
        {
            get => _currentView;
            private set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

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
            POSCommand = new RelayCommand(_ => Show("نقطة البيع", new POS_UserControl()));
            SalesCommand = new RelayCommand(_ => Show("المبيعات", new SalesHistory_UserControl()));
            PurchaseCommand = new RelayCommand(_ => Show("المشتريات", new Purchase_Products_UserControl()));
            InventoryCommand = new RelayCommand(_ => Show("الأصناف والمخزون", new Inventory_UserControl()));
            MovingProductsCommand = new RelayCommand(_ => Show("تحويل المخزون", new Moving_Products_UserControl()));
            CustomersCommand = new RelayCommand(_ => Show("العملاء", new Customer_Add_UserControl()));
            SuppliersCommand = new RelayCommand(_ => Show("الموردون", new Supplier_Add_UserControl()));
            QuotationsCommand = new RelayCommand(_ => Show("عروض الأسعار", new PriceQuotation_UserControl()));
            UsersCommand = new RelayCommand(_ => Show("المستخدمون", new Users_UserControl()));
            RolesCommand = new RelayCommand(_ => Show("الصلاحيات", new Roles_UserControl()));
            CompanyCommand = new RelayCommand(_ => Show("بيانات الشركة والإعدادات", new CompanyInfo_UserControl()));
            CurrencyRatesCommand = new RelayCommand(_ => Show("العملات وأسعار الصرف", new CurrencyRates_UserControl()));
            AccountStatementsCommand = new RelayCommand(_ => Show("حسابات العملاء والموردين", new AccountStatements_UserControl()));
            ReturnsCommand = new RelayCommand(_ => Show("مرتجعات المبيعات والمشتريات", new Returns_UserControl()));
            ManufacturingCommand = new RelayCommand(_ => Show("التصنيع", new Manufacturing_UserControl()));

            ShowDashboard();
        }

        private void ShowDashboard()
        {
            PageTitle = "لوحة التحكم";
            CurrentView = null;
        }

        private void Show(string title, UserControl view)
        {
            PageTitle = title;
            CurrentView = view;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => _execute(parameter);
    }
}
