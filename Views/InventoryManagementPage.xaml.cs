using System.Windows.Controls;
using System.Windows.Input;
using VillageSmartPOS.ViewModels;

namespace VillageSmartPOS.Views
{
    public partial class InventoryManagementPage : UserControl
    {
        public InventoryManagementPage()
        {
            InitializeComponent();
            
            // Initialize Unit Measure ComboBox when page loads
            Loaded += (s, e) => 
            {
                InitializeProductUnitMeasureComboBox();
                
                // Set up the ViewModel to notify when product details are loaded
                if (DataContext is InventoryViewModel viewModel)
                {
                    viewModel.SetComboBoxSelections = SetComboBoxSelections;
                }
            };
        }

        /// <summary>
        /// Sets the ComboBox selections based on the loaded product data
        /// </summary>
        public void SetComboBoxSelections(string unitType, string unitMeasure)
        {
            // Set Unit Type selection
            if (ProductUnitTypeComboBox != null)
            {
                foreach (ComboBoxItem item in ProductUnitTypeComboBox.Items)
                {
                    if (item.Tag is string tag && tag == unitType)
                    {
                        ProductUnitTypeComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            
            // Set Unit Measure selection
            if (ProductUnitMeasureComboBox != null)
            {
                ProductUnitMeasureComboBox.SelectedItem = unitMeasure;
            }
        }

        /// <summary>
        /// Initializes the Product Unit Measure ComboBox with default options
        /// </summary>
        private void InitializeProductUnitMeasureComboBox()
        {
            // Check if ComboBox is initialized
            if (ProductUnitMeasureComboBox == null)
            {
                return;
            }
            
            // Clear existing items
            ProductUnitMeasureComboBox.Items.Clear();
            
            // Add default unit options (since Unit Type defaults to "unit")
            ProductUnitMeasureComboBox.Items.Add("pieces");
            ProductUnitMeasureComboBox.Items.Add("bottles");
            ProductUnitMeasureComboBox.Items.Add("packets");
            ProductUnitMeasureComboBox.Items.Add("boxes");
            ProductUnitMeasureComboBox.Items.Add("cans");
            ProductUnitMeasureComboBox.Items.Add("units");
        }

        /// <summary>
        /// Handles Product Unit Type ComboBox selection change to refresh Unit Measure options
        /// </summary>
        private void ProductUnitTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if ComboBoxes are initialized
            if (ProductUnitTypeComboBox == null || ProductUnitMeasureComboBox == null)
            {
                return;
            }
            
            // Get the selected ComboBoxItem and extract its Tag value
            if (ProductUnitTypeComboBox.SelectedItem is ComboBoxItem selectedItem && 
                selectedItem.Tag is string unitType && 
                DataContext is InventoryViewModel viewModel)
            {
                // Update the ViewModel's ProductUnitType property
                viewModel.ProductUnitType = unitType;
                
                // Store the current Unit Measure selection if it exists
                string currentUnitMeasure = ProductUnitMeasureComboBox.SelectedItem as string ?? "pieces";
                
                // Clear existing items from ProductUnitMeasureComboBox
                ProductUnitMeasureComboBox.Items.Clear();
                
                // Add appropriate items based on UnitType
                if (unitType == "mass")
                {
                    ProductUnitMeasureComboBox.Items.Add("kg");
                    ProductUnitMeasureComboBox.Items.Add("g");
                    ProductUnitMeasureComboBox.Items.Add("lb");
                    ProductUnitMeasureComboBox.Items.Add("oz");
                    
                    // Try to select the previous mass-based unit measure, or default to "kg"
                    string defaultMassMeasure = (currentUnitMeasure == "kg" || currentUnitMeasure == "g" || 
                                              currentUnitMeasure == "lb" || currentUnitMeasure == "oz") 
                                              ? currentUnitMeasure : "kg";
                    
                    ProductUnitMeasureComboBox.SelectedItem = defaultMassMeasure;
                    viewModel.ProductUnitMeasure = defaultMassMeasure;
                }
                else
                {
                    ProductUnitMeasureComboBox.Items.Add("pieces");
                    ProductUnitMeasureComboBox.Items.Add("bottles");
                    ProductUnitMeasureComboBox.Items.Add("packets");
                    ProductUnitMeasureComboBox.Items.Add("boxes");
                    ProductUnitMeasureComboBox.Items.Add("cans");
                    ProductUnitMeasureComboBox.Items.Add("units");
                    
                    // Try to select the previous unit-based unit measure, or default to "pieces"
                    string defaultUnitMeasure = (currentUnitMeasure == "pieces" || currentUnitMeasure == "bottles" || 
                                              currentUnitMeasure == "packets" || currentUnitMeasure == "boxes" || 
                                              currentUnitMeasure == "cans" || currentUnitMeasure == "units") 
                                              ? currentUnitMeasure : "pieces";
                    
                    ProductUnitMeasureComboBox.SelectedItem = defaultUnitMeasure;
                    viewModel.ProductUnitMeasure = defaultUnitMeasure;
                }
            }
        }

        /// <summary>
        /// Handles Product Unit Measure ComboBox selection change
        /// </summary>
        private void ProductUnitMeasureComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected item and update the ViewModel
            if (ProductUnitMeasureComboBox.SelectedItem is string selectedMeasure && 
                DataContext is InventoryViewModel viewModel)
            {
                viewModel.ProductUnitMeasure = selectedMeasure;
            }
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Handle mouse wheel scrolling for the DataGrid
            if (ProductsScrollViewer != null)
            {
                // Scroll the ScrollViewer instead of the DataGrid
                ProductsScrollViewer.ScrollToVerticalOffset(ProductsScrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }
    }
} 