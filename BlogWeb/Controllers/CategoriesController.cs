using BlogWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogWeb.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly BlogDBContext _context;

        public CategoriesController(BlogDBContext context)
        {
            _context = context;
        }

        public IActionResult Index(String? name)
        {
            List<Category> categories = _context.Categories.ToList();

            if (!String.IsNullOrEmpty(name))
            {
                List<Category> filteredCate = _context.Categories.Where(c => c.CatName.ToLower().Contains(name.ToLower())).ToList();
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
            List<Post> posts = _context.Posts.Where(p => p.CatId == id).ToList();
            if(posts == null)
            {
                return NotFound();
            }
            return View(posts);
        }
    }
}
