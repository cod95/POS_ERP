using POS.ViewModels;
using System.Windows.Controls;

namespace POS.CustomControl
{
    public partial class Inventory_UserControl : UserControl
    {
        private readonly InventoryViewModel viewModel;

        public Inventory_UserControl()
        {
            InitializeComponent();
            viewModel = new InventoryViewModel();
            DataContext = viewModel;
        }
    }
}