using AS1_HE180034_PRN222.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace FUNewsManagement.Controllers
{
    public class LoginController : Controller
    {
        private readonly FunewsManagementContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(FunewsManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(SystemAccount systemAccount)
        {
            // Admin authentication
            string adminEmail = _configuration["AdminAccount:Email"];
            string adminPass = _configuration["AdminAccount:Password"];

            // Check if admin credentials match
            if (adminEmail == systemAccount.AccountEmail &&
                adminPass == systemAccount.AccountPassword)
            {
                HttpContext.Session.SetString("UserRole", "Admin");
                return RedirectToAction("AccountManagement", "Admin");
            }

            // User authentication
            var accountCheck = _context.SystemAccounts
                .FirstOrDefault(s => s.AccountEmail == systemAccount.AccountEmail);

            // Handle authentication failures
            if (accountCheck == null)
            {
                ViewBag.ErrorMessage = "Email not found";
                return View(systemAccount);
            }

            if (accountCheck.AccountPassword != systemAccount.AccountPassword)
            {
                ViewBag.ErrorMessage = "Incorrect password";
                return View(systemAccount);
            }

            // Handle role-based authentication
            switch (accountCheck.AccountRole)
            {
                case 1:
                    HttpContext.Session.SetString("UserID", accountCheck.AccountId.ToString());
                    HttpContext.Session.SetString("UserRole", "Staff");
                    return RedirectToAction("StaffDashBoard", "Staff", accountCheck);

                case 2:
                    HttpContext.Session.SetString("UserID", "2");
                    HttpContext.Session.SetString("UserRole", "Lecture");
                    return RedirectToAction("NewsArticleView", "NewsArticle");

                default:
                    ViewBag.ErrorMessage = "Account does not have permission";
                    return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}