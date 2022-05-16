using BlogWeb.Data;
using BlogWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogWeb.Controllers
{
    public class AuthorController : Controller
    {
        private readonly BlogWebContext _context;

        public AuthorController(BlogWebContext context)
        {
            _context = context;
        }
        public IActionResult Index(String? name)
        {
            List<AdminAccount> accounts = _context.AdminAccount.ToList();
            if (!String.IsNullOrEmpty(name))
            {
                List<AdminAccount> filteredAuthor = _context.AdminAccount.Where(a => a.FullName.ToLower().Contains(name.ToLower())).ToList();
                ViewBag.Name = name;
                return View(filteredAuthor);
            }

            if (accounts != null)
            {
                return View(accounts);
            }

            return View();
        }

        [HttpGet]
        public IActionResult Detail(int? id)
        {
            List<Post> posts = _context.Post.Where(p => p.AdminAccountId == id).ToList();
            ViewBag.AdminAccount = _context.AdminAccount.ToList();
            if (posts == null)
            {
                return NotFound();
            }
            return View(posts);
        }
    }
}
