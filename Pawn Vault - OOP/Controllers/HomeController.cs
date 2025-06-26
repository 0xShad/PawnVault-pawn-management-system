using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawn_Vault___OOP.Models;
using Pawn_Vault___OOP.Models.ViewModels;
using Pawn_Vault___OOP.Interfaces;

namespace Pawn_Vault___OOP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoanRepository _loanRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ICustomerRepository _customerRepository;

        public HomeController(
            ILogger<HomeController> logger,
            ILoanRepository loanRepository,
            IPaymentRepository paymentRepository,
            IInventoryRepository inventoryRepository,
            ICustomerRepository customerRepository)
        {
            _logger = logger;
            _loanRepository = loanRepository;
            _paymentRepository = paymentRepository;
            _inventoryRepository = inventoryRepository;
            _customerRepository = customerRepository;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel();
            
            // Get today's payments for revenue calculation
            var allPayments = await _paymentRepository.GetAllPaymentsAsync();
            var todaysPayments = allPayments.Where(p => p.PaymentDate.Date == DateTime.Today);
            
            // Revenue from interest earned on redemption payments
            decimal interestRevenue = 0;
            foreach (var payment in todaysPayments.Where(p => p.TransactionType.ToLower() == "redemption"))
            {
                // Load the loan to get the principal amount
                var loan = await _loanRepository.GetLoanbyIdAsync(payment.LoanID);
                if (loan != null)
                {
                    // Interest earned = Payment Amount - Loan Principal
                    var interestEarned = payment.Amount - loan.Amount;
                    if (interestEarned > 0)
                    {
                        interestRevenue += interestEarned;
                    }
                }
            }
            
            // Revenue from sold inventory items (full estimated value)
            var allInventory = await _inventoryRepository.GetAllItemsAsync();
            var todaysSoldItems = allInventory.Where(i => 
                i.DateAdded.Date == DateTime.Today && 
                i.Status.ToLower() == "sold" && 
                !i.IsDeleted);
            decimal salesRevenue = todaysSoldItems.Sum(i => i.EstimatedValue);
            
            // Total revenue = Interest Revenue + Sales Revenue
            model.TodaysRevenue = interestRevenue + salesRevenue;
            model.InterestRevenue = interestRevenue;
            model.SalesRevenue = salesRevenue;
            
            // Get active loans count
            var allLoans = await _loanRepository.GetAllLoansAsync();
            model.ActiveLoansCount = allLoans.Count(l => l.Status == "Active");
            
            // Get inventory items count (available items, not sold)
            model.InventoryItemsCount = allInventory.Count(i => 
                i.Status.ToLower() != "sold" && 
                !i.IsDeleted);
            
            // Get recent transactions (last 5)
            model.RecentTransactions = allPayments
                .OrderByDescending(p => p.PaymentDate)
                .Take(5)
                .ToList();
            
            // Calculate change percentages for yesterday
            var yesterdaysPayments = allPayments.Where(p => p.PaymentDate.Date == DateTime.Today.AddDays(-1));
            decimal yesterdaysInterestRevenue = 0;
            foreach (var payment in yesterdaysPayments.Where(p => p.TransactionType.ToLower() == "redemption"))
            {
                var loan = await _loanRepository.GetLoanbyIdAsync(payment.LoanID);
                if (loan != null)
                {
                    var interestEarned = payment.Amount - loan.Amount;
                    if (interestEarned > 0)
                    {
                        yesterdaysInterestRevenue += interestEarned;
                    }
                }
            }
            
            // Yesterday's sales revenue
            var yesterdaysSoldItems = allInventory.Where(i => 
                i.DateAdded.Date == DateTime.Today.AddDays(-1) && 
                i.Status.ToLower() == "sold" && 
                !i.IsDeleted);
            decimal yesterdaysSalesRevenue = yesterdaysSoldItems.Sum(i => i.EstimatedValue);
            
            var yesterdaysRevenue = yesterdaysInterestRevenue + yesterdaysSalesRevenue;
            
            if (yesterdaysRevenue > 0)
            {
                var changePercent = ((model.TodaysRevenue - yesterdaysRevenue) / yesterdaysRevenue) * 100;
                model.RevenueChangeText = changePercent > 0 ? $"+{changePercent:F1}% from yesterday" : $"{changePercent:F1}% from yesterday";
            }
            else
            {
                if (interestRevenue > 0 && salesRevenue > 0)
                    model.RevenueChangeText = "Interest + Sales earned today!";
                else if (interestRevenue > 0)
                    model.RevenueChangeText = "Interest earned today!";
                else if (salesRevenue > 0)
                    model.RevenueChangeText = "Sales revenue earned today!";
                else
                    model.RevenueChangeText = "No revenue earned yet";
            }
            
            model.LoansChangeText = "Active loans";
            model.InventoryChangeText = "Items in stock";
            
            // Additional insights calculations
            var today = DateTime.Today;
            
            // Overdue and due today loans
            model.OverdueLoansCount = allLoans.Count(l => 
                l.Status.ToLower() == "active" && 
                l.DueDate.Date < today && 
                !l.IsDeleted);
                
            model.DueTodayLoansCount = allLoans.Count(l => 
                l.Status.ToLower() == "active" && 
                l.DueDate.Date == today && 
                !l.IsDeleted);
            
            // Total values
            model.TotalActiveLoansValue = allLoans
                .Where(l => l.Status.ToLower() == "active" && !l.IsDeleted)
                .Sum(l => l.Amount);
                
            model.TotalInventoryValue = allInventory
                .Where(i => i.Status.ToLower() != "sold" && !i.IsDeleted)
                .Sum(i => i.EstimatedValue);
            
            // New customers this month
            var allCustomers = await _customerRepository.GetAllCustomersAsync();
            model.NewCustomersThisMonth = allCustomers.Count(c => 
                c.DateAdded.Month == today.Month && 
                c.DateAdded.Year == today.Year && 
                !c.IsDeleted);
            
            // Transactions today
            model.TransactionsToday = todaysPayments.Count();
            
            // Average loan amount
            var activeLoans = allLoans.Where(l => l.Status.ToLower() == "active" && !l.IsDeleted);
            model.AvgLoanAmount = activeLoans.Any() ? activeLoans.Average(l => l.Amount) : 0;
            
            // Most popular category
            var categoryGroups = allInventory
                .Where(i => !i.IsDeleted)
                .GroupBy(i => i.Category)
                .OrderByDescending(g => g.Count());
            model.MostPopularCategory = categoryGroups.FirstOrDefault()?.Key ?? "N/A";
            
            // Weekly and monthly revenue
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);
            
            // Weekly revenue calculation
            var weeklyPayments = allPayments.Where(p => 
                p.PaymentDate.Date >= weekStart && 
                p.PaymentDate.Date <= today);
            
            decimal weeklyInterestRevenue = 0;
            foreach (var payment in weeklyPayments.Where(p => p.TransactionType.ToLower() == "redemption"))
            {
                var loan = await _loanRepository.GetLoanbyIdAsync(payment.LoanID);
                if (loan != null)
                {
                    var interestEarned = payment.Amount - loan.Amount;
                    if (interestEarned > 0)
                    {
                        weeklyInterestRevenue += interestEarned;
                    }
                }
            }
            
            var weeklySoldItems = allInventory.Where(i => 
                i.DateAdded.Date >= weekStart && 
                i.DateAdded.Date <= today && 
                i.Status.ToLower() == "sold" && 
                !i.IsDeleted);
            var weeklySalesRevenue = weeklySoldItems.Sum(i => i.EstimatedValue);
            
            model.WeeklyRevenue = weeklyInterestRevenue + weeklySalesRevenue;
            
            // Monthly revenue calculation
            var monthlyPayments = allPayments.Where(p => 
                p.PaymentDate.Date >= monthStart && 
                p.PaymentDate.Date <= today);
            
            decimal monthlyInterestRevenue = 0;
            foreach (var payment in monthlyPayments.Where(p => p.TransactionType.ToLower() == "redemption"))
            {
                var loan = await _loanRepository.GetLoanbyIdAsync(payment.LoanID);
                if (loan != null)
                {
                    var interestEarned = payment.Amount - loan.Amount;
                    if (interestEarned > 0)
                    {
                        monthlyInterestRevenue += interestEarned;
                    }
                }
            }
            
            var monthlySoldItems = allInventory.Where(i => 
                i.DateAdded.Date >= monthStart && 
                i.DateAdded.Date <= today && 
                i.Status.ToLower() == "sold" && 
                !i.IsDeleted);
            var monthlySalesRevenue = monthlySoldItems.Sum(i => i.EstimatedValue);
            
            model.MonthlyRevenue = monthlyInterestRevenue + monthlySalesRevenue;
            
            // Pending renewals (loans due within next 7 days)
            var nextWeek = today.AddDays(7);
            model.PendingRenewalsCount = allLoans.Count(l => 
                l.Status.ToLower() == "active" && 
                l.DueDate.Date > today && 
                l.DueDate.Date <= nextWeek && 
                !l.IsDeleted);
            
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
