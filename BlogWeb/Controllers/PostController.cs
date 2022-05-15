using BlogWeb.Models;
/*using BlogWeb.Repositories;*/
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
/*using System.Data.Entity;*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlogWeb.Controllers
{
    public class PostController : Controller
    {
        private readonly BlogDBContext _dbContext;

        public PostController(BlogDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index(String? keyword)
        {
            List<AdminAccount> admins = _dbContext.AdminAccounts.ToList();
            ViewBag.Admins = admins;

            List<Post> listPost = _dbContext.Posts.ToList();
            ViewBag.Posts = listPost;



            if (!String.IsNullOrEmpty(keyword))
            {
                listPost = _dbContext.Posts.Where(p => p.Title.ToLower().Contains(keyword.ToLower())).ToList();
                ViewBag.Posts = listPost;
                ViewBag.Keyword = keyword;
            }
            return View();
        }
        [HttpPost]
        public IActionResult AddToFavorite(Favorite favorite, int postId)
        {
            var Session = HttpContext.Session;

            int? UserID = Session.GetInt32("UserAccountId");
            Post? post = _dbContext.Posts.Where(p => p.PostId == postId).FirstOrDefault();

            var user = _dbContext.UserAccounts.Where(u => u.UserAccountId == UserID).FirstOrDefault();

            if (UserID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<Favorite> favorites = _dbContext.Favorites.ToList();
            var favoriteItem = favorites.Find(p => p.PostId == post.PostId && p.UserAccountId == user.UserAccountId);

            if (favoriteItem == null)
            {
                favorite.PostId = post.PostId;
                favorite.UserAccountId = user.UserAccountId;
                _dbContext.Favorites.Add(favorite);
                _dbContext.SaveChanges();
                TempData["Success"] = "Add to Favorite successfully!";
                Session.SetInt32("FavoriteId", favorite.FavoriteId);
            }
            else
            {
                TempData["Fail"] = "This post already exists in Favorite!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var post = _dbContext.Posts.Where(p => p.PostId == id).FirstOrDefault();
            if (post == null)
            {
                return NotFound();
            }
            else
            {
                ViewBag.Post = post;
                ViewBag.Admins = _dbContext.AdminAccounts.ToList();
                ViewBag.UserAccounts = _dbContext.UserAccounts.ToList();
                ViewBag.Comments = _dbContext.Comments.ToList();
                return View();
            }
        }

        public async Task<IActionResult> DeleteComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var comment = _dbContext.Comments.Where(c => c.CommentId == id).FirstOrDefault();
            if (comment == null)
            {
                return NotFound();
            }
            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();
            TempData["Deleted"] = "Deleted this comment successfully!";
            return RedirectToAction("Details", new RouteValueDictionary(
                       new
                       {
                           controller = "Post",
                           action = "Details",
                           id = comment.PostId
                       }
                       ));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Comment(int postId, Comment comment)
        {
            var Session = HttpContext.Session;
            int? UserID = Session.GetInt32("UserAccountId");
            Post? post = _dbContext.Posts.Where(p => p.PostId == postId).FirstOrDefault();
            UserAccount? user = _dbContext.UserAccounts.Where(u => u.UserAccountId == UserID).FirstOrDefault();
            ViewBag.User = user;

            if (ModelState.IsValid)
            {
                comment.PostId = post.PostId;
                comment.UserAccountId = user.UserAccountId;
                comment.CreatedDate = DateTime.Now;
                _dbContext.Add(comment);
                _dbContext.SaveChangesAsync();
                TempData["Success"] = "Send a comment successfully!";
                return RedirectToAction("Details", new RouteValueDictionary(
                        new {
                            controller = "Post", action = "Details", id = postId }
                        ));
            }
            return View(comment);
        }
       

    }
}
