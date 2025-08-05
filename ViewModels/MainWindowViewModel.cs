using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using VillageSmartPOS.Helpers;
using VillageSmartPOS.Views;

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

