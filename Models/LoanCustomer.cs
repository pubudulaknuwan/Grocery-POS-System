using System;

namespace VillageSmartPOS.Models
{
    public class LoanCustomer
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Computed properties for display
        public string DisplayName => $"{CustomerId} - {Name}";
        public string BalanceText => $"Rs. {CurrentBalance:F2}";
        public string PhoneDisplay => string.IsNullOrEmpty(Phone) ? "No Phone" : Phone;
        public string AddressDisplay => string.IsNullOrEmpty(Address) ? "No Address" : Address;
    }
} 