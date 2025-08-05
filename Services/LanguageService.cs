using System;
using System.Collections.Generic;
using System.Windows;

namespace VillageSmartPOS.Services
{
    public class LanguageService
    {
        public static LanguageService Instance { get; } = new LanguageService();
        
        public event EventHandler<LanguageChangedEventArgs>? LanguageChanged;
        
        private LanguageService() { }

        public void ApplyLanguage(string languageCode)
        {
            try
            {
                var application = Application.Current;
                if (application == null) return;

                System.Diagnostics.Debug.WriteLine($"Starting to apply language: {languageCode}");

                // Apply language-specific resources
                if (languageCode.Equals("Sinhala", StringComparison.OrdinalIgnoreCase))
                {
                    ApplySinhalaLanguage(application);
                }
                else
                {
                    ApplyEnglishLanguage(application);
                }

                // Notify language change
                LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(languageCode));
                
                System.Diagnostics.Debug.WriteLine($"Language applied: {languageCode}");
                
                // Debug: Check if resources are actually applied
                System.Diagnostics.Debug.WriteLine($"SalesBilling resource: {application.Resources["SalesBilling"]}");
                System.Diagnostics.Debug.WriteLine($"BillItems resource: {application.Resources["BillItems"]}");
                System.Diagnostics.Debug.WriteLine($"Total resource: {application.Resources["Total"]}");
                
                // Force immediate UI refresh
                System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    if (application.MainWindow != null)
                    {
                        application.MainWindow.InvalidateVisual();
                        application.MainWindow.UpdateLayout();
                        
                        // Force refresh of all child elements recursively
                        RefreshAllElements(application.MainWindow);
                        
                        // Force a second refresh after a short delay
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                        {
                            application.MainWindow.InvalidateVisual();
                            application.MainWindow.UpdateLayout();
                            RefreshAllElements(application.MainWindow);
                            
                            // Force a third refresh after another delay
                            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                            {
                                application.MainWindow.InvalidateVisual();
                                application.MainWindow.UpdateLayout();
                                RefreshAllElements(application.MainWindow);
                                
                                // Try to find and refresh the ContentControl specifically
                                var contentControl = application.MainWindow.FindName("MainContentControl") as System.Windows.Controls.ContentControl;
                                if (contentControl != null)
                                {
                                    contentControl.InvalidateVisual();
                                    contentControl.UpdateLayout();
                                    RefreshAllElements(contentControl);
                                }
                            }), System.Windows.Threading.DispatcherPriority.Loaded);
                        }), System.Windows.Threading.DispatcherPriority.Loaded);
                    }
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying language: {ex.Message}");
            }
        }

        private void ApplyEnglishLanguage(Application application)
        {
            var resources = new Dictionary<string, object>();
            
            // Navigation
            resources["NavSalesBilling"] = "Sales Billing";
            resources["NavAddProduct"] = "Add Product";
            resources["NavInventory"] = "Inventory";
            resources["NavLoanCustomers"] = "Loan Customers";
            resources["NavReports"] = "Reports";
            resources["NavSettings"] = "Settings";
            resources["NavAnalytics"] = "Analytics";
            resources["NavBackup"] = "Backup";
            resources["NavUsers"] = "Users";
            resources["NavHelp"] = "Help";
            resources["NavLogs"] = "Logs";
            resources["NavAbout"] = "About";
            
            // Settings
            resources["SettingsTitle"] = "Settings";
            resources["ThemeSettings"] = "Theme Settings";
            resources["ThemeLabel"] = "Theme:";
            resources["LanguageLabel"] = "Language:";
            resources["LightTheme"] = "Light Theme";
            resources["DarkTheme"] = "Dark Theme";
            resources["EnglishLanguage"] = "English";
            resources["SinhalaLanguage"] = "සිංහල";
            
            // POS System Settings
            resources["POSSystemSettings"] = "POS System Settings";
            resources["CompanyName"] = "Company Name:";
            resources["ReceiptHeader"] = "Receipt Header:";
            resources["CurrencySymbol"] = "Currency Symbol:";
            resources["TaxRate"] = "Tax Rate (%):";
            resources["ReceiptFooter"] = "Receipt Footer:";
            
            // General Settings
            resources["GeneralSettings"] = "General Settings";
            resources["AutoPrintReceipts"] = "Auto-print receipts";
            resources["ShowStockWarnings"] = "Show stock warnings";
            resources["EnableSoundNotifications"] = "Enable sound notifications";
            resources["AutoSaveBills"] = "Auto-save bills";
            resources["ShowProductImages"] = "Show product images";
            
            // Action Buttons
            resources["SaveSettings"] = "Save Settings";
            resources["ResetToDefault"] = "Reset to Default";
            
            // Inventory Management
            resources["InventoryManagement"] = "Inventory Management";
            resources["Search"] = "Search";
            resources["LowStock"] = "Low Stock";
            resources["Products"] = "Products";
            resources["ProductDetails"] = "Product Details";
            resources["Name"] = "Name:";
            resources["Barcode"] = "Barcode:";
            resources["MarkedPrice"] = "Marked Price:";
            resources["OurPrice"] = "Our Price:";
            resources["Quantity"] = "Quantity:";
            resources["Category"] = "Category:";
            resources["Supplier"] = "Supplier:";
            resources["ReorderLevel"] = "Reorder Level:";
            resources["Description"] = "Description:";
            resources["LoadDetails"] = "Load Details";
            resources["Update"] = "Update";
            resources["Delete"] = "Delete";
            resources["Clear"] = "Clear";
            resources["LowStockAlert"] = "Low Stock Alert";
            resources["LowStockMessage"] = "Some products are running low on stock";
            
            // Sales Billing
            resources["SalesBilling"] = "Sales Billing";
            resources["SearchProducts"] = "Search Products";
            resources["QuantityWeight"] = "Quantity/Weight:";
            resources["Price"] = "Price:";
            resources["AddToBill"] = "Add to Bill";
            resources["BillItems"] = "Bill Items";
            resources["RemoveItem"] = "Remove Item";
            resources["BillSummary"] = "Bill Summary";
            resources["Items"] = "Items:";
            resources["Total"] = "Total:";
            resources["ClearBill"] = "Clear Bill";
            resources["PrintReceipt"] = "Print Receipt";
            
            // Loan Customer Management
            resources["LoanCustomerManagement"] = "Loan Customer Management";
            resources["ManageLoanCustomers"] = "Manage loan customers, track balances, and record repayments";
            resources["SearchCustomers"] = "Search Customers";
            resources["Refresh"] = "Refresh";
            resources["CustomerList"] = "Customer List";
            resources["TransactionHistory"] = "Transaction History";
            resources["CustomerDetails"] = "Customer Details";
            resources["CustomerID"] = "Customer ID *";
            resources["CustomerName"] = "Name *";
            resources["Phone"] = "Phone";
            resources["Address"] = "Address";
            resources["AddCustomer"] = "Add Customer";
            resources["UpdateCustomer"] = "Update Customer";
            resources["DeleteCustomer"] = "Delete Customer";
            resources["ClearForm"] = "Clear Form";
            resources["AddRepayment"] = "Add Repayment";
            resources["RepaymentAmount"] = "Repayment Amount";
            resources["AddRepaymentButton"] = "Add Repayment";
            resources["Information"] = "Information";
            resources["InfoSelectCustomer"] = "• Select a customer to view details and transactions";
            resources["InfoSearchBox"] = "• Use the search box to find customers quickly";
            resources["InfoAddRepayments"] = "• Add repayments to reduce customer balances";
            resources["InfoAutoLog"] = "• All transactions are automatically logged";
            resources["TipLoanCustomers"] = "Tip: Loan customers can be used in the payment dialog when processing bills";
            
            // Add Product
            resources["AddNewProduct"] = "Add New Product";
            resources["ProductName"] = "Product Name:";
            resources["OurPriceRs"] = "Our Price (Rs.):";
            resources["MarkedPriceRs"] = "Marked Price (Rs.):";
            resources["AddProduct"] = "Add Product";
            
                            // Apply resources to Application
                foreach (var kvp in resources)
                {
                    application.Resources[kvp.Key] = kvp.Value;
                }
                
                // Also apply resources to all loaded windows and user controls
                foreach (var window in application.Windows)
                {
                    if (window is System.Windows.Window win)
                    {
                        foreach (var kvp in resources)
                        {
                            win.Resources[kvp.Key] = kvp.Value;
                        }
                        win.InvalidateVisual();
                        
                        // Also apply resources to all child UserControls
                        ApplyResourcesToUserControls(win, resources);
                        
                        // Also apply resources to ContentControl specifically
                        var contentControl = win.FindName("MainContentControl") as System.Windows.Controls.ContentControl;
                        if (contentControl != null)
                        {
                            foreach (var kvp in resources)
                            {
                                contentControl.Resources[kvp.Key] = kvp.Value;
                            }
                            contentControl.InvalidateVisual();
                            contentControl.UpdateLayout();
                        }
                    }
                }
                
                // Also apply resources to MainWindow if it exists
                if (application.MainWindow != null)
                {
                    foreach (var kvp in resources)
                    {
                        application.MainWindow.Resources[kvp.Key] = kvp.Value;
                    }
                    application.MainWindow.InvalidateVisual();
                    
                    // Force a complete UI refresh by triggering property change
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        // Force refresh of all UI elements
                        application.MainWindow.InvalidateVisual();
                        application.MainWindow.UpdateLayout();
                        
                        // Force refresh of all child elements recursively
                        RefreshAllElements(application.MainWindow);
                        
                        // Force a second refresh after a short delay
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                        {
                            application.MainWindow.InvalidateVisual();
                            application.MainWindow.UpdateLayout();
                            RefreshAllElements(application.MainWindow);
                        }), System.Windows.Threading.DispatcherPriority.Loaded);
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                }
        }

        private void ApplySinhalaLanguage(Application application)
        {
            var resources = new Dictionary<string, object>();
            
            // Navigation
            resources["NavSalesBilling"] = "විකුණුම් බිල්පත්";
            resources["NavAddProduct"] = "නිෂ්පාදන එකතු කරන්න";
            resources["NavInventory"] = "භාණ්ඩ ගණන";
            resources["NavLoanCustomers"] = "ණය ගනුදෙනු";
            resources["NavReports"] = "වාර්තා";
            resources["NavSettings"] = "සැකසුම්";
            resources["NavAnalytics"] = "විශ්ලේෂණ";
            resources["NavBackup"] = "උපස්ථාන";
            resources["NavUsers"] = "පරිශීලකයින්";
            resources["NavHelp"] = "උදව්";
            resources["NavLogs"] = "ලඝු";
            resources["NavAbout"] = "ගැන";
            
            // Settings
            resources["SettingsTitle"] = "සැකසුම්";
            resources["ThemeSettings"] = "තේමා සැකසුම්";
            resources["ThemeLabel"] = "තේමාව:";
            resources["LanguageLabel"] = "භාෂාව:";
            resources["LightTheme"] = "ආලෝක තේමාව";
            resources["DarkTheme"] = "අඳුරු තේමාව";
            resources["EnglishLanguage"] = "English";
            resources["SinhalaLanguage"] = "සිංහල";
            
            // POS System Settings
            resources["POSSystemSettings"] = "POS පද්ධති සැකසුම්";
            resources["CompanyName"] = "සමාගමේ නම:";
            resources["ReceiptHeader"] = "පාරිතෝෂික ශීර්ෂය:";
            resources["CurrencySymbol"] = "මුදල් සංකේතය:";
            resources["TaxRate"] = "බදු අනුපාතය (%):";
            resources["ReceiptFooter"] = "පාරිතෝෂික පාදය:";
            
            // General Settings
            resources["GeneralSettings"] = "සාමාන්‍ය සැකසුම්";
            resources["AutoPrintReceipts"] = "ස්වයංක්‍රීය පාරිතෝෂික මුද්‍රණය";
            resources["ShowStockWarnings"] = "භාණ්ඩ අනතුරු ඇඟවීම් පෙන්වන්න";
            resources["EnableSoundNotifications"] = "ශබ්ද දැනුම්දීම් සබල කරන්න";
            resources["AutoSaveBills"] = "ස්වයංක්‍රීය බිල්පත් සුරැකීම";
            resources["ShowProductImages"] = "නිෂ්පාදන රූප පෙන්වන්න";
            
            // Action Buttons
            resources["SaveSettings"] = "සැකසුම් සුරැකීම";
            resources["ResetToDefault"] = "පෙරනිමියට යළි සකස් කරන්න";
            
            // Inventory Management
            resources["InventoryManagement"] = "භාණ්ඩ ගණන කළමනාකරණය";
            resources["Search"] = "සොයන්න";
            resources["LowStock"] = "අඩු භාණ්ඩ";
            resources["Products"] = "නිෂ්පාදන";
            resources["ProductDetails"] = "නිෂ්පාදන විස්තර";
            resources["Name"] = "නම:";
            resources["Barcode"] = "බාර්කෝඩ්:";
            resources["MarkedPrice"] = "සලකුණු මිල:";
            resources["OurPrice"] = "අපගේ මිල:";
            resources["Quantity"] = "ප්‍රමාණය:";
            resources["Category"] = "වර්ගය:";
            resources["Supplier"] = "සැපයුම්කරු:";
            resources["ReorderLevel"] = "නැවත ඇණවුම් මට්ටම:";
            resources["Description"] = "විස්තරය:";
            resources["LoadDetails"] = "විස්තර පූරණය";
            resources["Update"] = "යාවත්කාලීන කරන්න";
            resources["Delete"] = "මකන්න";
            resources["Clear"] = "මකන්න";
            resources["LowStockAlert"] = "අඩු භාණ්ඩ අනතුරු ඇඟවීම";
            resources["LowStockMessage"] = "සමහර නිෂ්පාදන අඩු භාණ්ඩ තිබේ";
            
            // Sales Billing
            resources["SalesBilling"] = "විකුණුම් බිල්පත්";
            resources["SearchProducts"] = "නිෂ්පාදන සොයන්න";
            resources["QuantityWeight"] = "ප්‍රමාණය/බර:";
            resources["Price"] = "මිල:";
            resources["AddToBill"] = "බිල්පතට එකතු කරන්න";
            resources["BillItems"] = "බිල්පත් භාණ්ඩ";
            resources["RemoveItem"] = "භාණ්ඩ ඉවත් කරන්න";
            resources["BillSummary"] = "බිල්පත් සාරාංශය";
            resources["Items"] = "භාණ්ඩ:";
            resources["Total"] = "මුළු:";
            resources["ClearBill"] = "බිල්පත මකන්න";
            resources["PrintReceipt"] = "පාරිතෝෂික මුද්‍රණය";
            
            // Loan Customer Management
            resources["LoanCustomerManagement"] = "ණය ගනුදෙනු කළමනාකරණය";
            resources["ManageLoanCustomers"] = "ණය ගනුදෙනු කළමනාකරන්න, ශේෂ පසුපස බලන්න, ගෙවීම් පටිගත කරන්න";
            resources["SearchCustomers"] = "ගනුදෙනු සොයන්න";
            resources["Refresh"] = "යාවත්කාලීන කරන්න";
            resources["CustomerList"] = "ගනුදෙනු ලැයිස්තුව";
            resources["TransactionHistory"] = "භාර්ගික ඉතිහාසය";
            resources["CustomerDetails"] = "ගනුදෙනු විස්තර";
            resources["CustomerID"] = "ගනුදෙනු හැඳුනුම *";
            resources["CustomerName"] = "නම *";
            resources["Phone"] = "දුරකථන";
            resources["Address"] = "ලිපිනය";
            resources["AddCustomer"] = "ගනුදෙනු එකතු කරන්න";
            resources["UpdateCustomer"] = "ගනුදෙනු යාවත්කාලීන කරන්න";
            resources["DeleteCustomer"] = "ගනුදෙනු මකන්න";
            resources["ClearForm"] = "පෝරමය මකන්න";
            resources["AddRepayment"] = "ගෙවීම් එකතු කරන්න";
            resources["RepaymentAmount"] = "ගෙවීම් මුදල";
            resources["AddRepaymentButton"] = "ගෙවීම් එකතු කරන්න";
            resources["Information"] = "තොරතුරු";
            resources["InfoSelectCustomer"] = "• විස්තර සහ භාර්ගික බැලීමට ගනුදෙනු තෝරන්න";
            resources["InfoSearchBox"] = "• ගනුදෙනු ඉක්මනින් සොයා ගැනීමට සොයුරු කොටුව භාවිතා කරන්න";
            resources["InfoAddRepayments"] = "• ගනුදෙනු ශේෂ අඩු කිරීමට ගෙවීම් එකතු කරන්න";
            resources["InfoAutoLog"] = "• සියලුම භාර්ගික ස්වයංක්‍රීයව පටිගත වේ";
            resources["TipLoanCustomers"] = "ඉඟිය: බිල්පත් සැකසීමේදී ගෙවීම් සංවාදයේ ණය ගනුදෙනු භාවිතා කළ හැකිය";
            
            // Add Product
            resources["AddNewProduct"] = "නව නිෂ්පාදන එකතු කරන්න";
            resources["ProductName"] = "නිෂ්පාදන නම:";
            resources["OurPriceRs"] = "අපගේ මිල (රු.):";
            resources["MarkedPriceRs"] = "සලකුණු මිල (රු.):";
            resources["AddProduct"] = "නිෂ්පාදන එකතු කරන්න";
            
            // Apply resources to Application
            foreach (var kvp in resources)
            {
                application.Resources[kvp.Key] = kvp.Value;
            }
            
            // Also apply resources to all loaded windows and user controls
            foreach (var window in application.Windows)
            {
                if (window is System.Windows.Window win)
                {
                    foreach (var kvp in resources)
                    {
                        win.Resources[kvp.Key] = kvp.Value;
                    }
                    win.InvalidateVisual();
                    
                    // Also apply resources to all child UserControls
                    ApplyResourcesToUserControls(win, resources);
                }
            }
            
                            // Also apply resources to MainWindow if it exists
                if (application.MainWindow != null)
                {
                    foreach (var kvp in resources)
                    {
                        application.MainWindow.Resources[kvp.Key] = kvp.Value;
                    }
                    application.MainWindow.InvalidateVisual();
                    
                    // Force a complete UI refresh by triggering property change
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        // Force refresh of all UI elements
                        application.MainWindow.InvalidateVisual();
                        application.MainWindow.UpdateLayout();
                        
                        // Force refresh of all child elements recursively
                        RefreshAllElements(application.MainWindow);
                        
                        // Force a second refresh after a short delay
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                        {
                            application.MainWindow.InvalidateVisual();
                            application.MainWindow.UpdateLayout();
                            RefreshAllElements(application.MainWindow);
                        }), System.Windows.Threading.DispatcherPriority.Loaded);
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                }
        }

        public string GetCurrentLanguage()
        {
            // Check if Sinhala language is applied by looking for Sinhala text
            if (Application.Current?.Resources.Contains("NavSalesBilling") == true)
            {
                var navText = Application.Current.Resources["NavSalesBilling"] as string;
                if (navText == "විකුණුම් බිල්පත්")
                {
                    return "Sinhala";
                }
            }
            return "English";
        }
        
        private void RefreshAllElements(System.Windows.FrameworkElement element)
        {
            if (element == null) return;
            
            // Force refresh of this element
            element.InvalidateVisual();
            element.UpdateLayout();
            
            // Recursively refresh all child elements
            var count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(element, i) as System.Windows.FrameworkElement;
                if (child != null)
                {
                    RefreshAllElements(child);
                }
            }
        }
        
        private void ApplyResourcesToUserControls(System.Windows.FrameworkElement element, Dictionary<string, object> resources)
        {
            if (element == null) return;
            
            // Apply resources to UserControls
            if (element is System.Windows.Controls.UserControl userControl)
            {
                foreach (var kvp in resources)
                {
                    userControl.Resources[kvp.Key] = kvp.Value;
                }
                userControl.InvalidateVisual();
                userControl.UpdateLayout();
                
                // Special handling for SalesBillingPage
                if (userControl.GetType().Name == "SalesBillingPage")
                {
                    System.Diagnostics.Debug.WriteLine("Found SalesBillingPage, applying resources directly");
                    foreach (var kvp in resources)
                    {
                        userControl.Resources[kvp.Key] = kvp.Value;
                    }
                    userControl.InvalidateVisual();
                    userControl.UpdateLayout();
                    
                    // Also apply resources to the parent of the UserControl
                    var parent = System.Windows.Media.VisualTreeHelper.GetParent(userControl) as System.Windows.FrameworkElement;
                    if (parent != null)
                    {
                        foreach (var kvp in resources)
                        {
                            parent.Resources[kvp.Key] = kvp.Value;
                        }
                        parent.InvalidateVisual();
                        parent.UpdateLayout();
                    }
                    
                    // Force refresh of specific elements that might be problematic
                    ForceRefreshSpecificElements(userControl);
                    
                    // Force a complete refresh of the UserControl with multiple cycles
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        userControl.InvalidateVisual();
                        userControl.UpdateLayout();
                        ForceRefreshSpecificElements(userControl);
                        
                        // Second refresh cycle
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                        {
                            userControl.InvalidateVisual();
                            userControl.UpdateLayout();
                            ForceRefreshSpecificElements(userControl);
                            
                            // Third refresh cycle
                            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                            {
                                userControl.InvalidateVisual();
                                userControl.UpdateLayout();
                                ForceRefreshSpecificElements(userControl);
                            }), System.Windows.Threading.DispatcherPriority.Loaded);
                        }), System.Windows.Threading.DispatcherPriority.Loaded);
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                }
            }
            
            // Also apply to all FrameworkElements
            if (element is System.Windows.FrameworkElement frameworkElement)
            {
                foreach (var kvp in resources)
                {
                    frameworkElement.Resources[kvp.Key] = kvp.Value;
                }
                frameworkElement.InvalidateVisual();
                frameworkElement.UpdateLayout();
            }
            
            // Recursively apply to all child elements
            var count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(element, i) as System.Windows.FrameworkElement;
                if (child != null)
                {
                    ApplyResourcesToUserControls(child, resources);
                }
            }
        }
        
        private void ForceRefreshSpecificElements(System.Windows.Controls.UserControl userControl)
        {
            // Force refresh of TextBlocks that might be problematic
            var textBlocks = FindVisualChildren<System.Windows.Controls.TextBlock>(userControl);
            foreach (var textBlock in textBlocks)
            {
                if (textBlock.Text != null && textBlock.Text.Contains("{DynamicResource"))
                {
                    System.Diagnostics.Debug.WriteLine($"Found problematic TextBlock: {textBlock.Text}");
                    // Force the binding to refresh
                    var binding = textBlock.GetBindingExpression(System.Windows.Controls.TextBlock.TextProperty);
                    if (binding != null)
                    {
                        binding.UpdateTarget();
                    }
                    textBlock.InvalidateVisual();
                    textBlock.UpdateLayout();
                    
                    // Try to directly set the text based on the resource key
                    var resourceKey = ExtractResourceKey(textBlock.Text);
                    if (!string.IsNullOrEmpty(resourceKey) && Application.Current.Resources.Contains(resourceKey))
                    {
                        textBlock.Text = Application.Current.Resources[resourceKey] as string ?? textBlock.Text;
                    }
                }
            }
            
            // Force refresh of Buttons that might be problematic
            var buttons = FindVisualChildren<System.Windows.Controls.Button>(userControl);
            foreach (var button in buttons)
            {
                if (button.Content != null && button.Content.ToString().Contains("{DynamicResource"))
                {
                    System.Diagnostics.Debug.WriteLine($"Found problematic Button: {button.Content}");
                    // Force the binding to refresh
                    var binding = button.GetBindingExpression(System.Windows.Controls.Button.ContentProperty);
                    if (binding != null)
                    {
                        binding.UpdateTarget();
                    }
                    button.InvalidateVisual();
                    button.UpdateLayout();
                    
                    // Try to directly set the content based on the resource key
                    var resourceKey = ExtractResourceKey(button.Content.ToString());
                    if (!string.IsNullOrEmpty(resourceKey) && Application.Current.Resources.Contains(resourceKey))
                    {
                        button.Content = Application.Current.Resources[resourceKey] as string ?? button.Content;
                    }
                }
            }
            
            // Force refresh of DataGridTextColumn headers
            var dataGrids = FindVisualChildren<System.Windows.Controls.DataGrid>(userControl);
            foreach (var dataGrid in dataGrids)
            {
                dataGrid.InvalidateVisual();
                dataGrid.UpdateLayout();
            }
        }
        
        private string ExtractResourceKey(string dynamicResourceText)
        {
            // Extract the resource key from {DynamicResource KeyName}
            if (dynamicResourceText.StartsWith("{DynamicResource ") && dynamicResourceText.EndsWith("}"))
            {
                return dynamicResourceText.Substring(16, dynamicResourceText.Length - 17);
            }
            return string.Empty;
        }
        
        private IEnumerable<T> FindVisualChildren<T>(System.Windows.DependencyObject depObj) where T : System.Windows.DependencyObject
        {
            if (depObj == null) yield break;
            
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                if (child is T t)
                    yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }
    }

    public class LanguageChangedEventArgs : EventArgs
    {
        public string LanguageCode { get; }
        
        public LanguageChangedEventArgs(string languageCode)
        {
            LanguageCode = languageCode;
        }
    }
} 