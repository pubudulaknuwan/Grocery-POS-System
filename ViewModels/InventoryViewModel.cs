using System.Collections.ObjectModel;
using System.Windows.Input;
using VillageSmartPOS.Helpers;
using VillageSmartPOS.Models;
using VillageSmartPOS.Services;

namespace VillageSmartPOS.ViewModels
{
    public class InventoryViewModel : BaseViewModel
    {
        private readonly DatabaseService dbService = new();

        // Simple properties without complex notifications
        private string searchTerm = string.Empty;
        public string SearchTerm
        {
            get => searchTerm;
            set { searchTerm = value; OnPropertyChanged(); }
        }

        private Product? selectedProduct;
        public Product? SelectedProduct
        {
            get => selectedProduct;
            set { selectedProduct = value; OnPropertyChanged(); }
        }

        // Product details for editing
        private string productName = string.Empty;
        public string ProductName
        {
            get => productName;
            set { productName = value; OnPropertyChanged(); }
        }

        private string productBarcode = string.Empty;
        public string ProductBarcode
        {
            get => productBarcode;
            set { productBarcode = value; OnPropertyChanged(); }
        }

        private decimal productPrice;
        public decimal ProductPrice
        {
            get => productPrice;
            set { productPrice = value; OnPropertyChanged(); }
        }

        private decimal productMarkedPrice;
        public decimal ProductMarkedPrice
        {
            get => productMarkedPrice;
            set { productMarkedPrice = value; OnPropertyChanged(); }
        }

        private int productQuantity;
        public int ProductQuantity
        {
            get => productQuantity;
            set { productQuantity = value; OnPropertyChanged(); }
        }

        private string productCategory = string.Empty;
        public string ProductCategory
        {
            get => productCategory;
            set { productCategory = value; OnPropertyChanged(); }
        }

        private string productSupplier = string.Empty;
        public string ProductSupplier
        {
            get => productSupplier;
            set { productSupplier = value; OnPropertyChanged(); }
        }

        private string productDescription = string.Empty;
        public string ProductDescription
        {
            get => productDescription;
            set { productDescription = value; OnPropertyChanged(); }
        }

        private int productReorderLevel = 10;
        public int ProductReorderLevel
        {
            get => productReorderLevel;
            set { productReorderLevel = value; OnPropertyChanged(); }
        }

        // Collections
        public ObservableCollection<Product> Products { get; set; } = new();
        public ObservableCollection<Product> LowStockProducts { get; set; } = new();

        // Commands
        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand UpdateProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand ClearFormCommand { get; }
        public ICommand ShowLowStockCommand { get; }
        public ICommand LoadDetailsCommand { get; }

        public InventoryViewModel()
        {
            RefreshCommand = new RelayCommand(LoadProducts);
            SearchCommand = new RelayCommand(SearchProducts);
            UpdateProductCommand = new RelayCommand(UpdateProduct);
            DeleteProductCommand = new RelayCommand(DeleteProduct);
            ClearFormCommand = new RelayCommand(ClearForm);
            ShowLowStockCommand = new RelayCommand(ShowLowStock);
            LoadDetailsCommand = new RelayCommand(LoadProductDetails);

            LoadProducts();
            LoadLowStockProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var products = dbService.GetAllProducts();
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading products: {ex.Message}");
            }
        }

        private void LoadLowStockProducts()
        {
            try
            {
                var lowStockProducts = dbService.GetLowStockProducts();
                LowStockProducts.Clear();
                foreach (var product in lowStockProducts)
                {
                    LowStockProducts.Add(product);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading low stock products: {ex.Message}");
            }
        }

        private void SearchProducts()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchTerm))
                {
                    LoadProducts();
                    return;
                }

                var searchResults = dbService.SearchProducts(SearchTerm);
                Products.Clear();
                foreach (var product in searchResults)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching products: {ex.Message}");
            }
        }

        private void LoadProductDetails()
        {
            try
            {
                if (SelectedProduct == null)
                {
                    ClearForm();
                    return;
                }

                ProductName = SelectedProduct.Name;
                ProductBarcode = SelectedProduct.Barcode;
                ProductPrice = SelectedProduct.Price;
                ProductMarkedPrice = SelectedProduct.MarkedPrice;
                ProductQuantity = SelectedProduct.Quantity;
                ProductCategory = SelectedProduct.Category;
                ProductSupplier = SelectedProduct.Supplier;
                ProductDescription = SelectedProduct.Description;
                ProductReorderLevel = SelectedProduct.ReorderLevel;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading product details: {ex.Message}");
            }
        }

        private void UpdateProduct()
        {
            try
            {
                if (SelectedProduct == null || string.IsNullOrWhiteSpace(ProductName) || string.IsNullOrWhiteSpace(ProductBarcode))
                {
                    return;
                }

                // Update database
                dbService.UpdateProduct(SelectedProduct.Id, ProductName, ProductBarcode, ProductPrice, 
                                     ProductMarkedPrice, ProductQuantity, ProductCategory, ProductSupplier, ProductDescription);
                
                // Simple reload without complex notifications
                LoadProducts();
                LoadLowStockProducts();
                ClearForm();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating product: {ex.Message}");
            }
        }

        private void DeleteProduct()
        {
            try
            {
                if (SelectedProduct == null)
                {
                    return;
                }

                // Delete from database
                dbService.DeleteProduct(SelectedProduct.Id);
                
                // Simple reload without complex notifications
                LoadProducts();
                LoadLowStockProducts();
                ClearForm();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting product: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            ProductName = string.Empty;
            ProductBarcode = string.Empty;
            ProductPrice = 0;
            ProductMarkedPrice = 0;
            ProductQuantity = 0;
            ProductCategory = string.Empty;
            ProductSupplier = string.Empty;
            ProductDescription = string.Empty;
            ProductReorderLevel = 10;
            SelectedProduct = null;
        }

        private void ShowLowStock()
        {
            try
            {
                var lowStockProducts = dbService.GetLowStockProducts();
                Products.Clear();
                foreach (var product in lowStockProducts)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing low stock: {ex.Message}");
            }
        }
    }
} 