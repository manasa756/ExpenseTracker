using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    public class DashBoard : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashBoard(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            var userEmail = User.Identity.Name;
            var transactions = _context.Transactions.Include(t => t.Category).Where(t=>t.UserEmail==userEmail).ToList();
            var TotalIncome = transactions.Where(t =>t.Category!=null&& t.Category.Type == "Income").Sum(t => t.Amount);
            var TotalExpenses = transactions.Where(t => t.Category != null && t.Category.Type == "Expense").Sum(t => t.Amount);
            var balance = TotalIncome - TotalExpenses;
            ViewBag.Income = TotalIncome;
            ViewBag.Expense = TotalExpenses;
            ViewBag.balance = balance;

            var categoryData = transactions
      .Where(t => t.Category != null && t.Category.Type == "Expense")
      .GroupBy(t => t.Category.Title)
      .Select(g => new {
          Category = g.Key,
          Amount = g.Sum(x => x.Amount)
      }).ToList();

            ViewBag.CategoryData = categoryData;

            var recent = transactions
       .OrderByDescending(t => t.Date)
       .Take(5)
       .ToList();

            ViewBag.RecentTransactions = recent;

            return View();

        }
    }
}
