using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using VillageSmartPOS.ViewModels;
using VillageSmartPOS.Models;
using VillageSmartPOS.Services;
using VillageSmartPOS.Helpers;

namespace VillageSmartPOS.Views
{
    public partial class PaymentDialog : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly DatabaseService _dbService;
        private LoanCustomer? _selectedLoanCustomer;

        // Payment type tracking
        public enum PaymentType
        {
            Cash,
            Loan
        }

        // Basic payment properties
        private decimal totalAmount;
        public decimal TotalAmount 
        { 
            get => totalAmount; 
            set { totalAmount = value; OnPropertyChanged(); } 
        }

        private decimal paidAmount;
        public decimal PaidAmount 
        { 
            get => paidAmount; 
            set { paidAmount = value; OnPropertyChanged(); OnPropertyChanged(nameof(BalanceAmount)); } 
        }

        public decimal BalanceAmount => PaidAmount - TotalAmount;
        public Brush BalanceColor => BalanceAmount >= 0 ? Brushes.Green : Brushes.Red;

        // Loan customer properties
        private string loanCustomerId = string.Empty;
        public string LoanCustomerId
        {
            get => loanCustomerId;
            set
            {
                loanCustomerId = value;
                OnPropertyChanged();
                LookupCustomer();
            }
        }

        public string CustomerName => _selectedLoanCustomer?.Name ?? "";
        public string CustomerPhone => _selectedLoanCustomer?.PhoneDisplay ?? "";
        public string CustomerAddress => _selectedLoanCustomer?.AddressDisplay ?? "";
        public string CurrentBalanceText => _selectedLoanCustomer != null ? $"Current Balance: ${_selectedLoanCustomer.CurrentBalance:F2}" : "";
        public string NewBalanceText => _selectedLoanCustomer != null ? $"New Balance: ${_selectedLoanCustomer.CurrentBalance + TotalAmount:F2}" : "";

        public System.Windows.Visibility CustomerDetailsVisibility => 
            _selectedLoanCustomer != null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        public System.Windows.Visibility PayAsLoanButtonVisibility => 
            _selectedLoanCustomer != null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        public string PaymentStatusText => _selectedLoanCustomer != null 
            ? $"Loan customer found: {_selectedLoanCustomer.Name}" 
            : "Enter customer ID for loan payment or use cash payment below";

        // Commands
        public ICommand PayAsLoanCommand => new RelayCommand(PayAsLoan, CanPayAsLoan);
        public ICommand ConfirmCashPaymentCommand => new RelayCommand(ConfirmCashPayment, CanConfirmCashPayment);

        public PaymentType SelectedPaymentType { get; set; } = PaymentType.Cash;
        public bool IsLoanPayment => SelectedPaymentType == PaymentType.Loan;
        public string PaymentMethodText => IsLoanPayment ? "PAID AS LOAN" : $"PAID: ${PaidAmount:F2}";

        public PaymentDialog(decimal totalAmount)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            TotalAmount = totalAmount;
            PaidAmount = totalAmount; // Default to exact amount
            DataContext = this;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LookupCustomer()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LoanCustomerId))
                {
                    _selectedLoanCustomer = null;
                    OnPropertyChanged(nameof(CustomerName));
                    OnPropertyChanged(nameof(CustomerPhone));
                    OnPropertyChanged(nameof(CustomerAddress));
                    OnPropertyChanged(nameof(CurrentBalanceText));
                    OnPropertyChanged(nameof(NewBalanceText));
                    OnPropertyChanged(nameof(CustomerDetailsVisibility));
                    OnPropertyChanged(nameof(PayAsLoanButtonVisibility));
                    OnPropertyChanged(nameof(PaymentStatusText));
                    return;
                }

                _selectedLoanCustomer = _dbService.GetLoanCustomerById(LoanCustomerId);
                
                OnPropertyChanged(nameof(CustomerName));
                OnPropertyChanged(nameof(CustomerPhone));
                OnPropertyChanged(nameof(CustomerAddress));
                OnPropertyChanged(nameof(CurrentBalanceText));
                OnPropertyChanged(nameof(NewBalanceText));
                OnPropertyChanged(nameof(CustomerDetailsVisibility));
                OnPropertyChanged(nameof(PayAsLoanButtonVisibility));
                OnPropertyChanged(nameof(PaymentStatusText));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error looking up customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PayAsLoan()
        {
            try
            {
                if (_selectedLoanCustomer == null)
                {
                    MessageBox.Show("Please enter a valid customer ID!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var oldBalance = _selectedLoanCustomer.CurrentBalance;
                var newBalance = oldBalance + TotalAmount;

                // Set payment type to loan
                SelectedPaymentType = PaymentType.Loan;
                PaidAmount = 0; // No cash received for loan payment

                MessageBox.Show($"Payment processed as loan for {_selectedLoanCustomer.Name}. New balance: ${newBalance:F2}", 
                    "Loan Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing loan payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConfirmCashPayment()
        {
            try
            {
                if (PaidAmount < TotalAmount)
                {
                    MessageBox.Show("Paid amount cannot be less than total amount!", "Invalid Payment", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SelectedPaymentType = PaymentType.Cash;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing cash payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanPayAsLoan() => _selectedLoanCustomer != null;
        private bool CanConfirmCashPayment() => PaidAmount >= TotalAmount;

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 