using AS1_HE180034_PRN222.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagement.Controllers
{
    public class AdminController : Controller
    {
        private readonly FunewsManagementContext _context;

        public AdminController(FunewsManagementContext context)
        {
            _context = context;
        }

        public IActionResult DashBoard()
        {
            string role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Login", "Login");
            }

            switch (role)
            {
                case "Admin":
                    return View("AccountManagement", "Admin");
                case "Staff":
                    return RedirectToAction("StaffDashBoard", "Staff");
                default:
                    return RedirectToAction("NewsArticleView", "NewsArticle");
            }
        }

        // Account Management
        public IActionResult AccountManagement()
        {
            var accountList = _context.SystemAccounts.ToList();

            if (accountList == null || !accountList.Any())
            {
                ViewBag.Message = "No accounts found";
                return View(new List<SystemAccount>());
            }

            return View(accountList);
        }

        // Create Account
        public IActionResult AddAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAccount(SystemAccount systemAccount)
        {
            // Generate new ID
            short maxId = _context.SystemAccounts.Any()
                ? _context.SystemAccounts.Max(a => a.AccountId)
                : (short)0;

            systemAccount.AccountId = (short)(maxId + 1);

            // Check for existing accounts with same name or email
            var existingAccount = _context.SystemAccounts
                .FirstOrDefault(a => a.AccountName == systemAccount.AccountName ||
                                    a.AccountEmail == systemAccount.AccountEmail);

            if (existingAccount != null)
            {
                if (existingAccount.AccountName == systemAccount.AccountName)
                {
                    ModelState.AddModelError("AccountName", "Account name already exists");
                }

                if (existingAccount.AccountEmail == systemAccount.AccountEmail)
                {
                    ModelState.AddModelError("AccountEmail", "Email address already exists");
                }

                return View(systemAccount);
            }

            if (ModelState.IsValid)
            {
                _context.SystemAccounts.Add(systemAccount);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Account added successfully";
                return RedirectToAction("AccountManagement");
            }

            return View(systemAccount);
        }

        // Edit Account
        public IActionResult EditAccount(short id)
        {
            var account = _context.SystemAccounts.FirstOrDefault(a => a.AccountId == id);

            if (account == null)
            {
                return NotFound("Account not found");
            }

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAccount(SystemAccount systemAccount)
        {
            var existingAccount = _context.SystemAccounts
                .FirstOrDefault(a => a.AccountId == systemAccount.AccountId);

            if (existingAccount == null)
            {
                return NotFound("Account not found");
            }

            // Update only the role (as per original code)
            existingAccount.AccountRole = systemAccount.AccountRole;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Account updated successfully";
            return RedirectToAction("AccountManagement");
        }

        // Delete Account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAccount(short id)
        {
            var account = _context.SystemAccounts.FirstOrDefault(a => a.AccountId == id);

            if (account == null)
            {
                return NotFound("Account not found");
            }

            _context.SystemAccounts.Remove(account);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Account deleted successfully";
            return RedirectToAction("AccountManagement");
        }

        // Report Generation
        public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                ViewBag.Message = "Please select both start and end dates";
                return View(new List<NewsArticle>());
            }

            var reportList = await _context.NewsArticles
                .Where(n => n.CreatedDate >= startDate && n.ModifiedDate <= endDate)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            if (!reportList.Any())
            {
                ViewBag.Message = "No articles found for the selected date range";
            }

            return View(reportList);
        }
    }
}