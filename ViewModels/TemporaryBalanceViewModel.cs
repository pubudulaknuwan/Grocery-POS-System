using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VillageSmartPOS.Helpers;
using VillageSmartPOS.Models;
using VillageSmartPOS.Services;

namespace VillageSmartPOS.ViewModels
{
    public class TemporaryBalanceViewModel : BaseViewModel
    {
        private readonly DatabaseService dbService = new();

        // Properties for the form
        private string customerName = string.Empty;
        public string CustomerName
        {
            get => customerName;
            set { customerName = value; OnPropertyChanged(); }
        }

        private string balanceText = string.Empty;
        public string BalanceText
        {
            get => balanceText;
            set { balanceText = value; OnPropertyChanged(); }
        }

        private string notes = string.Empty;
        public string Notes
        {
            get => notes;
            set { notes = value; OnPropertyChanged(); }
        }

        private string searchTerm = string.Empty;
        public string SearchTerm
        {
            get => searchTerm;
            set 
            { 
                searchTerm = value; 
                OnPropertyChanged();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SearchBalances();
                }
                else
                {
                    LoadAllBalances();
                }
            }
        }

        private string statusMessage = string.Empty;
        public string StatusMessage
        {
            get => statusMessage;
            set { statusMessage = value; OnPropertyChanged(); }
        }

        // Selected balance for editing
        private TemporaryBalance? selectedBalance;
        public TemporaryBalance? SelectedBalance
        {
            get => selectedBalance;
            set 
            { 
                selectedBalance = value; 
                OnPropertyChanged();
                if (selectedBalance != null)
                {
                    LoadBalanceForEditing();
                }
            }
        }

        // Collections
        public ObservableCollection<TemporaryBalance> Balances { get; set; } = new();

        // Commands
        public ICommand AddBalanceCommand { get; }
        public ICommand UpdateBalanceCommand { get; }
        public ICommand DeleteBalanceCommand { get; }
        public ICommand ClearFormCommand { get; }
        public ICommand SearchCommand { get; }

        public TemporaryBalanceViewModel()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== DEBUG: TemporaryBalanceViewModel constructor started ===");
                
                System.Diagnostics.Debug.WriteLine("Initializing commands...");
                AddBalanceCommand = new RelayCommand(AddBalance, CanAddBalance);
                UpdateBalanceCommand = new RelayCommand(UpdateBalance, CanUpdateBalance);
                DeleteBalanceCommand = new RelayCommand(DeleteBalance, CanDeleteBalance);
                ClearFormCommand = new RelayCommand(ClearForm);
                SearchCommand = new RelayCommand(SearchBalances);
                System.Diagnostics.Debug.WriteLine("Commands initialized successfully");

                System.Diagnostics.Debug.WriteLine("Loading all balances...");
                LoadAllBalances();
                System.Diagnostics.Debug.WriteLine("LoadAllBalances completed");
                
                System.Diagnostics.Debug.WriteLine("=== DEBUG: TemporaryBalanceViewModel constructor completed successfully ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== ERROR in TemporaryBalanceViewModel constructor ===");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"=== END ERROR ===");
                throw; // Re-throw to see the error
            }
        }

        private bool CanAddBalance()
        {
            return !string.IsNullOrWhiteSpace(CustomerName) && 
                   !string.IsNullOrWhiteSpace(BalanceText) &&
                   decimal.TryParse(BalanceText, out decimal balance) && 
                   balance > 0;
        }

        private bool CanUpdateBalance()
        {
            return SelectedBalance != null && 
                   !string.IsNullOrWhiteSpace(CustomerName) && 
                   !string.IsNullOrWhiteSpace(BalanceText) &&
                   decimal.TryParse(BalanceText, out decimal balance) && 
                   balance > 0;
        }

        private bool CanDeleteBalance()
        {
            return SelectedBalance != null;
        }

        private void AddBalance()
        {
            if (!CanAddBalance())
            {
                StatusMessage = "Please enter valid customer name and balance";
                return;
            }

            if (!decimal.TryParse(BalanceText, out decimal balance))
            {
                StatusMessage = "Please enter a valid balance amount";
                return;
            }

            try
            {
                dbService.AddTemporaryBalance(CustomerName.Trim(), balance, Notes.Trim());
                StatusMessage = $"Added temporary balance for {CustomerName} - Rs. {balance:F2}";
                ClearForm();
                LoadAllBalances();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error adding balance: {ex.Message}";
            }
        }

        private void UpdateBalance()
        {
            if (!CanUpdateBalance())
            {
                StatusMessage = "Please select a balance to update and enter valid data";
                return;
            }

            if (!decimal.TryParse(BalanceText, out decimal balance))
            {
                StatusMessage = "Please enter a valid balance amount";
                return;
            }

            try
            {
                dbService.UpdateTemporaryBalance(SelectedBalance!.Id, CustomerName.Trim(), balance, Notes.Trim());
                StatusMessage = $"Updated balance for {CustomerName} - Rs. {balance:F2}";
                ClearForm();
                LoadAllBalances();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error updating balance: {ex.Message}";
            }
        }

        private void DeleteBalance()
        {
            if (SelectedBalance == null)
            {
                StatusMessage = "Please select a balance to delete";
                return;
            }

            try
            {
                dbService.DeleteTemporaryBalance(SelectedBalance.Id);
                StatusMessage = $"Deleted balance for {SelectedBalance.CustomerName}";
                ClearForm();
                LoadAllBalances();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting balance: {ex.Message}";
            }
        }

        private void ClearForm()
        {
            CustomerName = string.Empty;
            BalanceText = string.Empty;
            Notes = string.Empty;
            SelectedBalance = null;
            StatusMessage = "Form cleared";
        }

        private void LoadBalanceForEditing()
        {
            if (SelectedBalance != null)
            {
                CustomerName = SelectedBalance.CustomerName;
                BalanceText = SelectedBalance.Balance.ToString("F2");
                Notes = SelectedBalance.Notes ?? "";
                StatusMessage = $"Editing balance for {SelectedBalance.CustomerName}";
            }
        }

        private void LoadAllBalances()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== DEBUG: LoadAllBalances started ===");
                System.Diagnostics.Debug.WriteLine("Calling dbService.GetAllTemporaryBalances()...");
                
                var balances = dbService.GetAllTemporaryBalances();
                System.Diagnostics.Debug.WriteLine($"Database returned {balances.Count} balances");
                
                System.Diagnostics.Debug.WriteLine("Clearing existing balances...");
                Balances.Clear();
                
                System.Diagnostics.Debug.WriteLine("Adding balances to collection...");
                foreach (var balance in balances)
                {
                    try
                    {
                        Balances.Add(balance);
                        System.Diagnostics.Debug.WriteLine($"Added balance: {balance.CustomerName} - {balance.FormattedBalance}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"ERROR adding balance to collection: {ex.Message}");
                        // Continue with next balance instead of crashing
                    }
                }
                
                StatusMessage = $"Loaded {Balances.Count} temporary balances";
                System.Diagnostics.Debug.WriteLine($"Status message set: {StatusMessage}");
                System.Diagnostics.Debug.WriteLine("=== DEBUG: LoadAllBalances completed successfully ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== ERROR in LoadAllBalances ===");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"=== END ERROR ===");
                StatusMessage = $"Error loading balances: {ex.Message}";
                
                // Show error to user but don't crash
                System.Windows.MessageBox.Show($"Error loading balances: {ex.Message}", "Database Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }

        private void SearchBalances()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== DEBUG: SearchBalances started with term: '{SearchTerm}' ===");
                
                if (string.IsNullOrWhiteSpace(SearchTerm))
                {
                    System.Diagnostics.Debug.WriteLine("Search term is empty, loading all balances");
                    LoadAllBalances();
                    return;
                }

                System.Diagnostics.Debug.WriteLine("Calling dbService.SearchTemporaryBalances()...");
                var balances = dbService.SearchTemporaryBalances(SearchTerm);
                System.Diagnostics.Debug.WriteLine($"Search returned {balances.Count} balances");
                
                Balances.Clear();
                foreach (var balance in balances)
                {
                    try
                    {
                        Balances.Add(balance);
                        System.Diagnostics.Debug.WriteLine($"Added search result: {balance.CustomerName} - {balance.FormattedBalance}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"ERROR adding search result to collection: {ex.Message}");
                        // Continue with next balance
                    }
                }
                StatusMessage = $"Found {Balances.Count} balances matching '{SearchTerm}'";
                System.Diagnostics.Debug.WriteLine($"=== DEBUG: SearchBalances completed successfully ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== ERROR in SearchBalances ===");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"=== END ERROR ===");
                StatusMessage = $"Error searching balances: {ex.Message}";
                
                // Show error to user but don't crash
                System.Windows.MessageBox.Show($"Error searching balances: {ex.Message}", "Search Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }
    }
} 