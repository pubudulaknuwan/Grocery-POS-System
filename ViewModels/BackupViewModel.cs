using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using iTextSharp.text;
using iTextSharp.text.pdf;
using VillageSmartPOS.Helpers;
using VillageSmartPOS.Models;
using VillageSmartPOS.Services;

namespace VillageSmartPOS.ViewModels
{
    public class BackupViewModel : BaseViewModel
    {
        private readonly DatabaseService dbService = new();
        private readonly string reportsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "VillageSmartPOS_Reports");

        private string statusMessage = "Ready to generate reports";
        public string StatusMessage
        {
            get => statusMessage;
            set { statusMessage = value; OnPropertyChanged(); }
        }

        private double progressValue = 0;
        public double ProgressValue
        {
            get => progressValue;
            set { progressValue = value; OnPropertyChanged(); }
        }

        private bool isGenerating = false;
        public bool IsGenerating
        {
            get => isGenerating;
            set { isGenerating = value; OnPropertyChanged(); }
        }

        public ICommand GenerateLoanReportCommand { get; }
        public ICommand GenerateBalanceReportCommand { get; }
        public ICommand GenerateProductReportCommand { get; }
        public ICommand ExportSqlScriptCommand { get; }
        public ICommand OpenReportsFolderCommand { get; }

        public BackupViewModel()
        {
            GenerateLoanReportCommand = new RelayCommand(async () => await GenerateLoanReport());
            GenerateBalanceReportCommand = new RelayCommand(async () => await GenerateBalanceReport());
            GenerateProductReportCommand = new RelayCommand(async () => await GenerateProductReport());
            ExportSqlScriptCommand = new RelayCommand(async () => await ExportSqlScript());
            OpenReportsFolderCommand = new RelayCommand(OpenReportsFolder);

            // Create reports folder if it doesn't exist
            if (!Directory.Exists(reportsFolder))
            {
                Directory.CreateDirectory(reportsFolder);
            }
        }

        private async Task GenerateLoanReport()
        {
            try
            {
                IsGenerating = true;
                StatusMessage = "Generating loan customers report...";
                ProgressValue = 10;

                var customers = dbService.GetAllLoanCustomers();
                ProgressValue = 30;

                var fileName = $"Loan_Customers_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var filePath = Path.Combine(reportsFolder, fileName);

                // Check if file already exists and delete it
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                await Task.Run(() => CreateLoanReport(customers, filePath));
                ProgressValue = 100;

                // Verify file was created successfully
                if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
                {
                    StatusMessage = $"✅ Loan report generated successfully: {fileName}";
                    MessageBox.Show($"Loan customers report generated successfully!\nFile: {fileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    throw new Exception("PDF file was not created or is empty");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error generating loan report: {ex.Message}";
                MessageBox.Show($"Error generating loan report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsGenerating = false;
                ProgressValue = 0;
            }
        }

        private async Task GenerateBalanceReport()
        {
            try
            {
                IsGenerating = true;
                StatusMessage = "Generating temporary balances report...";
                ProgressValue = 10;

                var balances = dbService.GetAllTemporaryBalances();
                ProgressValue = 30;

                var fileName = $"Temporary_Balances_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var filePath = Path.Combine(reportsFolder, fileName);

                // Check if file already exists and delete it
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                await Task.Run(() => CreateBalanceReport(balances, filePath));
                ProgressValue = 100;

                // Verify file was created successfully
                if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
                {
                    StatusMessage = $"✅ Balance report generated successfully: {fileName}";
                    MessageBox.Show($"Temporary balances report generated successfully!\nFile: {fileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    throw new Exception("PDF file was not created or is empty");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error generating balance report: {ex.Message}";
                MessageBox.Show($"Error generating balance report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsGenerating = false;
                ProgressValue = 0;
            }
        }

        private async Task GenerateProductReport()
        {
            try
            {
                IsGenerating = true;
                StatusMessage = "Generating product details report...";
                ProgressValue = 10;

                var products = dbService.GetAllProducts();
                ProgressValue = 30;

                var fileName = $"Product_Details_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var filePath = Path.Combine(reportsFolder, fileName);

                // Check if file already exists and delete it
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                await Task.Run(() => CreateProductReport(products, filePath));
                ProgressValue = 100;

                // Verify file was created successfully
                if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
                {
                    StatusMessage = $"✅ Product report generated successfully: {fileName}";
                    MessageBox.Show($"Product details report generated successfully!\nFile: {fileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    throw new Exception("PDF file was not created or is empty");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error generating product report: {ex.Message}";
                MessageBox.Show($"Error generating product report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsGenerating = false;
                ProgressValue = 0;
            }
        }

        private async Task ExportSqlScript()
        {
            try
            {
                IsGenerating = true;
                StatusMessage = "Exporting database SQL script...";
                ProgressValue = 10;

                var fileName = $"Database_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                var filePath = Path.Combine(reportsFolder, fileName);

                await Task.Run(() => CreateSqlScript(filePath));
                ProgressValue = 100;

                StatusMessage = $"✅ SQL script exported successfully: {fileName}";
                MessageBox.Show($"Database SQL script exported successfully!\nFile: {fileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error exporting SQL script: {ex.Message}";
                MessageBox.Show($"Error exporting SQL script: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsGenerating = false;
                ProgressValue = 0;
            }
        }

        private void OpenReportsFolder()
        {
            try
            {
                if (Directory.Exists(reportsFolder))
                {
                    Process.Start("explorer.exe", reportsFolder);
                }
                else
                {
                    Directory.CreateDirectory(reportsFolder);
                    Process.Start("explorer.exe", reportsFolder);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening reports folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateLoanReport(List<LoanCustomer> customers, string filePath)
        {
            FileStream? fs = null;
            Document? document = null;
            PdfWriter? writer = null;

            try
            {
                fs = new FileStream(filePath, FileMode.Create);
                document = new Document(PageSize.A4, 25, 25, 30, 30);
                writer = PdfWriter.GetInstance(document, fs);

                document.Open();

                // Title
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph title = new Paragraph("Loan Customers Report", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Date
                Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Paragraph date = new Paragraph($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", dateFont);
                date.Alignment = Element.ALIGN_CENTER;
                date.SpacingAfter = 20f;
                document.Add(date);

                // Summary
                var totalCustomers = customers.Count;
                var totalBalance = customers.Sum(c => c.CurrentBalance);
                var activeCustomers = customers.Count(c => c.CurrentBalance > 0);

                Paragraph summary = new Paragraph($"Total Customers: {totalCustomers} | Active Loans: {activeCustomers} | Total Outstanding: Rs. {totalBalance:F2}", dateFont);
                summary.Alignment = Element.ALIGN_CENTER;
                summary.SpacingAfter = 20f;
                document.Add(summary);

                // Table
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;

                // Headers
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                table.AddCell(new PdfPCell(new Phrase("Customer ID", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Name", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Phone", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Address", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Current Balance", headerFont)));

                // Data - Use standard font
                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                foreach (var customer in customers.OrderByDescending(c => c.CurrentBalance))
                {
                    table.AddCell(new PdfPCell(new Phrase(customer.CustomerId, dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(customer.Name ?? "", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(customer.Phone ?? "", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(customer.Address ?? "", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase($"Rs. {customer.CurrentBalance:F2}", dataFont)));
                }

                document.Add(table);
            }
            finally
            {
                // Ensure proper disposal of resources
                if (document != null)
                {
                    document.Close();
                }
                if (writer != null)
                {
                    writer.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        private void CreateBalanceReport(List<TemporaryBalance> balances, string filePath)
        {
            FileStream? fs = null;
            Document? document = null;
            PdfWriter? writer = null;

            try
            {
                fs = new FileStream(filePath, FileMode.Create);
                document = new Document(PageSize.A4, 25, 25, 30, 30);
                writer = PdfWriter.GetInstance(document, fs);

                document.Open();

                // Title
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph title = new Paragraph("Temporary Balances Report", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Date
                Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Paragraph date = new Paragraph($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", dateFont);
                date.Alignment = Element.ALIGN_CENTER;
                date.SpacingAfter = 20f;
                document.Add(date);

                // Summary
                var totalBalances = balances.Count;
                var totalAmount = balances.Sum(b => b.Balance);

                Paragraph summary = new Paragraph($"Total Records: {totalBalances} | Total Amount: Rs. {totalAmount:F2}", dateFont);
                summary.Alignment = Element.ALIGN_CENTER;
                summary.SpacingAfter = 20f;
                document.Add(summary);

                // Table
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;

                // Headers
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                table.AddCell(new PdfPCell(new Phrase("ID", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Customer Name", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Balance", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Created", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Notes", headerFont)));

                // Data - Use standard font
                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                foreach (var balance in balances.OrderByDescending(b => b.Balance))
                {
                    table.AddCell(new PdfPCell(new Phrase(balance.Id.ToString(), dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(balance.CustomerName ?? "", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase($"Rs. {balance.Balance:F2}", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(balance.CreatedAt.ToString("dd/MM/yyyy"), dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(balance.Notes ?? "", dataFont)));
                }

                document.Add(table);
            }
            finally
            {
                // Ensure proper disposal of resources
                if (document != null)
                {
                    document.Close();
                }
                if (writer != null)
                {
                    writer.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        private void CreateProductReport(List<Product> products, string filePath)
        {
            FileStream? fs = null;
            Document? document = null;
            PdfWriter? writer = null;

            try
            {
                fs = new FileStream(filePath, FileMode.Create);
                document = new Document(PageSize.A4, 25, 25, 30, 30);
                writer = PdfWriter.GetInstance(document, fs);

                document.Open();

                // Title
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph title = new Paragraph("Product Details Report", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Date
                Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Paragraph date = new Paragraph($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", dateFont);
                date.Alignment = Element.ALIGN_CENTER;
                date.SpacingAfter = 20f;
                document.Add(date);

                // Summary
                var totalProducts = products.Count;
                var totalValue = products.Sum(p => p.Price * p.Quantity);
                var lowStockProducts = products.Count(p => p.IsLowStock);
                var outOfStockProducts = products.Count(p => p.IsOutOfStock);

                Paragraph summary = new Paragraph($"Total Products: {totalProducts} | Total Value: Rs. {totalValue:F2} | Low Stock: {lowStockProducts} | Out of Stock: {outOfStockProducts}", dateFont);
                summary.Alignment = Element.ALIGN_CENTER;
                summary.SpacingAfter = 20f;
                document.Add(summary);

                // Table
                PdfPTable table = new PdfPTable(8);
                table.WidthPercentage = 100;

                // Headers
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);
                table.AddCell(new PdfPCell(new Phrase("Name", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Barcode", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Price", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Marked Price", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Quantity", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Category", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Supplier", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Status", headerFont)));

                // Data - Use standard font
                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 7);

                foreach (var product in products.OrderBy(p => p.Name))
                {
                    // Handle Sinhala text properly
                    string productName = product.Name ?? "";
                    
                    table.AddCell(new PdfPCell(new Phrase(productName, dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(product.Barcode, dataFont)));
                    table.AddCell(new PdfPCell(new Phrase($"Rs. {product.Price:F2}", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(product.MarkedPrice > 0 ? $"Rs. {product.MarkedPrice:F2}" : "", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase($"{product.Quantity} {product.UnitMeasure}", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(product.Category ?? "", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(product.Supplier ?? "", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(product.StockStatus, dataFont)));
                }

                document.Add(table);
            }
            finally
            {
                // Ensure proper disposal of resources
                if (document != null)
                {
                    document.Close();
                }
                if (writer != null)
                {
                    writer.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        private void CreateSqlScript(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("-- VillageSmartPOS Database Backup");
                writer.WriteLine($"-- Generated on: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                writer.WriteLine();
                writer.WriteLine("-- Create database");
                writer.WriteLine("CREATE DATABASE IF NOT EXISTS posdb;");
                writer.WriteLine("USE posdb;");
                writer.WriteLine();

                // Products table
                writer.WriteLine("-- Create products table");
                writer.WriteLine(@"CREATE TABLE IF NOT EXISTS products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    barcode VARCHAR(50) UNIQUE,
    price DECIMAL(10,2) NOT NULL,
    marked_price DECIMAL(10,2) NOT NULL,
    quantity INT NOT NULL DEFAULT 0,
    unit_type ENUM('mass', 'unit') NOT NULL DEFAULT 'unit',
    unit_measure VARCHAR(20) NOT NULL DEFAULT 'pieces',
    category VARCHAR(100),
    supplier VARCHAR(100),
    reorder_level INT DEFAULT 10,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);");
                writer.WriteLine();

                // Loan customers table
                writer.WriteLine("-- Create loan_customers table");
                writer.WriteLine(@"CREATE TABLE IF NOT EXISTS loan_customers (
    id INT AUTO_INCREMENT PRIMARY KEY,
    customer_id VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    address TEXT,
    current_balance DECIMAL(10,2) DEFAULT 0.00,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);");
                writer.WriteLine();

                // Loan transactions table
                writer.WriteLine("-- Create loan_transactions table");
                writer.WriteLine(@"CREATE TABLE IF NOT EXISTS loan_transactions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    customer_id VARCHAR(50) NOT NULL,
    transaction_type ENUM('payment', 'loan') NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    old_balance DECIMAL(10,2) NOT NULL,
    new_balance DECIMAL(10,2) NOT NULL,
    bill_number VARCHAR(50),
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (customer_id) REFERENCES loan_customers(customer_id) ON DELETE CASCADE
);");
                writer.WriteLine();

                // Temporary balances table
                writer.WriteLine("-- Create temporary_balances table");
                writer.WriteLine(@"CREATE TABLE IF NOT EXISTS temporary_balances (
    id INT AUTO_INCREMENT PRIMARY KEY,
    customer_name VARCHAR(255) NOT NULL,
    balance DECIMAL(10,2) NOT NULL,
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);");
                writer.WriteLine();

                // Bill settings table
                writer.WriteLine("-- Create bill_settings table");
                writer.WriteLine(@"CREATE TABLE IF NOT EXISTS bill_settings (
    id INT AUTO_INCREMENT PRIMARY KEY,
    grocery_name VARCHAR(255) NOT NULL DEFAULT 'රසිංහ වෙළඳසැල',
    store_address TEXT NOT NULL,
    phone_number_1 VARCHAR(20) NOT NULL DEFAULT '0352263213',
    phone_number_2 VARCHAR(20) NOT NULL DEFAULT '0763082845',
    cashier_name VARCHAR(255) NOT NULL DEFAULT 'Avindra Ranasinghe',
    logo_text VARCHAR(50) NOT NULL DEFAULT 'RS',
    receipt_header TEXT NOT NULL,
    receipt_footer TEXT NOT NULL,
    currency_symbol VARCHAR(10) NOT NULL DEFAULT 'Rs.',
    tax_rate DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);");
                writer.WriteLine();

                writer.WriteLine("-- Insert default bill settings");
                writer.WriteLine(@"INSERT INTO bill_settings (grocery_name, store_address, phone_number_1, phone_number_2, cashier_name, logo_text, receipt_header, receipt_footer, currency_symbol, tax_rate)
SELECT * FROM (
    SELECT 'රසිංහ වෙළඳසැල' as grocery_name,
           'රන්දෙණිය, පිරිබැද්දර, කාගල්ල' as store_address,
           '0352263213' as phone_number_1,
           '0763082845' as phone_number_2,
           'Avindra Ranasinghe' as cashier_name,
           'RS' as logo_text,
           'Thank you for shopping with us!' as receipt_header,
           'Please visit again!' as receipt_footer,
           'Rs.' as currency_symbol,
           0.00 as tax_rate
) AS temp
WHERE NOT EXISTS (SELECT 1 FROM bill_settings LIMIT 1);");
                writer.WriteLine();

                writer.WriteLine("-- End of backup script");
            }
        }
    }
} 