using System;
using System.Collections.Generic;

namespace VillageSmartPOS.Services
{
    public class BillSettingsService
    {
        private static BillSettingsService? _instance;
        private readonly DatabaseService _databaseService;

        public static BillSettingsService Instance
        {
            get
            {
                _instance ??= new BillSettingsService();
                return _instance;
            }
        }

        private BillSettingsService()
        {
            _databaseService = new DatabaseService();
        }

        // Bill Customization Properties
        public string GroceryName => _databaseService.GetBillSetting("grocery_name", "රසිංහ වෙළඳසැල");
        public string StoreAddress => _databaseService.GetBillSetting("store_address", "රන්දෙණිය, පිරිබැද්දර, කාගල්ල");
        public string PhoneNumber1 => _databaseService.GetBillSetting("phone_number_1", "0352263213");
        public string PhoneNumber2 => _databaseService.GetBillSetting("phone_number_2", "0763082845");
        public string CashierName => _databaseService.GetBillSetting("cashier_name", "Avindra Ranasinghe");
        public string LogoText => _databaseService.GetBillSetting("logo_text", "RS");
        public string ReceiptHeader => _databaseService.GetBillSetting("receipt_header", "Thank you for shopping with us!");
        public string ReceiptFooter => _databaseService.GetBillSetting("receipt_footer", "Please visit again!");
        public string CurrencySymbol => _databaseService.GetBillSetting("currency_symbol", "Rs.");
        public string TaxRate => _databaseService.GetBillSetting("tax_rate", "0");

        // General Settings Properties (these will be handled separately for now)
        public bool AutoPrintReceipts => false;
        public bool ShowStockWarnings => true;
        public bool EnableSoundNotifications => true;
        public bool AutoSaveBills => true;
        public bool ShowProductImages => false;

        // Theme and Language Properties (these will be handled separately for now)
        public string Theme => "Light";
        public string Language => "English";

        /// <summary>
        /// Get all bill settings as a dictionary for easy access
        /// </summary>
        public Dictionary<string, string> GetAllBillSettings()
        {
            return new Dictionary<string, string>
            {
                { "GroceryName", GroceryName },
                { "StoreAddress", StoreAddress },
                { "PhoneNumber1", PhoneNumber1 },
                { "PhoneNumber2", PhoneNumber2 },
                { "CashierName", CashierName },
                { "LogoText", LogoText },
                { "ReceiptHeader", ReceiptHeader },
                { "ReceiptFooter", ReceiptFooter },
                { "CurrencySymbol", CurrencySymbol },
                { "TaxRate", TaxRate }
            };
        }

        /// <summary>
        /// Refresh settings from database (useful after settings are updated)
        /// </summary>
        public void RefreshSettings()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== BillSettingsService.RefreshSettings called ===");
                
                // Create a new DatabaseService instance to ensure fresh connection
                var freshDbService = new DatabaseService();
                
                // Force reload by accessing properties with fresh database service
                var groceryName = freshDbService.GetBillSetting("grocery_name", "රසිංහ වෙළඳසැල");
                var storeAddress = freshDbService.GetBillSetting("store_address", "රන්දෙණිය, පිරිබැද්දර, කාගල්ල");
                var phoneNumber1 = freshDbService.GetBillSetting("phone_number_1", "0352263213");
                var phoneNumber2 = freshDbService.GetBillSetting("phone_number_2", "0763082845");
                var cashierName = freshDbService.GetBillSetting("cashier_name", "Avindra Ranasinghe");
                var logoText = freshDbService.GetBillSetting("logo_text", "RS");
                var receiptHeader = freshDbService.GetBillSetting("receipt_header", "Thank you for shopping with us!");
                var receiptFooter = freshDbService.GetBillSetting("receipt_footer", "Please visit again!");
                var currencySymbol = freshDbService.GetBillSetting("currency_symbol", "Rs.");
                var taxRate = freshDbService.GetBillSetting("tax_rate", "0");
                
                System.Diagnostics.Debug.WriteLine("Refreshed values from database:");
                System.Diagnostics.Debug.WriteLine($"  grocery_name: '{groceryName}'");
                System.Diagnostics.Debug.WriteLine($"  store_address: '{storeAddress}'");
                System.Diagnostics.Debug.WriteLine($"  phone_number_1: '{phoneNumber1}'");
                System.Diagnostics.Debug.WriteLine($"  phone_number_2: '{phoneNumber2}'");
                System.Diagnostics.Debug.WriteLine($"  cashier_name: '{cashierName}'");
                System.Diagnostics.Debug.WriteLine($"  logo_text: '{logoText}'");
                System.Diagnostics.Debug.WriteLine($"  receipt_header: '{receiptHeader}'");
                System.Diagnostics.Debug.WriteLine($"  receipt_footer: '{receiptFooter}'");
                System.Diagnostics.Debug.WriteLine($"  currency_symbol: '{currencySymbol}'");
                System.Diagnostics.Debug.WriteLine($"  tax_rate: '{taxRate}'");
                
                System.Diagnostics.Debug.WriteLine("=== BillSettingsService.RefreshSettings completed ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in RefreshSettings: {ex.Message}");
            }
        }

        /// <summary>
        /// Get formatted store information for receipts
        /// </summary>
        public string GetFormattedStoreInfo()
        {
            return $"{GroceryName}\n{StoreAddress}\n{PhoneNumber1} | {PhoneNumber2}";
        }

        /// <summary>
        /// Get formatted phone numbers
        /// </summary>
        public string GetFormattedPhoneNumbers()
        {
            return $"{PhoneNumber1} | {PhoneNumber2}";
        }
    }
} 