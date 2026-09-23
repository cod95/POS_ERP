using System.Windows.Controls;
using POS.ViewModels;

namespace POS.CustomControl
{
    public partial class CurrencyRates_UserControl : UserControl
    {
        public CurrencyRates_UserControl()
        {
            InitializeComponent();
            DataContext = new CurrencyRateViewModel();
        }
    }
}