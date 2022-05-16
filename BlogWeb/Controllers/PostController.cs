using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Data;
using BlogWeb.Models;

namespace BlogWeb.Controllers
{
    public class PostController : Controller
    {
        private readonly BlogWebContext _dbContext;
        private Extensions.Extension exten;

        public PostController(BlogWebContext context)
        {
            _dbContext = context;
            exten = new Extensions.Extension(_dbContext);
        }
        public IActionResult Index(String? keyword)
        {
            List<AdminAccount> admins = _dbContext.AdminAccount.ToList();
            ViewBag.Admins = admins;

            List<Post> listPost = _dbContext.Post.ToList();
            ViewBag.Posts = listPost;

            if (!String.IsNullOrEmpty(keyword))
            {
                listPost = _dbContext.Post.Where(p => p.Title.ToLower().Contains(keyword.ToLower())).ToList();
                ViewBag.Posts = listPost;
                ViewBag.Keyword = keyword;
            }
            return View();
        }
        [HttpPost]
        public IActionResult AddToFavorite(Favorite favorite, int postId)
        {
            int? UserID = exten.SessionUserID(HttpContext);
            Post? post = _dbContext.Post.Where(p => p.PostId == postId).FirstOrDefault();

            var user = _dbContext.UserAccount.Where(u => u.UserAccountId == UserID).FirstOrDefault();

            if (UserID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<Favorite> favorites = _dbContext.Favorite.ToList();
            var favoriteItem = favorites.Find(p => p.PostId == post.PostId && p.UserAccountId == user.UserAccountId);

            if (favoriteItem == null)
            {
                favorite.PostId = post.PostId;
                favorite.UserAccountId = user.UserAccountId;
                _dbContext.Favorite.Add(favorite);
                _dbContext.SaveChanges();
                TempData["Success"] = "Add to Favorite successfully!";
                HttpContext.Session.SetInt32("FavoriteId", favorite.FavoriteId);
            }
            else
            {
                TempData["Fail"] = "This post already exists in Favorite!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var post = _dbContext.Post.Where(p => p.PostId == id).FirstOrDefault();
            if (post == null)
            {
                return NotFound();
            }
            else
            {
                ViewBag.Post = post;
                ViewBag.Admins = _dbContext.AdminAccount.ToList();
                ViewBag.UserAccounts = _dbContext.UserAccount.ToList();
                ViewBag.Comments = _dbContext.Comment.ToList();
                return View();
            }
        }

        public async Task<IActionResult> DeleteComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var comment = _dbContext.Comment.Where(c => c.CommentId == id).FirstOrDefault();
            if (comment == null)
            {
                return NotFound();
            }
            _dbContext.Comment.Remove(comment);
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
            int? UserID = exten.SessionUserID(HttpContext);
            Post? post = _dbContext.Post.Where(p => p.PostId == postId).FirstOrDefault();
            UserAccount? user = _dbContext.UserAccount.Where(u => u.UserAccountId == UserID).FirstOrDefault();
            ViewBag.User = user;

            if (ModelState.IsValid)
            {
                comment.PostId = post.PostId;
                comment.UserAccountId = user.UserAccountId;
                comment.CreatedDate = DateTime.Now;
                comment.hidden = false;
                _dbContext.Comment.Add(comment);
                _dbContext.SaveChanges();
                TempData["Success"] = "Send a comment successfully!";
                return RedirectToAction("Details", new RouteValueDictionary(
                        new
                        {
                            controller = "Post",
                            action = "Details",
                            id = postId
                        }
                        ));
            }
            return View(comment);
        }


    }
}
