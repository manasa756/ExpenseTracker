using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
          _context= context;
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterPage register)
        {
            if (ModelState.IsValid)
            {
                var User = new User
                {
                    Email = register.Email,
                    Password = register.Password
                };
                _context.Users.Add(User);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(register);
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
       
        
[HttpPost]
    public async Task<IActionResult> Login(LoginPage login)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == login.Email && u.Password == login.Password);

            if (user != null)
            {
                // ✅ Create claims (user identity)
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // ✅ SIGN IN (this was missing 🔥)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Invalid email or password";
        }

        return View(login);
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

}
}
