using System.Windows.Controls;
using POS.ViewModels;

namespace POS.CustomControl
{
    public partial class AccountStatements_UserControl : UserControl
    {
        public AccountStatements_UserControl()
        {
            InitializeComponent();
            DataContext = new AccountStatementViewModel();
        }
    }
}