using System.Windows;
using System.Windows.Controls;
using System.Printing;
using VillageSmartPOS.ViewModels;
using VillageSmartPOS.Services;
using VillageSmartPOS.Models;
using System;

namespace VillageSmartPOS.Views
{
    public partial class BillReceiptPreview : Window
    {
        private UserControl _receiptView;
        private BillReceiptViewModel _receiptViewModel;
        private readonly DatabaseService _dbService;

        public BillReceiptPreview(UserControl receiptView, BillReceiptViewModel receiptViewModel)
        {
            InitializeComponent();
            _receiptView = receiptView;
            _receiptViewModel = receiptViewModel;
            _dbService = new DatabaseService();
            ReceiptContent.Content = _receiptView;
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show payment dialog first
                PaymentDialog paymentDialog = new PaymentDialog(_receiptViewModel.TotalAmount);
                paymentDialog.Owner = this;
                
                if (paymentDialog.ShowDialog() == true)
                {
                    if (paymentDialog.SelectedPaymentType == PaymentDialog.PaymentType.Loan)
                    {
                        // Handle loan payment
                        HandleLoanPayment(paymentDialog);
                    }
                    else
                    {
                        // Handle cash payment
                        _receiptViewModel.SetPaymentAmount(paymentDialog.PaidAmount);
                        
                        // Force UI refresh
                        _receiptView.DataContext = null;
                        _receiptView.DataContext = _receiptViewModel;
                    }
                    
                    // Automatically print to POS-80 printer without showing printer selection dialog
                    PrintToPOS80Printer();
                    
                    string paymentType = paymentDialog.SelectedPaymentType == PaymentDialog.PaymentType.Loan ? "Loan" : "Cash";
                    MessageBox.Show($"Receipt printed successfully to default printer! Payment: {paymentType}", "Print Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error printing receipt: {ex.Message}", "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintToPOS80Printer()
        {
            try
            {
                // Create print dialog but don't show it
                PrintDialog printDialog = new PrintDialog();
                
                // Create PrintServer instance
                PrintServer printServer = new PrintServer();
                
                // Get default printer from settings
                var settings = _dbService.GetAllBillSettings();
                string defaultPrinter = settings.GetValueOrDefault("default_printer", "POS-80");
                
                // Try to find and select the default printer
                bool printerFound = false;
                foreach (var printer in printServer.GetPrintQueues())
                {
                    if (printer.Name.Equals(defaultPrinter, StringComparison.OrdinalIgnoreCase))
                    {
                        printDialog.PrintQueue = printer;
                        printerFound = true;
                        break;
                    }
                }
                
                // If default printer not found, try to find any thermal printer
                if (!printerFound)
                {
                    foreach (var printer in printServer.GetPrintQueues())
                    {
                        if (printer.Name.Contains("thermal", StringComparison.OrdinalIgnoreCase) ||
                            printer.Name.Contains("receipt", StringComparison.OrdinalIgnoreCase) ||
                            printer.Name.Contains("80mm", StringComparison.OrdinalIgnoreCase) ||
                            printer.Name.Contains("POS-80", StringComparison.OrdinalIgnoreCase) ||
                            printer.Name.Contains("POS80", StringComparison.OrdinalIgnoreCase) ||
                            printer.Name.Contains("POS_80", StringComparison.OrdinalIgnoreCase))
                        {
                            printDialog.PrintQueue = printer;
                            printerFound = true;
                            break;
                        }
                    }
                }
                
                // Set page size for 80mm thermal printer (approximately 72mm printable width)
                // 80mm = 3.15 inches, but printable area is typically around 72mm (2.83 inches)
                double thermalPrinterWidth = 2.83 * 96; // Convert inches to DPI units (96 DPI)
                double thermalPrinterHeight = 11.0 * 96; // Standard receipt height
                
                Size pageSize = new Size(thermalPrinterWidth, thermalPrinterHeight);
                
                // Measure and arrange the receipt for thermal printer dimensions
                _receiptView.Measure(pageSize);
                _receiptView.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
                
                // Print directly without showing dialog
                printDialog.PrintVisual(_receiptView, "VillageSmartPOS Bill");
            }
            catch (System.Exception ex)
            {
                // If automatic printing fails, fall back to manual printer selection
                MessageBox.Show($"Automatic printing failed: {ex.Message}\n\nPlease select your printer manually.", 
                    "Print Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                
                // Show manual printer selection as fallback
                PrintDialog manualPrintDialog = new PrintDialog();
                if (manualPrintDialog.ShowDialog() == true)
                {
                    Size pageSize = new Size(manualPrintDialog.PrintableAreaWidth, manualPrintDialog.PrintableAreaHeight);
                    _receiptView.Measure(pageSize);
                    _receiptView.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
                    manualPrintDialog.PrintVisual(_receiptView, "VillageSmartPOS Bill");
                }
            }
        }

        private void HandleLoanPayment(PaymentDialog paymentDialog)
        {
            try
            {
                // Get the loan customer details
                var customer = _dbService.GetLoanCustomerById(paymentDialog.LoanCustomerId);
                if (customer == null)
                {
                    MessageBox.Show("Customer not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var oldBalance = customer.CurrentBalance;
                var newBalance = oldBalance + _receiptViewModel.TotalAmount;

                // Update the receipt with loan information
                _receiptViewModel.SetLoanPayment(
                    customer.CustomerId,
                    customer.Name,
                    oldBalance,
                    newBalance
                );

                // Force UI refresh
                _receiptView.DataContext = null;
                _receiptView.DataContext = _receiptViewModel;

                // Add the loan transaction to database
                _dbService.AddLoanTransaction(
                    customer.CustomerId,
                    TransactionType.PURCHASE,
                    _receiptViewModel.TotalAmount,
                    oldBalance,
                    newBalance,
                    _receiptViewModel.BillNumber,
                    "Purchase on loan"
                );
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error processing loan payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
