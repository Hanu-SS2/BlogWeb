#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Data;
using BlogWeb.Models;
using Microsoft.AspNetCore.Authorization;
using PagedList.Core;
using BlogWeb.Helpers;

namespace BlogWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize()]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Posts
        public IActionResult Index(int? page)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/login.html");
            var accountId = HttpContext.Session.GetString("AccountId");
            if (accountId == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            var account = _context.Account.AsNoTracking().FirstOrDefault(a => a.AccountId == int.Parse(accountId));
            if (account == null) return NotFound();

            List<Post> isPosts = new List<Post>();
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE; // 20
            if (account.RoleId == 3)
            {
                isPosts = _context.Post.Include(p => p.Account)
                .Include(p => p.Cart)
                .OrderByDescending(x => x.CartId)
                .ToList();
            }
            else
            {
                isPosts = _context.Post.Include(p => p.Account).Include(p => p.Cart)
                .Where(x => x.AccountId == account.AccountId)
                .OrderByDescending(x => x.CartId)
                .ToList();
            }
            PagedList<Post> posts = new PagedList<Post>(isPosts.AsQueryable(), pageNumber, pageSize);
            return View(posts);
        }

        // GET: Admin/Posts/Details/5
        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Account)
                .Include(p => p.Cart)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/login.html");
            var accountId = HttpContext.Session.GetString("AccountId");
            if (accountId == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            ViewData["Category"] = new SelectList(_context.Category, "CartId", "CartName");
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Contents,Thumb,Published,Alias,CreatedDate,AccountId,ShortContent,Author,Tags,CartId,IsHot,IsNewfeed")] Post post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/login.html");
            var accountId = HttpContext.Session.GetString("AccountId");
            if (accountId == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            var account = _context.Account.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(accountId));
            if (account == null) return NotFound();
            if (account.RoleId != 3)
            {
                if (post.AccountId != account.AccountId) return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                _ = post.AccountId == account.AccountId;
                _ = post.Author == account.FullName;
                if (post.CartId == null)
                {
                    post.CreatedDate = DateTime.Now;
                    post.Alias = Utilities.SEOUrl(post.Title);
                    //post.Views = 0;
                }
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string Newname = Utilities.SEOUrl(post.Title) + extension;
                    post.Thumb = await Utilities.UploadFile(fThumb, @"news\", Newname.ToLower());
                }

            }
            ViewData["AccountId"] = new SelectList(_context.Account, "AccountId", "AccountId", post.AccountId);
            ViewData["CartId"] = new SelectList(_context.Category, "CartId", "CartId", post.CartId);
            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Account, "AccountId", "AccountId", post.AccountId);
            ViewData["CartId"] = new SelectList(_context.Category, "CartId", "CartId", post.CartId);
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Contents,Thumb,Published,Alias,CreatedDate,AccountId,ShortContent,Author,Tags,CartId,IsHot,IsNewfeed")] Post post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/login.html");
            var accountId = HttpContext.Session.GetString("AccountId");
            if (accountId == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            var account = _context.Account.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(accountId));
            if (account == null) return NotFound();
            if (account.RoleId != 3)
            {
                if (post.AccountId != account.AccountId) return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string Newname = Utilities.SEOUrl(post.Title) + extension;
                        post.Thumb = await Utilities.UploadFile(fThumb, @"news\", Newname.ToLower());
                    }
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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
            ViewData["AccountId"] = new SelectList(_context.Account, "AccountId", "AccountId", post.AccountId);
            ViewData["CartId"] = new SelectList(_context.Category, "CartId", "CartId", post.CartId);
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Account)
                .Include(p => p.Cart)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.PostId == id);
        }
    }
}
