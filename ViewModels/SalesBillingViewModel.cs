using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using VillageSmartPOS.Helpers;
using VillageSmartPOS.Models;
using VillageSmartPOS.Services;
using VillageSmartPOS.Views;

namespace VillageSmartPOS.ViewModels
{
    public class SalesBillingViewModel : BaseViewModel
    {
        private readonly DatabaseService dbService = new();

        private string searchTerm = string.Empty;
        public string SearchTerm
        {
            get => searchTerm;
            set 
            { 
                searchTerm = value; 
                OnPropertyChanged();
                // Auto-search when user types
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SearchProduct();
                }
            }
        }

        private string quantityText = "1";
        public string QuantityText
        {
            get => quantityText;
            set { quantityText = value; OnPropertyChanged(); }
        }

        public decimal Quantity
        {
            get => decimal.TryParse(quantityText, out decimal result) ? result : 0;
        }

        private string name = string.Empty;
        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(); }
        }

        private string barcode = string.Empty;
        public string Barcode
        {
            get => barcode;
            set { barcode = value; OnPropertyChanged(); }
        }

        private decimal price = 0;
        public decimal Price
        {
            get => price;
            set { price = value; OnPropertyChanged(); }
        }

        private string statusMessage = string.Empty;
        public string StatusMessage
        {
            get => statusMessage;
            set { statusMessage = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Product> searchSuggestions = new();
        public ObservableCollection<Product> SearchSuggestions
        {
            get => searchSuggestions;
            set { searchSuggestions = value; OnPropertyChanged(); }
        }

        // Manual Product Entry Properties
        private string manualProductName = string.Empty;
        public string ManualProductName
        {
            get => manualProductName;
            set { manualProductName = value; OnPropertyChanged(); }
        }

        private string manualProductPrice = string.Empty;
        public string ManualProductPrice
        {
            get => manualProductPrice;
            set { manualProductPrice = value; OnPropertyChanged(); }
        }

        private string manualProductQuantity = "1";
        public string ManualProductQuantity
        {
            get => manualProductQuantity;
            set { manualProductQuantity = value; OnPropertyChanged(); }
        }

        public ObservableCollection<BillItem> BillItems { get; set; } = new();

        public decimal TotalAmount => BillItems.Sum(item => item.Total);

        public ICommand AddToBillCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand ClearBillCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand SelectProductCommand { get; }
        public ICommand AddManualProductCommand { get; }

        public SalesBillingViewModel()
        {
            AddToBillCommand = new RelayCommand(AddToBill, CanAddToBill);
            PrintCommand = new RelayCommand(PrintReceipt, CanPrintReceipt);
            ClearBillCommand = new RelayCommand(ClearBill, CanClearBill);
            RemoveItemCommand = new RelayCommand<BillItem>(RemoveItem);
            SelectProductCommand = new RelayCommand<Product>(SelectProduct);
            AddManualProductCommand = new RelayCommand(AddManualProduct, CanAddManualProduct);
            
            // Add some test data for debugging
            //AddTestData();
        }

        private bool CanAddToBill()
        {
            return !string.IsNullOrWhiteSpace(SearchTerm) && Quantity > 0 && Price > 0;
        }

        private bool CanPrintReceipt()
        {
            return BillItems.Count > 0;
        }

        private bool CanClearBill()
        {
            return BillItems.Count > 0;
        }

        private void SearchProduct()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm)) 
            {
                SearchSuggestions.Clear();
                return;
            }

            // Get suggestions for dropdown
            var suggestions = dbService.GetProductSuggestions(SearchTerm, 8);
            SearchSuggestions.Clear();
            foreach (var suggestion in suggestions)
            {
                SearchSuggestions.Add(suggestion);
            }

            // Try to find exact match or best match
            var product = dbService.GetProductByNameOrBarcode(SearchTerm);
            if (product != null)
            {
                Name = product.Name;
                Barcode = product.Barcode;
                Price = product.Price;
                StatusMessage = $"Found: {product.Name} - Stock: {product.Quantity}";
            }
            else
            {
                Name = string.Empty;
                Barcode = string.Empty;
                Price = 0;
                if (SearchSuggestions.Count > 0)
                {
                    StatusMessage = $"Found {SearchSuggestions.Count} similar products. Select from dropdown or continue typing.";
                }
                else
                {
                    StatusMessage = "Product not found";
                }
            }
        }

        private void AddToBill()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm)) 
            {
                StatusMessage = "Please enter a product name or barcode";
                return;
            }

            var product = dbService.GetProductByNameOrBarcode(SearchTerm);
            if (product == null)
            {
                StatusMessage = "Product not found. Try searching by product name or barcode.";
                return;
            }

            // Check if we have enough stock - allow with warning for empty quantities
            if (product.Quantity < Quantity)
            {
                if (product.Quantity == 0)
                {
                    // Allow adding products with empty quantity but show warning
                    StatusMessage = $"⚠️ WARNING: {product.Name} has no stock quantity set. Product added to bill anyway.";
                }
                else
                {
                    // For products with some stock but insufficient, show error
                    StatusMessage = $"Insufficient stock. Available: {product.Quantity}";
                    return;
                }
            }

            var existingItem = BillItems.FirstOrDefault(i => i.Barcode == product.Barcode);
            if (existingItem != null)
            {
                existingItem.Quantity += Quantity;
                StatusMessage = $"Updated quantity for {product.Name}";
            }
            else
            {
                var newItem = new BillItem
                {
                    Name = product.Name,
                    Barcode = product.Barcode,
                    Price = Price, // Use the current Price (which could be modified by user)
                    MarkedPrice = product.MarkedPrice,
                    Quantity = Quantity,
                    UnitType = product.UnitType,
                    UnitMeasure = product.UnitMeasure
                };
                
                BillItems.Add(newItem);
                StatusMessage = $"Added {product.Name} to bill (Total items: {BillItems.Count})";
            }

            // Update stock in database (only if product has stock quantity set)
            if (product.Quantity > 0)
            {
                dbService.UpdateStockAfterSale(product.Barcode, Quantity);
            }

            // Notify UI
            OnPropertyChanged(nameof(BillItems));
            OnPropertyChanged(nameof(TotalAmount));

            // Reset input
            SearchTerm = string.Empty;
            QuantityText = "1";
            Name = string.Empty;
            Barcode = string.Empty;
            Price = 0;
            OnPropertyChanged(nameof(SearchTerm));
            OnPropertyChanged(nameof(QuantityText));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Barcode));
            OnPropertyChanged(nameof(Price));
        }

        private void ClearBill()
        {
            BillItems.Clear();
            OnPropertyChanged(nameof(BillItems));
            OnPropertyChanged(nameof(TotalAmount));
            StatusMessage = "Bill cleared";
        }

        private void RemoveItem(BillItem? item)
        {
            if (item != null)
            {
                BillItems.Remove(item);
                OnPropertyChanged(nameof(BillItems));
                OnPropertyChanged(nameof(TotalAmount));
                StatusMessage = $"Removed {item.Name} from bill";
            }
        }

        private void SelectProduct(Product? product)
        {
            if (product != null)
            {
                SearchTerm = product.Name;
                Name = product.Name;
                Barcode = product.Barcode;
                Price = product.Price;
                SearchSuggestions.Clear();
                StatusMessage = $"Selected: {product.Name} - Stock: {product.Quantity}";
            }
        }

        private void PrintReceipt()
        {
            if (BillItems.Count == 0)
            {
                StatusMessage = "No items in bill to print";
                return;
            }

            var billData = new BillReceiptViewModel
            {
                Items = new ObservableCollection<BillItem>(BillItems),
                TotalAmount = TotalAmount,
                PaidAmount = TotalAmount
            };

            var billView = new BillReceiptView
            {
                DataContext = billData
            };

            var previewWindow = new BillReceiptPreview(billView, billData);
            previewWindow.ShowDialog();
            
            StatusMessage = "Receipt printed successfully";
        }

        private void AddTestData()
        {
            BillItems.Add(new BillItem
            {
                Name = "Test Product 1",
                Barcode = "123456789",
                Price = 10.00m,
                MarkedPrice = 12.00m,
                Quantity = 2.0m
            });

            BillItems.Add(new BillItem
            {
                Name = "Test Product 2",
                Barcode = "987654321",
                Price = 15.50m,
                MarkedPrice = 15.50m,
                Quantity = 1.0m
            });

            OnPropertyChanged(nameof(BillItems));
            OnPropertyChanged(nameof(TotalAmount));
        }

        // Manual Product Entry Methods
        private bool CanAddManualProduct()
        {
            return !string.IsNullOrWhiteSpace(ManualProductName) && 
                   !string.IsNullOrWhiteSpace(ManualProductPrice) &&
                   decimal.TryParse(ManualProductPrice, out decimal price) && 
                   price > 0;
        }

        private void AddManualProduct()
        {
            if (!CanAddManualProduct())
            {
                StatusMessage = "Please enter valid product name and price";
                return;
            }

            if (!decimal.TryParse(ManualProductPrice, out decimal price))
            {
                StatusMessage = "Please enter a valid price";
                return;
            }

            if (!decimal.TryParse(ManualProductQuantity, out decimal quantity))
            {
                quantity = 1; // Default to 1 if invalid quantity
            }

            if (quantity <= 0)
            {
                quantity = 1; // Default to 1 if quantity is 0 or negative
            }

            var manualItem = new BillItem
            {
                Name = ManualProductName.Trim(),
                Barcode = "", // No barcode for manual products
                Price = price,
                MarkedPrice = price, // Same as price for manual products
                Quantity = quantity,
                UnitType = "unit",
                UnitMeasure = "pieces"
            };

            BillItems.Add(manualItem);
            OnPropertyChanged(nameof(BillItems));
            OnPropertyChanged(nameof(TotalAmount));

            // Clear manual input fields
            ManualProductName = string.Empty;
            ManualProductPrice = string.Empty;
            ManualProductQuantity = "1";

            StatusMessage = $"Added manual product: {manualItem.Name} - Rs. {price:F2}";
        }
    }
}
