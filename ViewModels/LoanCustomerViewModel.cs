using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using VillageSmartPOS.Models;
using VillageSmartPOS.Services;
using VillageSmartPOS.Helpers;

namespace VillageSmartPOS.ViewModels
{
    public class LoanCustomerViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;
        private string _searchTerm = string.Empty;
        private LoanCustomer? _selectedCustomer;
        private bool _isEditing = false;

        // Form properties
        private string _customerId = string.Empty;
        private string _name = string.Empty;
        private string _phone = string.Empty;
        private string _address = string.Empty;
        private decimal _repaymentAmount = 0;

        public ObservableCollection<LoanCustomer> Customers { get; set; }
        public ObservableCollection<LoanTransaction> Transactions { get; set; }

        public LoanCustomerViewModel()
        {
            _dbService = new DatabaseService();
            Customers = new ObservableCollection<LoanCustomer>();
            Transactions = new ObservableCollection<LoanTransaction>();
            
            LoadCustomers();
        }

        // Search functionality
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
                SearchCustomers();
            }
        }

        // Selected customer
        public LoanCustomer? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
                if (_selectedCustomer != null)
                {
                    LoadCustomerDetails();
                }
            }
        }

        // Form properties
        public string CustomerId
        {
            get => _customerId;
            set
            {
                _customerId = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        public decimal RepaymentAmount
        {
            get => _repaymentAmount;
            set
            {
                _repaymentAmount = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand AddCustomerCommand => new RelayCommand(AddCustomer);
        public ICommand UpdateCustomerCommand => new RelayCommand(UpdateCustomer, CanUpdateCustomer);
        public ICommand DeleteCustomerCommand => new RelayCommand(DeleteCustomer, CanDeleteCustomer);
        public ICommand ClearFormCommand => new RelayCommand(ClearForm);
        public ICommand LoadDetailsCommand => new RelayCommand(LoadCustomerDetails, CanLoadDetails);
        public ICommand AddRepaymentCommand => new RelayCommand(AddRepayment, CanAddRepayment);
        public ICommand CleanupTransactionsCommand => new RelayCommand(CleanupTransactions);


        private void LoadCustomers()
        {
            try
            {
                Customers.Clear();
                var customers = _dbService.GetAllLoanCustomers();
                foreach (var customer in customers)
                {
                    Customers.Add(customer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchCustomers()
        {
            try
            {
                Customers.Clear();
                var customers = string.IsNullOrEmpty(SearchTerm) 
                    ? _dbService.GetAllLoanCustomers() 
                    : _dbService.SearchLoanCustomers(SearchTerm);
                
                foreach (var customer in customers)
                {
                    Customers.Add(customer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddCustomer()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CustomerId) || string.IsNullOrWhiteSpace(Name))
                {
                    MessageBox.Show("Customer ID and Name are required!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _dbService.AddLoanCustomer(CustomerId, Name, Phone, Address);
                MessageBox.Show("Customer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                ClearForm();
                LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateCustomer()
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    MessageBox.Show("Please select a customer to update!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(CustomerId) || string.IsNullOrWhiteSpace(Name))
                {
                    MessageBox.Show("Customer ID and Name are required!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _dbService.UpdateLoanCustomer(SelectedCustomer.Id, CustomerId, Name, Phone, Address);
                MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                ClearForm();
                LoadCustomers();
                IsEditing = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteCustomer()
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    MessageBox.Show("Please select a customer to delete!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to delete customer '{SelectedCustomer.Name}'?", 
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    _dbService.DeleteLoanCustomer(SelectedCustomer.Id);
                    MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    ClearForm();
                    LoadCustomers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            CustomerId = string.Empty;
            Name = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
            RepaymentAmount = 0;
            SelectedCustomer = null;
            IsEditing = false;
            Transactions.Clear();
        }

        private void LoadCustomerDetails()
        {
            if (SelectedCustomer == null) return;

            CustomerId = SelectedCustomer.CustomerId;
            Name = SelectedCustomer.Name;
            Phone = SelectedCustomer.Phone;
            Address = SelectedCustomer.Address;
            IsEditing = true;

            // Load transactions
            LoadTransactions();
            
            // Debug: Force UI update
            OnPropertyChanged(nameof(Transactions));
        }

        private void LoadTransactions()
        {
            try
            {
                if (SelectedCustomer == null) return;

                Transactions.Clear();
                var transactions = _dbService.GetLoanTransactions(SelectedCustomer.CustomerId);
                
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }
                
                // Force UI update
                OnPropertyChanged(nameof(Transactions));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transactions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddRepayment()
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    MessageBox.Show("Please select a customer!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (RepaymentAmount <= 0)
                {
                    MessageBox.Show("Repayment amount must be greater than 0!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (RepaymentAmount > SelectedCustomer.CurrentBalance)
                {
                    MessageBox.Show("Repayment amount cannot exceed current balance!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var oldBalance = SelectedCustomer.CurrentBalance;
                var newBalance = oldBalance - RepaymentAmount;

                _dbService.AddLoanTransaction(
                    SelectedCustomer.CustomerId,
                    TransactionType.REPAYMENT,
                    RepaymentAmount,
                    oldBalance,
                    newBalance,
                    "",
                    "Manual repayment"
                );

                MessageBox.Show("Repayment added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                RepaymentAmount = 0;
                LoadCustomers(); // Refresh customer list to update balances
                LoadTransactions(); // Refresh transaction list
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding repayment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Command can-execute methods
        private bool CanUpdateCustomer() => SelectedCustomer != null && IsEditing;
        private bool CanDeleteCustomer() => SelectedCustomer != null;
        private bool CanLoadDetails() => SelectedCustomer != null;
        private bool CanAddRepayment() => SelectedCustomer != null && RepaymentAmount > 0 && RepaymentAmount <= (SelectedCustomer?.CurrentBalance ?? 0);

        private void CleanupTransactions()
        {
            try
            {
                var result = MessageBox.Show(
                    "This will delete old transactions for all customers, keeping only the last 10 transactions per customer.\n\n" +
                    "This action will NOT affect the current balance of any customer.\n\n" +
                    "Do you want to continue?",
                    "Cleanup Old Transactions",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _dbService.CleanupAllOldTransactions();
                    MessageBox.Show(
                        "Old transactions have been cleaned up successfully!\n\n" +
                        "Only the last 10 transactions per customer are now kept.",
                        "Cleanup Complete",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during cleanup: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




    }
} 