#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Areas.Admin.Models;
using BlogWeb.Data;
using PagedList.Core;

namespace BlogWeb.Areas.Admin
{
    [Area("Admin")]
    public class PostsController : Controller
    {
 
        private readonly BlogWebDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostsController(BlogWebDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this._webHostEnvironment = webHostEnvironment;
        }
        // GET: Admin/Posts
        [HttpGet]
        public IActionResult Index(int? page, string? searchSring)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20; //20
            //var pageSize = 2;

            var lsPost = _context.Post.Include(c => c.Category).AsQueryable();
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
                    CategoryId = item.CategoryId,
                    Title = prefix + " " + item.Title
                });
                if (item.CategoryChildren?.Count > 0)
                {
                    CreateSelectItems(item.CategoryChildren.ToList(), des, level + 1);
                }
            }
        }
        // GET: Admin/Posts/Create
        [HttpGet]
        public IActionResult Create()
        {
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
            ViewData["CategoryId"] = new SelectList(items, "CategoryId", "Title");
            ViewBag.CurrentTime = DateTime.UtcNow.ToString("s");
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Description,Contents,image,CreateDate,CategoryId")] Post post)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(post.image.FileName);
            string extension = Path.GetExtension(post.image.FileName);

            post.imageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

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
            ViewData["CategoryId"] = new SelectList(items, "CategoryId", "Title");
            ViewBag.CurrentTime = DateTime.UtcNow.ToString("s");
            return View(post);
        }
        // GET: Admin/Posts/Edit/5
        [HttpGet]
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
            ViewData["CategoryId"] = new SelectList(items, "CategoryId", "Title");
            ViewBag.CurrentTime = DateTime.UtcNow.ToString("s");
            return View(post);
        }
        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,Contents,CreateDate,CategoryId,image,imageName")] Post post)
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
                post.imageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
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
            ViewData["CategoryId"] = new SelectList(items, "CategoryId", "Title");
            ViewBag.CurrentTime = DateTime.UtcNow.ToString("s");
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
