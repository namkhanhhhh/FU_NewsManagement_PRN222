using AS1_HE180034_PRN222.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FUNewsManagement.Controllers
{
    public class StaffController : Controller
    {
        private readonly FunewsManagementContext _context;

        public StaffController(FunewsManagementContext context)
        {
            _context = context;
        }

        // Dashboard
        public IActionResult StaffDashBoard()
        {
            return View();
        }

        // Category Management
        public IActionResult CategoryManagement()
        {
            var categoryList = _context.Categories.ToList();
            return View(categoryList);
        }

        public IActionResult AddCategory()
        {
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.CategoryName })
                .ToList();

            return View(new Category());
        }

        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Add category successful";
                return RedirectToAction("CategoryManagement");
            }

            return View(category);
        }

        public IActionResult EditCategory(short id)
        {
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.CategoryName })
                .ToList();

            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);

            return View(category);
        }

        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Edit category successful!";
                return RedirectToAction("CategoryManagement");
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult DeleteCategory(short id)
        {
            var category = _context.Categories
                .Include(c => c.NewsArticles)
                .FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.NewsArticles.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete. The category have news used";
                return RedirectToAction("CategoryManagement");
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Delete successful!";
            return RedirectToAction("CategoryManagement");
        }

        // News Management
        public IActionResult NewsMangement(string search, short? categoryId)
        {
            var newsArticleList = _context.NewsArticles
                .Include(n => n.Tags)
                .Include(n => n.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                newsArticleList = newsArticleList.Where(n =>
                    n.NewsTitle.Contains(search) ||
                    n.Headline.Contains(search));
            }

            if (categoryId.HasValue)
            {
                newsArticleList = newsArticleList.Where(n => n.CategoryId == categoryId);
            }

            ViewBag.Categories = _context.Categories.ToList();

            return View(newsArticleList.ToList());
        }

        public async Task<IActionResult> EditNewsArticleList(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle = await _context.NewsArticles
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (newsArticle == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();

            return View(newsArticle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNewsArticleList(string id, NewsArticle model, List<int> selectedTags)
        {
            if (id != model.NewsArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingArticle = await _context.NewsArticles
                        .Include(n => n.Tags)
                        .FirstOrDefaultAsync(n => n.NewsArticleId == id);

                    if (existingArticle != null)
                    {
                        // Update article properties
                        existingArticle.NewsTitle = model.NewsTitle;
                        existingArticle.Headline = model.Headline;
                        existingArticle.NewsContent = model.NewsContent;
                        existingArticle.NewsSource = model.NewsSource;
                        existingArticle.CategoryId = model.CategoryId;
                        existingArticle.NewsStatus = model.NewsStatus;
                        existingArticle.ModifiedDate = DateTime.Now;

                        // Update tags
                        var selectedTagsList = _context.Tags
                            .Where(t => selectedTags.Contains(t.TagId))
                            .ToList();

                        existingArticle.Tags.Clear();

                        foreach (var tag in selectedTagsList)
                        {
                            existingArticle.Tags.Add(tag);
                        }

                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.NewsArticles.Any(e => e.NewsArticleId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["SuccessMessage"] = "Update new article successful!";
                return RedirectToAction("NewsMangement");
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(model);
        }

        public IActionResult AddNewsArticleList()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewsArticleList(NewsArticle newsArticle, List<int> selectedTags)
        {
            short id = short.Parse(HttpContext.Session.GetString("UserID"));

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var subError in error.Value.Errors)
                    {
                        Console.WriteLine($"Lỗi tại {error.Key}: {subError.ErrorMessage}");
                    }
                }

                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                return View(newsArticle);
            }

            if (ModelState.IsValid)
            {
                // Set creation and modification info
                newsArticle.CreatedDate = DateTime.Now;
                newsArticle.ModifiedDate = DateTime.Now;
                newsArticle.CreatedById = id;
                newsArticle.UpdatedById = id;

                // Add tags if selected
                if (selectedTags != null && selectedTags.Any())
                {
                    newsArticle.Tags = _context.Tags
                        .Where(t => selectedTags.Contains(t.TagId))
                        .ToList();
                }

                _context.NewsArticles.Add(newsArticle);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Add new article successful!";
                return RedirectToAction("NewsMangement");
            }

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            return View(newsArticle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNewsArticle(string id)
        {
            var newsArticle = await _context.NewsArticles
                .Include(na => na.Tags)
                .FirstOrDefaultAsync(na => na.NewsArticleId == id);

            if (newsArticle == null)
            {
                TempData["ErrorMessage"] = "New not existed";
                return RedirectToAction("NewsMangement");
            }

            // Remove associations before deleting the article
            foreach (var tag in newsArticle.Tags.ToList())
            {
                tag.NewsArticles.Remove(newsArticle);
            }

            _context.NewsArticles.Remove(newsArticle);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Delete successful";
            return RedirectToAction("NewsMangement");
        }

        // User Profile Management
        public IActionResult Profile()
        {
            short id = short.Parse(HttpContext.Session.GetString("UserID"));
            var account = _context.SystemAccounts.FirstOrDefault(a => a.AccountId == id);
            return View(account);
        }

        public IActionResult EditProfile(short id)
        {
            var account = _context.SystemAccounts.FirstOrDefault(a => a.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(SystemAccount model)
        {
            var account = _context.SystemAccounts.FirstOrDefault(a => a.AccountId == model.AccountId);

            if (account == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                account.AccountName = model.AccountName;
                account.AccountEmail = model.AccountEmail;
                account.AccountRole = model.AccountRole;

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Update profile successful!";
                return RedirectToAction("Profile");
            }

            return View(account);
        }

        public IActionResult ChangePassword(short id)
        {
            var account = _context.SystemAccounts.FirstOrDefault(a => a.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            var model = new ChangePasswordViewModel
            {
                AccountId = account.AccountId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = _context.SystemAccounts.FirstOrDefault(a => a.AccountId == model.AccountId);

                if (account == null)
                {
                    return NotFound();
                }

                // Validate old password
                if (account.AccountPassword != model.OldPassword)
                {
                    ModelState.AddModelError("OldPassword", "Current password is incorrect");
                    return View(model);
                }

                // Validate password confirmation
                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "New password and confirmation do not match");
                    return View(model);
                }

                // Update password
                account.AccountPassword = model.NewPassword;
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Profile");
            }

            return View(model);
        }

        // User History
        public IActionResult History()
        {
            short id = short.Parse(HttpContext.Session.GetString("UserID") ?? "0");

            var history = _context.NewsArticles
                .Include(a => a.CreatedBy)
                .Where(a => a.CreatedById == id)
                .ToList();

            if (!history.Any())
            {
                return NotFound();
            }

            return View(history);
        }
    }
}