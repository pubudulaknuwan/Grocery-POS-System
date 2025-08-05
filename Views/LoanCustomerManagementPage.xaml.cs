using System.Windows.Controls;
using VillageSmartPOS.ViewModels;

namespace VillageSmartPOS.Views
{
    public partial class LoanCustomerManagementPage : UserControl
    {
        public LoanCustomerManagementPage()
        {
            InitializeComponent();
            DataContext = new LoanCustomerViewModel();
        }
    }
} 