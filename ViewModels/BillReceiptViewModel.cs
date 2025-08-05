using System;
using System.Collections.ObjectModel;
using System.Linq;
using VillageSmartPOS.Models;
using VillageSmartPOS.Services;

namespace VillageSmartPOS.ViewModels
{
    public class BillReceiptViewModel : BaseViewModel
    {
        private readonly BillSettingsService _billSettings = BillSettingsService.Instance;

        public string BillNumber { get; set; }
        public string DateTime { get; set; }
        
        // Bill customization properties from database
        public string LogoText => _billSettings.LogoText;
        public string GroceryName => _billSettings.GroceryName;
        public string StoreAddress => _billSettings.StoreAddress;
        public string PhoneNumbers => $"Tel: {_billSettings.PhoneNumber1} / {_billSettings.PhoneNumber2}";
        public string CashierName => $"අය කළේ: {_billSettings.CashierName}";
        public string ReceiptFooter => _billSettings.ReceiptFooter;
        public string CurrencySymbol => _billSettings.CurrencySymbol;

        public ObservableCollection<BillItem> Items { get; set; }

        private decimal totalAmount;
        public decimal TotalAmount 
        { 
            get => totalAmount; 
            set { totalAmount = value; OnPropertyChanged(); OnPropertyChanged(nameof(BalanceAmount)); OnPropertyChanged(nameof(PaymentMethodText)); } 
        }

        private decimal paidAmount;
        public decimal PaidAmount 
        { 
            get => paidAmount; 
            set { paidAmount = value; OnPropertyChanged(); OnPropertyChanged(nameof(BalanceAmount)); OnPropertyChanged(nameof(PaymentMethodText)); } 
        }

        public decimal BalanceAmount => PaidAmount - TotalAmount;

        // Loan payment properties
        private bool isLoanPayment = false;
        public bool IsLoanPayment 
        { 
            get => isLoanPayment; 
            set { isLoanPayment = value; OnPropertyChanged(); OnPropertyChanged(nameof(PaymentMethodText)); OnPropertyChanged(nameof(LoanBalanceText)); OnPropertyChanged(nameof(CustomerInfoText)); } 
        }
        
        public string LoanCustomerId { get; set; } = string.Empty;
        public string LoanCustomerName { get; set; } = string.Empty;
        public decimal OldLoanBalance { get; set; } = 0;
        public decimal NewLoanBalance { get; set; } = 0;

        // Payment method display
        public string PaymentMethodText => IsLoanPayment ? "PAID AS LOAN" : $"PAID: Rs. {PaidAmount:F2}";

        // Loan customer information
        public string CustomerInfoText => $"Customer: {LoanCustomerName} (ID: {LoanCustomerId})";
        public string LoanBalanceText => $"Old Balance: Rs. {OldLoanBalance:F2} | New Balance: Rs. {NewLoanBalance:F2}";

        // Savings calculations
        public decimal TotalSavings => Items?.Sum(item => item.Savings) ?? 0;
        public decimal TotalMarkedAmount => Items?.Sum(item => item.MarkedPrice * item.Quantity) ?? 0;
        public string TotalSavingsText => $"You Saved: Rs. {TotalSavings:F2}";

        public BillReceiptViewModel()
        {
            Items = new ObservableCollection<BillItem>();
            
            // Generate unique bill number and set current date/time
            BillNumber = GenerateBillNumber();
            DateTime = $"දිනය: {System.DateTime.Now:yyyy-MM-dd hh:mm tt}";
            
            // Subscribe to settings saved event
            SettingsViewModel.SettingsSaved += OnSettingsSaved;
        }

        /// <summary>
        /// Generates a unique bill number based on current date and time
        /// Format: YYMMDDHHMMSS (e.g., 241128143052 for 2024-11-28 14:30:52)
        /// </summary>
        private string GenerateBillNumber()
        {
            var now = System.DateTime.Now;
            string billNumber = $"බිල්පත් අංකය: {now:yyMMddHHmmss}";
            return billNumber;
        }

        /// <summary>
        /// Regenerates the bill number and updates the date/time
        /// </summary>
        public void RegenerateBillNumber()
        {
            BillNumber = GenerateBillNumber();
            DateTime = $"දිනය: {System.DateTime.Now:yyyy-MM-dd hh:mm tt}";
            OnPropertyChanged(nameof(BillNumber));
            OnPropertyChanged(nameof(DateTime));
        }

        private void OnSettingsSaved(object? sender, EventArgs e)
        {
            // Refresh bill settings when settings are saved
            RefreshBillSettings();
        }

        public void SetPaymentAmount(decimal amount)
        {
            PaidAmount = amount;
        }

        public void SetLoanPayment(string customerId, string customerName, decimal oldBalance, decimal newBalance)
        {
            IsLoanPayment = true;
            LoanCustomerId = customerId;
            LoanCustomerName = customerName;
            OldLoanBalance = oldBalance;
            NewLoanBalance = newBalance;
        }

        /// <summary>
        /// Refresh bill settings from database
        /// </summary>
        public void RefreshBillSettings()
        {
            // Force property change notifications to refresh the UI
            OnPropertyChanged(nameof(LogoText));
            OnPropertyChanged(nameof(GroceryName));
            OnPropertyChanged(nameof(StoreAddress));
            OnPropertyChanged(nameof(PhoneNumbers));
            OnPropertyChanged(nameof(CashierName));
            OnPropertyChanged(nameof(ReceiptFooter));
        }
    }
}
