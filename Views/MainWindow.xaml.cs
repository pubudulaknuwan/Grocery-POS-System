using System.Windows;
using System.Windows.Controls;
using VillageSmartPOS.ViewModels;

namespace VillageSmartPOS.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Ensure the DataContext is set properly
            if (DataContext == null)
            {
                DataContext = new MainWindowViewModel();
            }
            
            // Force a UI refresh after initialization
            this.Loaded += (sender, e) =>
            {
                this.InvalidateVisual();
                
                // Force refresh of all content controls
                var contentControl = this.FindName("MainContentControl") as ContentControl;
                if (contentControl != null)
                {
                    contentControl.InvalidateVisual();
                }
                
                // Force refresh of all child elements
                RefreshAllChildElements(this);
                
                // Debug: Check if resources are available
                System.Diagnostics.Debug.WriteLine($"SalesBilling resource: {Application.Current.Resources["SalesBilling"]}");
                System.Diagnostics.Debug.WriteLine($"BillItems resource: {Application.Current.Resources["BillItems"]}");
                System.Diagnostics.Debug.WriteLine($"Total resource: {Application.Current.Resources["Total"]}");
            };
        }

        // Event handler for the "Clear Form" button click
        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SalesBillingViewModel viewModel)
            {
                viewModel.Name = string.Empty;
                viewModel.Barcode = string.Empty;
                viewModel.Price = 0;
                viewModel.QuantityText = "0";
            }
        }
        
        private void RefreshAllChildElements(FrameworkElement element)
        {
            if (element == null) return;
            
            // Force refresh of this element
            element.InvalidateVisual();
            element.UpdateLayout();
            
            // Recursively refresh all child elements
            var count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                if (child != null)
                {
                    RefreshAllChildElements(child);
                }
            }
        }
    }
}
