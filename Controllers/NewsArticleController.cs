using AS1_HE180034_PRN222.Models;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Controllers
{
	public class newsArticleController : Controller
	{
		private readonly FunewsManagementContext _context;

		public newsArticleController(FunewsManagementContext context)
		{
		_context = context;
		}

		public IActionResult NewsArticleView()
		{
			var listNews = _context.NewsArticles.Where(a => a.NewsStatus == true).ToList();
			return View(listNews);
		}

        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = _context.NewsArticles
                .FirstOrDefault(n => n.NewsArticleId == id);

            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

    }
}
