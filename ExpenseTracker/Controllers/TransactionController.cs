using ExpenseTracker.Migrations;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userEmail = User.Identity.Name;

            var transaction = _context.Transactions.Include(t => t.Category).Where(t => t.UserEmail == userEmail).ToList();
            return View(transaction);
        }
        public IActionResult Create()
        {
            var userEmail = User.Identity.Name;

            ViewBag.CategoryList = new SelectList(
                _context.Categories.Where(c => c.UserEmail == userEmail),
                "CategoryId",
                "Title"
            );

            return View();
        }
        [HttpPost]
        public IActionResult Create(Transaction transaction)
        {
            var userEmail = User.Identity.Name;

            // ✅ assign BEFORE validation
            transaction.UserEmail = userEmail;
            transaction.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Transactions.Add(transaction);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // ✅ reload dropdown when validation fails
            ViewBag.CategoryList = new SelectList(
                _context.Categories.Where(c => c.UserEmail == userEmail),
                "CategoryId",
                "Title"
            );

            return View(transaction);
        }




        public IActionResult Edit(int id)
        {
            var userEmail = User.Identity.Name;

            var transaction = _context.Transactions
                .FirstOrDefault(t => t.TransactionId == id && t.UserEmail == userEmail);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewBag.CategoryList = new SelectList(
    _context.Categories
.Where(c => c.UserEmail == userEmail && c.Title != null)
        .ToList(),
    "CategoryId",
    "Title",
    transaction?.CategoryId ?? 0 // fallback if null


    );
            return View(transaction);
        }

        [HttpPost]
        public IActionResult Edit(Transaction transaction)
        {

            
                var userEmail = User.Identity.Name;

                var existingTransaction = _context.Transactions
                    .FirstOrDefault(t => t.TransactionId == transaction.TransactionId
                                      && t.UserEmail == userEmail);

                if (existingTransaction == null)
                {
                    return NotFound();
                }

                existingTransaction.Amount = transaction.Amount;
                existingTransaction.CategoryId = transaction.CategoryId;
                existingTransaction.Note = transaction.Note;
                existingTransaction.Date = transaction.Date;

                _context.SaveChanges();

                return RedirectToAction("Index");
            }


        
        public IActionResult Delete(int id)
        {
            var userEmail = User.Identity.Name;

            var transaction = _context.Transactions
                .FirstOrDefault(t => t.TransactionId == id && t.UserEmail == userEmail);
            if (transaction == null)
            {
                return NotFound();
            }

            ViewBag.CategoryList = new SelectList(
                _context.Categories
                    .Where(c => c.UserEmail == userEmail)
                    .ToList(),
                "CategoryId",
                "Title",
                transaction?.CategoryId ?? 0
            );

            return View(transaction);
        }
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult Deleted(int id)
        {
            var userEmail = User.Identity.Name;

            var transaction = _context.Transactions
    .FirstOrDefault(t => t.TransactionId == id && t.UserEmail == userEmail);

            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
