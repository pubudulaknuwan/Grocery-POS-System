using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VillageSmartPOS.Helpers;
using VillageSmartPOS.Services;
using System.Collections.Generic; // Added for Dictionary
using System.Printing; // Added for PrintServer

namespace VillageSmartPOS.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        
        // Static event to notify when settings are saved
        public static event EventHandler? SettingsSaved;

        // Theme and Language Settings
        private string _selectedTheme = "Light";
        private string _selectedLanguage = "English";

        // Bill Customization Settings
        private string _groceryName = "රසිංහ වෙළඳසැල";
        private string _storeAddress = "රන්දෙණිය, පිරිබැද්දර, කාගල්ල";
        private string _phoneNumber1 = "0352263213";
        private string _phoneNumber2 = "0763082845";
        private string _cashierName = "Avindra Ranasinghe";
        private string _logoText = "RS";
        private string _receiptHeader = "Thank you for shopping with us!";
        private string _receiptFooter = "Please visit again!";
        private string _currencySymbol = "Rs.";
        private string _taxRate = "0";

        // General Settings
        private bool _autoPrintReceipts = false;
        private bool _showStockWarnings = true;
        private bool _enableSoundNotifications = true;
        private bool _autoSaveBills = true;
        private bool _showProductImages = false;

        // Printer Settings
        private string _defaultPrinter = "POS-80";
        private List<string> _availablePrinters = new List<string>();

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                OnPropertyChanged();
            }
        }

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged();
            }
        }

        // Bill Customization Properties
        public string GroceryName
        {
            get => _groceryName;
            set
            {
                System.Diagnostics.Debug.WriteLine($"GroceryName property set to: '{value}'");
                _groceryName = value;
                OnPropertyChanged();
            }
        }

        public string StoreAddress
        {
            get => _storeAddress;
            set
            {
                _storeAddress = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber1
        {
            get => _phoneNumber1;
            set
            {
                _phoneNumber1 = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber2
        {
            get => _phoneNumber2;
            set
            {
                _phoneNumber2 = value;
                OnPropertyChanged();
            }
        }

        public string CashierName
        {
            get => _cashierName;
            set
            {
                _cashierName = value;
                OnPropertyChanged();
            }
        }

        public string LogoText
        {
            get => _logoText;
            set
            {
                _logoText = value;
                OnPropertyChanged();
            }
        }

        public string ReceiptHeader
        {
            get => _receiptHeader;
            set
            {
                _receiptHeader = value;
                OnPropertyChanged();
            }
        }

        public string ReceiptFooter
        {
            get => _receiptFooter;
            set
            {
                _receiptFooter = value;
                OnPropertyChanged();
            }
        }

        public string CurrencySymbol
        {
            get => _currencySymbol;
            set
            {
                _currencySymbol = value;
                OnPropertyChanged();
            }
        }

        public string TaxRate
        {
            get => _taxRate;
            set
            {
                _taxRate = value;
                OnPropertyChanged();
            }
        }

        // General Settings Properties
        public bool AutoPrintReceipts
        {
            get => _autoPrintReceipts;
            set
            {
                _autoPrintReceipts = value;
                OnPropertyChanged();
            }
        }

        public bool ShowStockWarnings
        {
            get => _showStockWarnings;
            set
            {
                _showStockWarnings = value;
                OnPropertyChanged();
            }
        }

        public bool EnableSoundNotifications
        {
            get => _enableSoundNotifications;
            set
            {
                _enableSoundNotifications = value;
                OnPropertyChanged();
            }
        }

        public bool AutoSaveBills
        {
            get => _autoSaveBills;
            set
            {
                _autoSaveBills = value;
                OnPropertyChanged();
            }
        }

        public bool ShowProductImages
        {
            get => _showProductImages;
            set
            {
                _showProductImages = value;
                OnPropertyChanged();
            }
        }

        // Printer Settings Properties
        public string DefaultPrinter
        {
            get => _defaultPrinter;
            set
            {
                _defaultPrinter = value;
                OnPropertyChanged();
            }
        }

        public List<string> AvailablePrinters
        {
            get => _availablePrinters;
            set
            {
                _availablePrinters = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveSettingsCommand { get; }
        public ICommand ResetSettingsCommand { get; }

        public SettingsViewModel()
        {
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            ResetSettingsCommand = new RelayCommand(ResetSettings);
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                // Initialize default bill settings in database if needed
                _databaseService.InitializeBillSettings();

                // Load bill settings from database
                var settings = _databaseService.GetAllBillSettings();

                // Load bill customization settings
                System.Diagnostics.Debug.WriteLine("Loading bill settings from database:");
                System.Diagnostics.Debug.WriteLine($"  Database grocery_name: '{settings.GetValueOrDefault("grocery_name", "රසිංහ වෙළඳසැල")}'");
                System.Diagnostics.Debug.WriteLine($"  Database store_address: '{settings.GetValueOrDefault("store_address", "රන්දෙණිය, පිරිබැද්දර, කාගල්ල")}'");
                
                GroceryName = settings.GetValueOrDefault("grocery_name", "රසිංහ වෙළඳසැල");
                StoreAddress = settings.GetValueOrDefault("store_address", "රන්දෙණිය, පිරිබැද්දර, කාගල්ල");
                PhoneNumber1 = settings.GetValueOrDefault("phone_number_1", "0352263213");
                PhoneNumber2 = settings.GetValueOrDefault("phone_number_2", "0763082845");
                CashierName = settings.GetValueOrDefault("cashier_name", "Avindra Ranasinghe");
                LogoText = settings.GetValueOrDefault("logo_text", "RS");
                ReceiptHeader = settings.GetValueOrDefault("receipt_header", "Thank you for shopping with us!");
                ReceiptFooter = settings.GetValueOrDefault("receipt_footer", "Please visit again!");
                CurrencySymbol = settings.GetValueOrDefault("currency_symbol", "Rs.");
                TaxRate = settings.GetValueOrDefault("tax_rate", "0");

                // Load general settings (these will be handled separately for now)
                AutoPrintReceipts = false;
                ShowStockWarnings = true;
                EnableSoundNotifications = true;
                AutoSaveBills = true;
                ShowProductImages = false;

                // Load printer settings
                LoadAvailablePrinters();
                DefaultPrinter = "POS-80"; // Set default value, don't load from database

                // Load theme and language settings (these will be handled separately for now)
                SelectedTheme = "Light";
                SelectedLanguage = "English";

                System.Diagnostics.Debug.WriteLine("Bill settings loaded successfully from database");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading bill settings: {ex.Message}");
            }
        }

        public void SaveSettings()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== SaveSettings method called ===");
                System.Diagnostics.Debug.WriteLine($"Current GroceryName: '{GroceryName}'");
                System.Diagnostics.Debug.WriteLine($"Current StoreAddress: '{StoreAddress}'");
                System.Diagnostics.Debug.WriteLine($"Current PhoneNumber1: '{PhoneNumber1}'");
                System.Diagnostics.Debug.WriteLine($"Current PhoneNumber2: '{PhoneNumber2}'");
                System.Diagnostics.Debug.WriteLine($"Current CashierName: '{CashierName}'");
                System.Diagnostics.Debug.WriteLine($"Current LogoText: '{LogoText}'");
                System.Diagnostics.Debug.WriteLine($"Current ReceiptHeader: '{ReceiptHeader}'");
                System.Diagnostics.Debug.WriteLine($"Current ReceiptFooter: '{ReceiptFooter}'");
                System.Diagnostics.Debug.WriteLine($"Current CurrencySymbol: '{CurrencySymbol}'");
                System.Diagnostics.Debug.WriteLine($"Current TaxRate: '{TaxRate}'");
                
                var billSettings = new Dictionary<string, string>
                {
                    // Bill customization settings only
                    { "grocery_name", GroceryName },
                    { "store_address", StoreAddress },
                    { "phone_number_1", PhoneNumber1 },
                    { "phone_number_2", PhoneNumber2 },
                    { "cashier_name", CashierName },
                    { "logo_text", LogoText },
                    { "receipt_header", ReceiptHeader },
                    { "receipt_footer", ReceiptFooter },
                    { "currency_symbol", CurrencySymbol },
                    { "tax_rate", TaxRate }
                    // Removed default_printer - only save bill customization settings
                };

                System.Diagnostics.Debug.WriteLine($"About to save {billSettings.Count} bill settings to database");
                foreach (var setting in billSettings)
                {
                    System.Diagnostics.Debug.WriteLine($"Setting: {setting.Key} = '{setting.Value}'");
                }

                // Save bill settings to database
                System.Diagnostics.Debug.WriteLine("Calling _databaseService.SaveBillSettings...");
                _databaseService.SaveBillSettings(billSettings);
                System.Diagnostics.Debug.WriteLine("_databaseService.SaveBillSettings completed successfully");

                // Force reload settings from database
                System.Diagnostics.Debug.WriteLine("Reloading settings from database...");
                LoadSettings();
                System.Diagnostics.Debug.WriteLine("Settings reloaded from database");

                // Refresh the BillSettingsService to ensure new values are loaded
                System.Diagnostics.Debug.WriteLine("Refreshing BillSettingsService...");
                BillSettingsService.Instance.RefreshSettings();
                System.Diagnostics.Debug.WriteLine("BillSettingsService refreshed");

                System.Diagnostics.Debug.WriteLine("=== Bill settings saved successfully to database ===");
                System.Diagnostics.Debug.WriteLine($"Grocery Name: {GroceryName}");
                System.Diagnostics.Debug.WriteLine($"Store Address: {StoreAddress}");
                System.Diagnostics.Debug.WriteLine($"Phone Numbers: {PhoneNumber1}, {PhoneNumber2}");
                System.Diagnostics.Debug.WriteLine($"Cashier Name: {CashierName}");
                System.Diagnostics.Debug.WriteLine($"Logo Text: {LogoText}");
                
                // Notify that settings have been saved
                SettingsSaved?.Invoke(this, EventArgs.Empty);
                System.Diagnostics.Debug.WriteLine("SettingsSaved event invoked");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR saving bill settings: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Show error message to user
                System.Windows.MessageBox.Show($"Error saving settings: {ex.Message}", "Save Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void LoadAvailablePrinters()
        {
            try
            {
                var printers = new List<string>();
                var printServer = new PrintServer();
                
                foreach (var printer in printServer.GetPrintQueues())
                {
                    printers.Add(printer.Name);
                }
                
                AvailablePrinters = printers;
                System.Diagnostics.Debug.WriteLine($"Loaded {printers.Count} available printers");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading printers: {ex.Message}");
                AvailablePrinters = new List<string> { "POS-80", "Default Printer" };
            }
        }

        private void ResetSettings()
        {
            // Reset to default values
            SelectedTheme = "Light";
            SelectedLanguage = "English";
            
            // Reset bill customization settings
            GroceryName = "රසිංහ වෙළඳසැල";
            StoreAddress = "රන්දෙණිය, පිරිබැද්දර, කාගල්ල";
            PhoneNumber1 = "0352263213";
            PhoneNumber2 = "0763082845";
            CashierName = "Avindra Ranasinghe";
            LogoText = "RS";
            ReceiptHeader = "Thank you for shopping with us!";
            ReceiptFooter = "Please visit again!";
            CurrencySymbol = "Rs.";
            TaxRate = "0";

            // Reset general settings
            AutoPrintReceipts = false;
            ShowStockWarnings = true;
            EnableSoundNotifications = true;
            AutoSaveBills = true;
            ShowProductImages = false;

            // Reset printer settings
            DefaultPrinter = "POS-80";
            LoadAvailablePrinters();

            // Save the reset settings to database
            SaveSettings();

            System.Diagnostics.Debug.WriteLine("Settings reset to default values");
        }
    }
} 