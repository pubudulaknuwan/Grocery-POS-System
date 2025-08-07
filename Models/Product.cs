using System;

namespace VillageSmartPOS.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal MarkedPrice { get; set; }
        public int Quantity { get; set; }
        public string UnitType { get; set; } = "unit"; // "mass" or "unit"
        public string UnitMeasure { get; set; } = "pieces"; // "kg", "g", "pieces", "bottles", etc.
        public string Category { get; set; } = string.Empty;
        public int ReorderLevel { get; set; } = 10;
        public string Supplier { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        
        // Computed properties for inventory management
        public bool IsLowStock => Quantity <= ReorderLevel;
        public bool IsOutOfStock => Quantity <= 0;
        public string StockStatus => IsOutOfStock ? "Out of Stock" : IsLowStock ? "Low Stock" : "In Stock";
        
        // Computed properties for dual pricing strategy
        public decimal Savings => MarkedPrice > 0 && MarkedPrice > Price ? MarkedPrice - Price : 0;
        public decimal SavingsPercentage => MarkedPrice > 0 ? (Savings / MarkedPrice) * 100 : 0;
        public string SavingsText => MarkedPrice > 0 && MarkedPrice > Price ? $"Save ${Savings:F2}" : "";
        public string DualPriceText => MarkedPrice > 0 && MarkedPrice > Price ? $"${MarkedPrice:F2} → ${Price:F2}" : $"${Price:F2}";
        
        // Computed properties for unit display
        public string QuantityDisplay => $"{Quantity} {UnitMeasure}";
        public string PricePerUnit => UnitType == "mass" ? $"{Price:F2} per {UnitMeasure}" : $"{Price:F2} per {UnitMeasure}";
        public string MarkedPricePerUnit => UnitType == "mass" ? $"{MarkedPrice:F2} per {UnitMeasure}" : $"{MarkedPrice:F2} per {UnitMeasure}";
        
        // Helper properties for UI
        public bool IsMassBased => UnitType == "mass";
        public bool IsUnitBased => UnitType == "unit";
        public string UnitTypeDisplay => UnitType == "mass" ? "Mass (kg/g)" : "Units (pieces/bottles)";
        
        // Formatted properties for display
        public string FormattedMarkedPrice => MarkedPrice > 0 ? $"{MarkedPrice:F2}" : "";
    }
}


