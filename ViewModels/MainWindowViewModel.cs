using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using VillageSmartPOS.Helpers;
using VillageSmartPOS.Views;
using VillageSmartPOS.ViewModels;

namespace VillageSmartPOS.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private UserControl currentView = null!;
        public UserControl CurrentView
        {
            get => currentView;
            set
            {
                currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToSalesBillingCommand { get; }
        public ICommand NavigateToAddProductCommand { get; }
        public ICommand NavigateToInventoryCommand { get; }
        public ICommand NavigateToLoanCustomersCommand { get; }
        public ICommand NavigateToTemporaryBalanceCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }

        public MainWindowViewModel()
        {
            // Default page
            CurrentView = new SalesBillingPage();
            CurrentView.DataContext = new SalesBillingViewModel();

            NavigateToSalesBillingCommand = new RelayCommand(ShowSalesBillingPage);
            NavigateToAddProductCommand = new RelayCommand(ShowAddProductPage);
            NavigateToInventoryCommand = new RelayCommand(ShowInventoryPage);
            NavigateToLoanCustomersCommand = new RelayCommand(ShowLoanCustomerPage);
            NavigateToTemporaryBalanceCommand = new RelayCommand(ShowTemporaryBalancePage);
            NavigateToSettingsCommand = new RelayCommand(ShowSettingsPage);
        }

        private void ShowSalesBillingPage()
        {
            CurrentView = new SalesBillingPage();
            CurrentView.DataContext = new SalesBillingViewModel();
        }

        private void ShowAddProductPage()
        {
            // You can create and replace with your AddProductPage.xaml
            CurrentView = new AddProductPage(); // Make sure this exists
        }

        private void ShowInventoryPage()
        {
            CurrentView = new InventoryManagementPage();
        }

        private void ShowLoanCustomerPage()
        {
            CurrentView = new LoanCustomerManagementPage();
        }

        private void ShowTemporaryBalancePage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== DEBUG: Starting ShowTemporaryBalancePage ===");
                System.Diagnostics.Debug.WriteLine("Creating TemporaryBalancePage...");
                
                CurrentView = new TemporaryBalancePage();
                System.Diagnostics.Debug.WriteLine("TemporaryBalancePage created successfully");
                
                System.Diagnostics.Debug.WriteLine("Creating TemporaryBalanceViewModel...");
                CurrentView.DataContext = new TemporaryBalanceViewModel();
                System.Diagnostics.Debug.WriteLine("TemporaryBalanceViewModel created and set as DataContext");
                
                System.Diagnostics.Debug.WriteLine("=== DEBUG: ShowTemporaryBalancePage completed successfully ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== ERROR in ShowTemporaryBalancePage ===");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"=== END ERROR ===");
                
                // Show error to user
                System.Windows.MessageBox.Show($"Error loading Temporary Balance page: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ShowSettingsPage()
        {
            CurrentView = new SettingsPage();
            CurrentView.DataContext = new SettingsViewModel();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

