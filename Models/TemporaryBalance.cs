using System;

namespace VillageSmartPOS.Models
{
    public class TemporaryBalance
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Notes { get; set; }

        // Computed properties for display
        public string FormattedBalance => $"Rs. {Balance:F2}";
        public string FormattedCreatedAt => CreatedAt.ToString("yyyy-MM-dd HH:mm");
        public string FormattedUpdatedAt => UpdatedAt.ToString("yyyy-MM-dd HH:mm");
        public string DisplayName => $"{CustomerName} - {FormattedBalance}";
    }
} 