using System.Windows.Controls;
using System.Windows.Input;

namespace VillageSmartPOS.Views
{
    /// <summary>
    /// Interaction logic for TemporaryBalancePage.xaml
    /// </summary>
    public partial class TemporaryBalancePage : UserControl
    {
        public TemporaryBalancePage()
        {
            InitializeComponent();
        }

        private void CustomerNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                BalanceTextBox.Focus();
            }
        }

        private void BalanceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                NotesTextBox.Focus();
            }
        }

        private void NotesTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                // Try to add the balance if the form is valid
                if (DataContext is ViewModels.TemporaryBalanceViewModel viewModel)
                {
                    if (viewModel.AddBalanceCommand.CanExecute(null))
                    {
                        viewModel.AddBalanceCommand.Execute(null);
                    }
                }
            }
        }
    }
} 