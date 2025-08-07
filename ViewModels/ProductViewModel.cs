using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VillageSmartPOS.Helpers;
using VillageSmartPOS.Models;
using VillageSmartPOS.Services;

namespace VillageSmartPOS.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly DatabaseService dbService = new();

        // Properties bound to UI
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private string _barcode = string.Empty;
        public string Barcode
        {
            get => _barcode;
            set { _barcode = value; OnPropertyChanged(nameof(Barcode)); }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

        private decimal _markedPrice;
        public decimal MarkedPrice
        {
            get => _markedPrice;
            set { _markedPrice = value; OnPropertyChanged(nameof(MarkedPrice)); }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(nameof(Quantity)); }
        }

        private string _category = string.Empty;
        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(nameof(Category)); }
        }

        private string _supplier = string.Empty;
        public string Supplier
        {
            get => _supplier;
            set { _supplier = value; OnPropertyChanged(nameof(Supplier)); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        private int _reorderLevel = 10;
        public int ReorderLevel
        {
            get => _reorderLevel;
            set { _reorderLevel = value; OnPropertyChanged(nameof(ReorderLevel)); }
        }

        private string _unitType = "unit";
        public string UnitType
        {
            get => _unitType;
            set 
            { 
                System.Diagnostics.Debug.WriteLine($"UnitType setter called: {_unitType} -> {value}");
                _unitType = value; 
                OnPropertyChanged(nameof(UnitType)); 
                OnPropertyChanged(nameof(UnitMeasureOptions));
                
                // Update UnitMeasure to a valid option for the new UnitType
                if (value == "mass" && (_unitMeasure == "pieces" || _unitMeasure == "bottles" || _unitMeasure == "packets" || _unitMeasure == "boxes" || _unitMeasure == "cans" || _unitMeasure == "units"))
                {
                    UnitMeasure = "kg";
                }
                else if (value == "unit" && (_unitMeasure == "kg" || _unitMeasure == "g" || _unitMeasure == "lb" || _unitMeasure == "oz"))
                {
                    UnitMeasure = "pieces";
                }
                
                // Force refresh of UnitMeasureOptions
                OnPropertyChanged(nameof(UnitMeasureOptions));
            }
        }

        private string _unitMeasure = "pieces";
        public string UnitMeasure
        {
            get => _unitMeasure;
            set { _unitMeasure = value; OnPropertyChanged(nameof(UnitMeasure)); }
        }

        // Unit measure options based on unit type
        public List<string> UnitMeasureOptions
        {
            get
            {
                var options = UnitType == "mass" 
                    ? new List<string> { "kg", "g", "lb", "oz" }
                    : new List<string> { "pieces", "bottles", "packets", "boxes", "cans", "units" };
                
                System.Diagnostics.Debug.WriteLine($"UnitMeasureOptions: UnitType={UnitType}, Options={string.Join(", ", options)}");
                return options;
            }
        }

        public ObservableCollection<Product> Products { get; set; } = new();

        public ICommand AddProductCommand { get; }
        public ICommand ClearFormCommand { get; }

        public ProductViewModel()
        {
            AddProductCommand = new RelayCommand(AddProduct);
            ClearFormCommand = new RelayCommand(ClearForm);
            LoadProducts(); // Load existing products from DB
        }

        private void LoadProducts()
        {
            try
            {
                var loadedProducts = dbService.GetAllProducts();
                Products.Clear();
                foreach (var product in loadedProducts)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading products: {ex.Message}");
            }
        }

        private void AddProduct()
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(Name))
                {
                    MessageBox.Show("Please enter a product name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Barcode))
                {
                    MessageBox.Show("Please enter a barcode.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Price <= 0)
                {
                    MessageBox.Show("Please enter a valid price greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Allow 0 marked price for products without marked prices (like fresh produce, bulk items)
                if (MarkedPrice < 0)
                {
                    MessageBox.Show("Marked price cannot be negative.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Quantity < 0)
                {
                    MessageBox.Show("Quantity cannot be negative.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                dbService.AddProduct(Name, Barcode, Price, MarkedPrice, Quantity, UnitType, UnitMeasure, Category, Supplier, Description);
                LoadProducts(); // Reload the list

                // Show success message
                MessageBox.Show($"Product '{Name}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear inputs
                ClearForm();
            }
            catch (InvalidOperationException ex)
            {
                // Handle duplicate barcode error
                MessageBox.Show(ex.Message, "Duplicate Barcode", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Error adding product: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Barcode = string.Empty;
            Price = 0;
            MarkedPrice = 0;
            Quantity = 0;
            UnitType = "unit";
            UnitMeasure = "pieces";
            Category = string.Empty;
            Supplier = string.Empty;
            Description = string.Empty;
            ReorderLevel = 10;
        }
    }
}
