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
    public class CommentsController : Controller
    {
        private readonly BlogWebContext _context;
        private Extensions.Extension exten;

        public CommentsController(BlogWebContext context)
        {
            _context = context;
            exten = new Extensions.Extension(_context);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            int? UserID = exten.SessionUserID(HttpContext);
            if (UserID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Post, "PostId", "PostId", comment.PostId);
            ViewData["UserAccountId"] = new SelectList(_context.UserAccount, "UserAccountId", "Email", comment.UserAccountId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,Contents,UserAccountId,PostId,CreatedDate, hidden")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    comment.hidden = false;
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new RouteValueDictionary(
                       new
                       {
                           controller = "Post",
                           action = "Details",
                           id = comment.PostId
                       }
                       ));
            }
            ViewData["PostId"] = new SelectList(_context.Post, "PostId", "PostId", comment.PostId);
            ViewData["UserAccountId"] = new SelectList(_context.UserAccount, "UserAccountId", "Email", comment.UserAccountId);
            return View(comment);
        }


        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.CommentId == id);
        }
    }
}
