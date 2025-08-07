using VillageSmartPOS.Services;

namespace VillageSmartPOS.Models
{
    public class BillItem
    {
        private readonly BillSettingsService _billSettings = BillSettingsService.Instance;

        public required string Name { get; set; }
        public required string Barcode { get; set; }
        public decimal Price { get; set; }
        public decimal MarkedPrice { get; set; }
        public decimal Quantity { get; set; }
        public string UnitType { get; set; } = "unit";
        public string UnitMeasure { get; set; } = "pieces";

        public decimal Total => Price * Quantity;
        public decimal MarkedTotal => MarkedPrice * Quantity;
        public decimal Savings => MarkedPrice > 0 ? MarkedTotal - Total : 0;
        public string SavingsText => MarkedPrice > 0 && MarkedPrice > Price ? $"Save {_billSettings.CurrencySymbol} {Savings:F2}" : "";
        
        // Formatted properties for display
        public string FormattedMarkedPrice => MarkedPrice > 0 ? $"{MarkedPrice:F2}" : "";
        public string FormattedTotal => $"{Total:F2}";
        public string FormattedPrice => $"{Price:F2}";
        public string FormattedQuantity => UnitType == "mass" ? $"{Quantity:F2} {UnitMeasure}" : $"{Quantity:F0} {UnitMeasure}";
    }
}

