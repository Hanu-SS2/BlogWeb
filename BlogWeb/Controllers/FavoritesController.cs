#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Models;

namespace BlogWeb.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly BlogDBContext _context;

        public FavoritesController(BlogDBContext context)
        {
            _context = context;
        }

        // GET: Favorites
        public async Task<IActionResult> Index()
        {
            var Session = HttpContext.Session;
            int? userID = Session.GetInt32("UserAccountId");

            if (userID != null)
            {
                List<Favorite> favorites = _context.Favorites.ToList();
                ViewBag.Favorites = favorites;

                var user = _context.UserAccounts
                .FirstOrDefault(u => u.UserAccountId == userID);
                ViewBag.User = user;
            }

            else
            {
                return RedirectToAction("Login", "Home");
            }
            List<Post> listPost = _context.Posts.ToList();
            ViewBag.Posts = listPost;

            return View();
        }

        // GET: Favorites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favorite = await _context.Favorites
                .Include(f => f.Post)
                .Include(f => f.UserAccount)
                .FirstOrDefaultAsync(m => m.FavoriteId == id);
            if (favorite == null)
            {
                return NotFound();
            }
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            TempData["Deleted"] = "Removed from 'Favorite'!";
            return RedirectToAction("Index");

        }
    }
}
