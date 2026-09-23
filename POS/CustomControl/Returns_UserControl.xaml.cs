using System.Windows.Controls;
using POS.ViewModels;

namespace POS.CustomControl
{
    public partial class Returns_UserControl : UserControl
    {
        public Returns_UserControl()
        {
            InitializeComponent();
            DataContext = new ReturnsViewModel();
        }
    }
}