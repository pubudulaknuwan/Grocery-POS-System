using System;
using System.Windows;
using System.Windows.Controls;
using VillageSmartPOS.ViewModels;
using VillageSmartPOS.Services;

namespace VillageSmartPOS.Views
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        private SettingsViewModel _viewModel;

        public SettingsPage()
        {
            InitializeComponent();
            _viewModel = new SettingsViewModel();
            DataContext = _viewModel;
        }

        private void LoadSettings()
        {
            // Load current theme
            string currentTheme = ThemeService.Instance.GetCurrentTheme();
            if (currentTheme == "Dark")
            {
                ThemeComboBox.SelectedIndex = 1; // Dark theme
            }
            else
            {
                ThemeComboBox.SelectedIndex = 0; // Light theme
            }
            
            // Load current language
            string currentLanguage = LanguageService.Instance.GetCurrentLanguage();
            if (currentLanguage == "Sinhala")
            {
                LanguageComboBox.SelectedIndex = 1; // Sinhala
            }
            else
            {
                LanguageComboBox.SelectedIndex = 0; // English
            }
            
            // Set default values for general settings
            AutoPrintCheckBox.IsChecked = _viewModel.AutoPrintReceipts;
            StockWarningsCheckBox.IsChecked = _viewModel.ShowStockWarnings;
            SoundNotificationsCheckBox.IsChecked = _viewModel.EnableSoundNotifications;
            AutoSaveBillsCheckBox.IsChecked = _viewModel.AutoSaveBills;
            ShowProductImagesCheckBox.IsChecked = _viewModel.ShowProductImages;
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string theme = selectedItem.Tag?.ToString() ?? "Light";
                System.Diagnostics.Debug.WriteLine($"Theme changed to: {theme}");
                
                // Update ViewModel
                _viewModel.SelectedTheme = theme;
                
                // Apply the selected theme
                ThemeService.Instance.ApplyTheme(theme);
                
                // Show confirmation message
                MessageBox.Show($"Theme changed to: {theme}", "Theme Change", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string language = selectedItem.Tag?.ToString() ?? "English";
                System.Diagnostics.Debug.WriteLine($"Language changed to: {language}");
                
                // Update ViewModel
                _viewModel.SelectedLanguage = language;
                
                // Apply the selected language
                LanguageService.Instance.ApplyLanguage(language);
                
                // Show confirmation message
                MessageBox.Show($"Language changed to: {language}", "Language Change", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AutoPrintCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            bool isChecked = AutoPrintCheckBox.IsChecked ?? false;
            _viewModel.AutoPrintReceipts = isChecked;
            System.Diagnostics.Debug.WriteLine($"Auto-print receipts: {isChecked}");
        }

        private void StockWarningsCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            bool isChecked = StockWarningsCheckBox.IsChecked ?? false;
            _viewModel.ShowStockWarnings = isChecked;
            System.Diagnostics.Debug.WriteLine($"Show stock warnings: {isChecked}");
        }

        private void SoundNotificationsCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            bool isChecked = SoundNotificationsCheckBox.IsChecked ?? false;
            _viewModel.EnableSoundNotifications = isChecked;
            System.Diagnostics.Debug.WriteLine($"Sound notifications: {isChecked}");
        }

        private void AutoSaveBillsCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            bool isChecked = AutoSaveBillsCheckBox.IsChecked ?? false;
            _viewModel.AutoSaveBills = isChecked;
            System.Diagnostics.Debug.WriteLine($"Auto-save bills: {isChecked}");
        }

        private void ShowProductImagesCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            bool isChecked = ShowProductImagesCheckBox.IsChecked ?? false;
            _viewModel.ShowProductImages = isChecked;
            System.Diagnostics.Debug.WriteLine($"Show product images: {isChecked}");
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("SaveSettings_Click called");
                System.Diagnostics.Debug.WriteLine($"Current GroceryName in ViewModel: {_viewModel.GroceryName}");
                
                // Save settings using ViewModel
                _viewModel.SaveSettingsCommand.Execute(null);
                
                MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SaveSettings_Click: {ex.Message}");
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reset all settings to default values?", 
                                       "Reset Settings", 
                                       MessageBoxButton.YesNo, 
                                       MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                // Reset settings using ViewModel
                _viewModel.ResetSettingsCommand.Execute(null);
                
                // Reload settings
                LoadSettings();
                
                MessageBox.Show("Settings reset to default values!", "Reset Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveBillSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== SaveBillSettings_Click called ===");
                
                // Get all current values from the UI textboxes directly
                string uiGroceryName = GroceryNameTextBox.Text;
                string uiStoreAddress = StoreAddressTextBox.Text;
                string uiPhoneNumber1 = PhoneNumber1TextBox.Text;
                string uiPhoneNumber2 = PhoneNumber2TextBox.Text;
                string uiCashierName = CashierNameTextBox.Text;
                string uiLogoText = LogoTextTextBox.Text;
                string uiReceiptHeader = ReceiptHeaderTextBox.Text;
                string uiReceiptFooter = ReceiptFooterTextBox.Text;
                string uiCurrencySymbol = CurrencySymbolTextBox.Text;
                
                System.Diagnostics.Debug.WriteLine("UI TextBox values:");
                System.Diagnostics.Debug.WriteLine($"  GroceryName: '{uiGroceryName}'");
                System.Diagnostics.Debug.WriteLine($"  StoreAddress: '{uiStoreAddress}'");
                System.Diagnostics.Debug.WriteLine($"  PhoneNumber1: '{uiPhoneNumber1}'");
                System.Diagnostics.Debug.WriteLine($"  PhoneNumber2: '{uiPhoneNumber2}'");
                System.Diagnostics.Debug.WriteLine($"  CashierName: '{uiCashierName}'");
                System.Diagnostics.Debug.WriteLine($"  LogoText: '{uiLogoText}'");
                System.Diagnostics.Debug.WriteLine($"  ReceiptHeader: '{uiReceiptHeader}'");
                System.Diagnostics.Debug.WriteLine($"  ReceiptFooter: '{uiReceiptFooter}'");
                System.Diagnostics.Debug.WriteLine($"  CurrencySymbol: '{uiCurrencySymbol}'");
                
                System.Diagnostics.Debug.WriteLine("ViewModel values before update:");
                System.Diagnostics.Debug.WriteLine($"  GroceryName: '{_viewModel.GroceryName}'");
                System.Diagnostics.Debug.WriteLine($"  StoreAddress: '{_viewModel.StoreAddress}'");
                System.Diagnostics.Debug.WriteLine($"  PhoneNumber1: '{_viewModel.PhoneNumber1}'");
                System.Diagnostics.Debug.WriteLine($"  PhoneNumber2: '{_viewModel.PhoneNumber2}'");
                System.Diagnostics.Debug.WriteLine($"  CashierName: '{_viewModel.CashierName}'");
                System.Diagnostics.Debug.WriteLine($"  LogoText: '{_viewModel.LogoText}'");
                System.Diagnostics.Debug.WriteLine($"  ReceiptHeader: '{_viewModel.ReceiptHeader}'");
                System.Diagnostics.Debug.WriteLine($"  ReceiptFooter: '{_viewModel.ReceiptFooter}'");
                System.Diagnostics.Debug.WriteLine($"  CurrencySymbol: '{_viewModel.CurrencySymbol}'");
                
                // Force update the ViewModel with all UI values
                _viewModel.GroceryName = uiGroceryName;
                _viewModel.StoreAddress = uiStoreAddress;
                _viewModel.PhoneNumber1 = uiPhoneNumber1;
                _viewModel.PhoneNumber2 = uiPhoneNumber2;
                _viewModel.CashierName = uiCashierName;
                _viewModel.LogoText = uiLogoText;
                _viewModel.ReceiptHeader = uiReceiptHeader;
                _viewModel.ReceiptFooter = uiReceiptFooter;
                _viewModel.CurrencySymbol = uiCurrencySymbol;
                
                System.Diagnostics.Debug.WriteLine("ViewModel values after update:");
                System.Diagnostics.Debug.WriteLine($"  GroceryName: '{_viewModel.GroceryName}'");
                System.Diagnostics.Debug.WriteLine($"  StoreAddress: '{_viewModel.StoreAddress}'");
                System.Diagnostics.Debug.WriteLine($"  PhoneNumber1: '{_viewModel.PhoneNumber1}'");
                System.Diagnostics.Debug.WriteLine($"  PhoneNumber2: '{_viewModel.PhoneNumber2}'");
                System.Diagnostics.Debug.WriteLine($"  CashierName: '{_viewModel.CashierName}'");
                System.Diagnostics.Debug.WriteLine($"  LogoText: '{_viewModel.LogoText}'");
                System.Diagnostics.Debug.WriteLine($"  ReceiptHeader: '{_viewModel.ReceiptHeader}'");
                System.Diagnostics.Debug.WriteLine($"  ReceiptFooter: '{_viewModel.ReceiptFooter}'");
                System.Diagnostics.Debug.WriteLine($"  CurrencySymbol: '{_viewModel.CurrencySymbol}'");
                
                // Call the ViewModel's SaveSettings method directly
                _viewModel.SaveSettings();
                
                MessageBox.Show("Bill settings saved successfully to database!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                System.Diagnostics.Debug.WriteLine("=== SaveBillSettings_Click completed successfully ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in SaveBillSettings_Click: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Error saving bill settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 