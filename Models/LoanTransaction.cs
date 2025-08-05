using System;

namespace VillageSmartPOS.Models
{
    public enum TransactionType
    {
        PURCHASE,
        REPAYMENT
    }

    public class LoanTransaction
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public decimal OldBalance { get; set; }
        public decimal NewBalance { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string Notes { get; set; } = string.Empty;

        // Computed properties for display
        public string TransactionTypeText => TransactionType.ToString();
        public string AmountText => $"Rs. {Amount:F2}";
        public string OldBalanceText => $"Rs. {OldBalance:F2}";
        public string NewBalanceText => $"Rs. {NewBalance:F2}";
        public string DateText => TransactionDate.ToString("yyyy-MM-dd HH:mm");
        
        // For UI display
        public string TransactionSummary => $"{TransactionTypeText}: Rs. {Amount:F2} (Balance: Rs. {OldBalance:F2} → Rs. {NewBalance:F2})";
    }
} 