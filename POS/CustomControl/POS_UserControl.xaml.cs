using POS.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POS.CustomControl
{
    public partial class POS_UserControl : UserControl
    {
        private readonly POSViewModel viewModel;

        public POS_UserControl()
        {
            InitializeComponent();
            viewModel = new POSViewModel();
            DataContext = viewModel;
        }

        private void BarcodeSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchByBarcode();
                e.Handled = true;
            }
        }

        private void SearchBarcode_Click(object sender, RoutedEventArgs e) => SearchByBarcode();

        private void SearchByBarcode()
        {
            var barcode = BarcodeSearchBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(barcode)) return;

            var product = viewModel.ProductList.FirstOrDefault(p =>
                string.Equals(p.Barcode?.Trim(), barcode, StringComparison.OrdinalIgnoreCase));

            if (product == null)
            {
                MessageBox.Show("لم يتم العثور على صنف بهذا الباركود.", "البحث", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            viewModel.SelectedProduct = product;
            BarcodeSearchBox.SelectAll();
        }

        private void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SelectedProduct = null;
            viewModel.Barcode = string.Empty;
            BarcodeSearchBox.Clear();
        }
    }
}