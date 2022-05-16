using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Data;
using BlogWeb.Models;
using PagedList.Core;

namespace BlogWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CommentsController : Controller
    {
        private readonly BlogWebContext _context;
        private Extensions.Extension exten;

        public CommentsController(BlogWebContext context)
        {
            _context = context;
            exten = new Extensions.Extension(_context);
        }

        // GET: Admin/Comments
        public async Task<IActionResult> Index(int? page)
        {
            if (!exten.isLogedIn(HttpContext))
            {
                return RedirectToAction("Login", "Home", new { Area = "" });
            }
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
            var lstCategories = _context.Comment;
            var lstUser = _context.UserAccount;
            PagedList<Comment> model = new PagedList<Comment>(lstCategories, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(model);
        }

        // GET: Admin/Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comment == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.Post)
                .Include(c => c.UserAccount)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // GET: Admin/Comments/Create
        public IActionResult Create()
        {
            int? accountId = exten.SessionAdminID(HttpContext);
            if (!exten.isRoleAdmin(accountId))
            {
                return RedirectToAction("Index");
            }
            ViewData["PostId"] = new SelectList(_context.Post, "PostId", "PostId");
            ViewData["UserAccountId"] = new SelectList(_context.Set<UserAccount>(), "UserAccountId", "UserAccountId");
            return View();
        }

        // POST: Admin/Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,Contents,CreatedDate,UserAccountId,PostId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Post, "PostId", "PostId", comment.PostId);
            ViewData["UserAccountId"] = new SelectList(_context.Set<UserAccount>(), "UserAccountId", "UserAccountId", comment.UserAccountId);
            return View(comment);
        }

        // GET: Admin/Comments/Edit/5
        public async Task<IActionResult> Hidden(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // POST: Admin/Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Hidden(int id, [Bind("CommentId,UserAccountId,PostId,Content,hidden")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction(nameof(Index));
            }
            return View(comment);
        }

        // GET: Admin/Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            int? accountId = exten.SessionAdminID(HttpContext);
            if (!exten.isRoleAdmin(accountId))
            {
                return RedirectToAction("Index");
            }
            if (id == null || _context.Comment == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.Post)
                .Include(c => c.UserAccount)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Admin/Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comment == null)
            {
                return Problem("Entity set 'BlogWebContext.Comment'  is null.");
            }
            var comment = await _context.Comment.FindAsync(id);
            if (comment != null)
            {
                _context.Comment.Remove(comment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
          return (_context.Comment?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
