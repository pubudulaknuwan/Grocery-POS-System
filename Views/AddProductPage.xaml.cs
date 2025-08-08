using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VillageSmartPOS.ViewModels;

namespace VillageSmartPOS.Views
{
    /// <summary>
    /// Interaction logic for AddProductPage.xaml
    /// </summary>
    public partial class AddProductPage : UserControl
    {
        public AddProductPage()
        {
            InitializeComponent();
            
            // Set up the delegate to reset form ComboBoxes after XAML is loaded
            Loaded += (s, e) => 
            {
                // Initialize Unit Measure ComboBox with default options
                InitializeUnitMeasureComboBox();
                
                // Set up the delegate to reset form ComboBoxes
                if (DataContext is ProductViewModel viewModel)
                {
                    viewModel.ResetFormComboBoxes = ResetFormComboBoxes;
                    viewModel.FocusProductNameField = FocusProductNameField;
                }
                
                // Focus Product Name field when page loads
                FocusProductNameField();
            };
        }

        /// <summary>
        /// Initializes the Unit Measure ComboBox with default options
        /// </summary>
        private void InitializeUnitMeasureComboBox()
        {
            // Check if ComboBox is initialized
            if (UnitMeasureComboBox == null)
            {
                return;
            }
            
            // Clear existing items
            UnitMeasureComboBox.Items.Clear();
            
            // Add default unit options (since Unit Type defaults to "unit")
            UnitMeasureComboBox.Items.Add("pieces");
            UnitMeasureComboBox.Items.Add("bottles");
            UnitMeasureComboBox.Items.Add("packets");
            UnitMeasureComboBox.Items.Add("boxes");
            UnitMeasureComboBox.Items.Add("cans");
            UnitMeasureComboBox.Items.Add("units");
            
            // Set default selection
            UnitMeasureComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles keyboard navigation between form fields
        /// </summary>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Prevent default Enter behavior
                
                // Get the current control that triggered the event
                Control? currentControl = sender as Control;
                if (currentControl == null) return;

                // Define the navigation order for the form fields
                Control? nextControl = GetNextControl(currentControl);
                
                if (nextControl != null)
                {
                    // Move focus to the next control
                    nextControl.Focus();
                    
                    // If it's a TextBox, select all text for easy replacement
                    if (nextControl is TextBox textBox)
                    {
                        textBox.SelectAll();
                    }
                }
                else
                {
                    // If we're at the last field, trigger the Add Product button
                    TriggerAddProduct();
                }
            }
        }

        /// <summary>
        /// Determines the next control in the navigation order
        /// </summary>
        private Control? GetNextControl(Control currentControl)
        {
            // Define the navigation order (left to right, top to bottom)
            var navigationOrder = new List<Control>
            {
                ProductNameTextBox,    // 1. Product Name
                BarcodeTextBox,        // 2. Barcode
                PriceTextBox,          // 3. Our Price
                MarkedPriceTextBox,    // 4. Marked Price
                QuantityTextBox,       // 5. Quantity
                UnitTypeComboBox,      // 6. Unit Type
                UnitMeasureComboBox,   // 7. Unit Measure
                CategoryTextBox,       // 8. Category
                SupplierTextBox,       // 9. Supplier
                ReorderLevelTextBox,   // 10. Reorder Level
                DescriptionTextBox,    // 11. Description
                AddProductButton,      // 12. Add Product Button
                ClearFormButton        // 13. Clear Form Button
            };

            // Find the current control in the navigation order
            int currentIndex = navigationOrder.IndexOf(currentControl);
            
            // Return the next control in the sequence
            if (currentIndex >= 0 && currentIndex < navigationOrder.Count - 1)
            {
                return navigationOrder[currentIndex + 1];
            }
            
            return null; // No next control (we're at the end)
        }

        /// <summary>
        /// Triggers the Add Product button when Enter is pressed on the last field
        /// </summary>
        private void TriggerAddProduct()
        {
            // Simply focus the Add Product button, just like navigating to the next field
            if (AddProductButton != null)
            {
                AddProductButton.Focus();
            }
        }

        /// <summary>
        /// Helper method to find a visual child by name
        /// </summary>
        private T? FindVisualChild<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                
                if (child is T result && child is FrameworkElement element && element.Name == name)
                {
                    return result;
                }
                
                var descendant = FindVisualChild<T>(child, name);
                if (descendant != null)
                {
                    return descendant;
                }
            }
            return null;
        }

        /// <summary>
        /// Handles Enter key press on the Add Product button
        /// </summary>
        private void AddProductButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Prevent default Enter behavior
                
                // Execute the Add Product command
                if (AddProductButton != null && AddProductButton.Command != null && AddProductButton.Command.CanExecute(null))
                {
                    AddProductButton.Command.Execute(null);
                }
            }
        }

        /// <summary>
        /// Handles Enter key press on the Clear Form button
        /// </summary>
        private void ClearFormButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Prevent default Enter behavior
                
                // Execute the Clear Form command
                if (ClearFormButton != null && ClearFormButton.Command != null && ClearFormButton.Command.CanExecute(null))
                {
                    ClearFormButton.Command.Execute(null);
                }
                
                // Focus Product Name field after clearing
                FocusProductNameField();
            }
        }

        /// <summary>
        /// Handles Unit Type ComboBox selection change to refresh Unit Measure options
        /// </summary>
        private void UnitTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if ComboBoxes are initialized
            if (UnitTypeComboBox == null || UnitMeasureComboBox == null)
            {
                return;
            }
            
            // Get the selected ComboBoxItem and extract its Tag value
            if (UnitTypeComboBox.SelectedItem is ComboBoxItem selectedItem && 
                selectedItem.Tag is string unitType && 
                DataContext is ProductViewModel viewModel)
            {
                // Update the ViewModel's UnitType property
                viewModel.UnitType = unitType;
                
                // Store the current Unit Measure selection if it exists
                string currentUnitMeasure = UnitMeasureComboBox.SelectedItem as string ?? "pieces";
                
                // Clear existing items from UnitMeasureComboBox
                UnitMeasureComboBox.Items.Clear();
                
                // Add appropriate items based on UnitType
                if (unitType == "mass")
                {
                    UnitMeasureComboBox.Items.Add("kg");
                    UnitMeasureComboBox.Items.Add("g");
                    UnitMeasureComboBox.Items.Add("lb");
                    UnitMeasureComboBox.Items.Add("oz");
                    
                    // Try to select the previous mass-based unit measure, or default to "kg"
                    string defaultMassMeasure = (currentUnitMeasure == "kg" || currentUnitMeasure == "g" || 
                                              currentUnitMeasure == "lb" || currentUnitMeasure == "oz") 
                                              ? currentUnitMeasure : "kg";
                    
                    UnitMeasureComboBox.SelectedItem = defaultMassMeasure;
                    viewModel.UnitMeasure = defaultMassMeasure;
                }
                else
                {
                    UnitMeasureComboBox.Items.Add("pieces");
                    UnitMeasureComboBox.Items.Add("bottles");
                    UnitMeasureComboBox.Items.Add("packets");
                    UnitMeasureComboBox.Items.Add("boxes");
                    UnitMeasureComboBox.Items.Add("cans");
                    UnitMeasureComboBox.Items.Add("units");
                    
                    // Try to select the previous unit-based unit measure, or default to "pieces"
                    string defaultUnitMeasure = (currentUnitMeasure == "pieces" || currentUnitMeasure == "bottles" || 
                                              currentUnitMeasure == "packets" || currentUnitMeasure == "boxes" || 
                                              currentUnitMeasure == "cans" || currentUnitMeasure == "units") 
                                              ? currentUnitMeasure : "pieces";
                    
                    UnitMeasureComboBox.SelectedItem = defaultUnitMeasure;
                    viewModel.UnitMeasure = defaultUnitMeasure;
                }
            }
        }

        /// <summary>
        /// Handles Unit Measure ComboBox selection change
        /// </summary>
        private void UnitMeasureComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected item and update the ViewModel
            if (UnitMeasureComboBox.SelectedItem is string selectedMeasure && 
                DataContext is ProductViewModel viewModel)
            {
                viewModel.UnitMeasure = selectedMeasure;
            }
        }

        /// <summary>
        /// Resets the form ComboBoxes to their default state
        /// </summary>
        public void ResetFormComboBoxes()
        {
            // Check if ComboBoxes are initialized
            if (UnitTypeComboBox == null || UnitMeasureComboBox == null)
            {
                return;
            }
            
            // Reset Unit Type to default
            if (UnitTypeComboBox.Items.Count > 0)
            {
                UnitTypeComboBox.SelectedIndex = 0;
            }
            
            // Reset Unit Measure to default
            InitializeUnitMeasureComboBox();
        }

        /// <summary>
        /// Focuses the Product Name TextBox
        /// </summary>
        public void FocusProductNameField()
        {
            if (ProductNameTextBox != null)
            {
                ProductNameTextBox.Focus();
                ProductNameTextBox.SelectAll();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
