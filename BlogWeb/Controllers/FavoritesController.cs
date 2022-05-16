#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Models;
using BlogWeb.Data;

namespace BlogWeb.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly BlogWebContext _context;
        private Extensions.Extension exten;

        public FavoritesController(BlogWebContext context)
        {
            _context = context;
            exten = new Extensions.Extension(_context);
        }

        // GET: Favorites
        public async Task<IActionResult> Index()
        {
            int? userID = exten.SessionUserID(HttpContext);

            if (userID != null)
            {
                List<Favorite> favorites = _context.Favorite.ToList();
                ViewBag.Favorites = favorites;

                var user = _context.UserAccount
                .FirstOrDefault(u => u.UserAccountId == userID);
                ViewBag.User = user;
            }

            else
            {
                return RedirectToAction("Login", "Home");
            }
            List<Post> listPost = _context.Post.ToList();
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

            var favorite = await _context.Favorite
                .Include(f => f.Post)
                .Include(f => f.UserAccount)
                .FirstOrDefaultAsync(m => m.FavoriteId == id);
            if (favorite == null)
            {
                return NotFound();
            }
            _context.Favorite.Remove(favorite);
            await _context.SaveChangesAsync();
            TempData["Deleted"] = "Removed from 'Favorite'!";
            return RedirectToAction("Index");

        }
    }
}
