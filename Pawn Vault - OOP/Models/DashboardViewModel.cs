using Pawn_Vault___OOP.Models;

namespace Pawn_Vault___OOP.Models.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TodaysRevenue { get; set; }
        public decimal InterestRevenue { get; set; }
        public decimal SalesRevenue { get; set; }
        public int ActiveLoansCount { get; set; }
        public int InventoryItemsCount { get; set; }
        public List<Payment> RecentTransactions { get; set; } = new List<Payment>();
        public string RevenueChangeText { get; set; } = "";
        public string LoansChangeText { get; set; } = "";
        public string InventoryChangeText { get; set; } = "";
        
        // Additional insights
        public int OverdueLoansCount { get; set; }
        public int DueTodayLoansCount { get; set; }
        public decimal TotalActiveLoansValue { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int TransactionsToday { get; set; }
        public decimal AvgLoanAmount { get; set; }
        public string MostPopularCategory { get; set; } = "";
        public decimal WeeklyRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int PendingRenewalsCount { get; set; }
    }
}
