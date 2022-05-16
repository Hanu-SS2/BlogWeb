using BlogWeb.Data;
using BlogWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogWeb.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly BlogWebContext _context;

        public CategoriesController(BlogWebContext context)
        {
            _context = context;
        }

        public IActionResult Index(String? name)
        {
            List<Category> categories = _context.Category.ToList();

            if (!String.IsNullOrEmpty(name))
            {
                List<Category> filteredCate = _context.Category.Where(c => c.CatName.ToLower().Contains(name.ToLower())).ToList();
                ViewBag.Name = name;
                return View(filteredCate);
            }

            if (categories != null)
            {
                return View(categories);
            }
            return View();
        }

        [HttpGet]
        public IActionResult Detail(int? id)
        {
            List<Post> posts = _context.Post.Where(p => p.CatId == id).ToList();
            ViewBag.AdminAccounts = _context.AdminAccount.ToList();
            if (posts == null)
            {
                return NotFound();
            }
            return View(posts);
        }
    }
}
