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
    public class PostsController : Controller
    {
        private readonly BlogWebContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private Extensions.Extension exten;

        public PostsController(BlogWebContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this._webHostEnvironment = webHostEnvironment;
            exten = new Extensions.Extension(_context);
        }

        // GET: Admin/Posts
        [HttpGet]
        public IActionResult Index(int? page, string? searchSring)
        {
            if (!exten.isLogedIn(HttpContext))
            {
                return RedirectToAction("Login", "Home", new { Area = "" });
            }
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 2; //20
            //var pageSize = 2;

            var lsPost = _context.Post.Include(c => c.Cat).Include(a => a.AdminAccount).AsQueryable();
            if (!string.IsNullOrEmpty(searchSring))
            {
                lsPost = lsPost.Where(x => x.Title.Contains(searchSring) || x.Description.Contains(searchSring));
            }

            lsPost.OrderByDescending(x => x.PostId);
            PagedList<Post> posts = new PagedList<Post>(lsPost, pageNumber, pageSize);
            ViewBag.SearchSring = searchSring;

            ViewBag.CurrentPage = pageNumber;
            return View(posts);
        }

        private void CreateSelectItems(List<Category> source, List<Category> des, int level)
        {
            string prefix = String.Concat(Enumerable.Repeat("----", level));
            foreach (var item in source)
            {
                //item.Title = prefix + " " + item.Title;
                des.Add(new Category()
                {
                    CatId = item.CatId,
                    CatName = prefix + " " + item.CatName
                });
                if (item.CategoryChildren?.Count > 0)
                {
                    CreateSelectItems(item.CategoryChildren.ToList(), des, level + 1);
                }
            }
        }
        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.AdminAccount)
                .Include(p => p.Cat)
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
            int? accountId = exten.SessionAdminID(HttpContext);
            if (!exten.isRoleAdmin(accountId))
            {
                return RedirectToAction("Index");
            }
            var qr = (from c in _context.Category select c)
           .Include(c => c.ParentCategory)                // load parent category
           .Include(c => c.CategoryChildren);             // load child category

            var categories = (qr.ToList())
                             .Where(c => c.ParentCategory == null).ToList();
            //var listcategory =  _context.Category.ToList();
            //categories.Insert(0, new Category()
            //{
            //    Title = "Root",
            //    Id = -1
            //});
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);
            ViewData["AdminAccountId"] = new SelectList(_context.AdminAccount, "AdminAccountId", "FullName");
            ViewData["CategoryId"] = new SelectList(items, "CatId", "CatName");
            ViewBag.CurrentTime = DateTime.UtcNow.ToString("s");
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Description,Contents,image,CreatedDate,IsHot,CatId,AdminAccountId")] Post post)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(post.image.FileName);
            string extension = Path.GetExtension(post.image.FileName);

            post.Thumb = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

            string path = Path.Combine(wwwRootPath + "\\images\\", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await post.image.CopyToAsync(fileStream);
            }
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var qr = (from c in _context.Category select c)
           .Include(c => c.ParentCategory)                // load parent category
           .Include(c => c.CategoryChildren);             // load child category

            var categories = (qr.ToList())
                             .Where(c => c.ParentCategory == null).ToList();
            //var listcategory =  _context.Category.ToList();
            //categories.Insert(0, new Category()
            //{
            //    Title = "Root",
            //    Id = -1
            //});
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);
            ViewData["AdminAccountId"] = new SelectList(_context.AdminAccount, "AdminAccountId", "FullName");
            ViewData["CategoryId"] = new SelectList(items, "CatId", "CatName");
            ViewBag.CurrentTime = DateTime.UtcNow.ToString("s");
            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            int? accountId = exten.SessionAdminID(HttpContext);
            if (!exten.isRoleAdmin(accountId))
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            // View Edit
            var qr = (from c in _context.Category select c)
           .Include(c => c.ParentCategory)                // load parent category
           .Include(c => c.CategoryChildren);             // load child category

            var categories = (qr.ToList())
                             .Where(c => c.ParentCategory == null).ToList();
            //var listcategory =  _context.Category.ToList();
            //categories.Insert(0, new Category()
            //{
            //    Title = "Root",
            //    Id = -1
            //});
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);
            ViewData["AdminAccountId"] = new SelectList(_context.AdminAccount, "AdminAccountId", "FullName");
            ViewData["CategoryId"] = new SelectList(items, "CatId", "CatName");
            ViewBag.CurrentTime = DateTime.UtcNow.ToString("s");
            return View(post);
        }
        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,Contents,CreatedDate,CatId,IsHot,image,imageName,AdminAccountId")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }
            if (post.image != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(post.image.FileName);
                string extension = Path.GetExtension(post.image.FileName);
                post.Thumb = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "\\images\\", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await post.image.CopyToAsync(fileStream);
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
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
            // View Edit
            var qr = (from c in _context.Category select c)
           .Include(c => c.ParentCategory)                // load parent category
           .Include(c => c.CategoryChildren);             // load child category

            var categories = (qr.ToList())
                             .Where(c => c.ParentCategory == null).ToList();
            //var listcategory =  _context.Category.ToList();
            //categories.Insert(0, new Category()
            //{
            //    Title = "Root",
            //    Id = -1
            //});
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);
            ViewData["AdminAccountId"] = new SelectList(_context.AdminAccount, "AdminAccountId", "FullName");
            ViewData["CategoryId"] = new SelectList(items, "CatId", "CatName");
            ViewBag.CurrentTime = DateTime.UtcNow.ToString("s");
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            int? accountId = exten.SessionAdminID(HttpContext);
            if (!exten.isRoleAdmin(accountId))
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
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