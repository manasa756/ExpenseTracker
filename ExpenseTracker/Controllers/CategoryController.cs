using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userEmail = User.Identity.Name;
            var categories = _context.Categories.Where(c => c.UserEmail == userEmail).ToList();
            return View(categories);
        }
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View("CategoryEdit", category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View("CategoryEdit", category);
            }

            var existing = _context.Categories.Find(category.CategoryId);

            if (existing == null)
                return NotFound();

            // ✅ update only needed fields
            existing.Title = category.Title;
            existing.Icon = category.Icon;
            existing.Type = category.Type;

            // ✅ VERY IMPORTANT: preserve UserEmail
            existing.UserEmail = User.Identity.Name;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult Deleted(int id)
        {
            var category=_context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Category category)
        {
            category.UserEmail = User.Identity.Name;
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
