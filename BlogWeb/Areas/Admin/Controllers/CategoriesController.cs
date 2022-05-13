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

namespace BlogWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly BlogWebDbContext _context;

        public CategoriesController(BlogWebDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var qr = (from c in _context.Category select c)
            .Include(c => c.ParentCategory)                // load parent category
            .Include(c => c.CategoryChildren);             // load child category

            var categories = (qr.ToList())
                             .Where(c => c.ParentCategory == null).ToList().AsQueryable();
            PagedList<Category> model = new PagedList<Category>(categories, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            //var blogWebContext = _context.Category.Include(c => c.ParentCategory);
            return View(model);
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .Include(c => c.ParentCategory)
                .Include(c => c.CategoryChildren)
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
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

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            var qr = (from c in _context.Category select c)
            .Include(c => c.ParentCategory)                // load parent category
            .Include(c => c.CategoryChildren);             // load child category

            var categories = (qr.ToList())
                             .Where(c => c.ParentCategory == null).ToList();
            //var listcategory =  _context.Category.ToList();
            categories.Insert(0, new Category()
            {
                Title = "Root",
                CategoryId = -1
            });
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);

            ViewData["ParentCategoryId"] = new SelectList(items, "CategoryId", "Title");
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CategoryId,ParentCategoryId,Title,Content,Published")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.ParentCategoryId.Value == -1)
                {
                    category.ParentCategoryId = null;
                }
                _context.Add(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            var qr = (from c in _context.Category select c)
                     .Include(c => c.ParentCategory)                // load parent category
                     .Include(c => c.CategoryChildren);             // load child category

            var categories = (qr.ToList())
                             .Where(c => c.ParentCategory == null).ToList();
            //var listcategory =  _context.Category.ToList();
            categories.Insert(0, new Category()
            {
                Title = "Root",
                CategoryId = -1
            });
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);

            ViewData["ParentCategoryId"] = new SelectList(items, "CategoryId", "Title");
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var qr = (from c in _context.Category select c)
                     .Include(c => c.ParentCategory)                // load parent category
                     .Include(c => c.CategoryChildren);             // load child category

            var categories = (qr.ToList())
                             .Where(c => c.ParentCategory == null).ToList();
            //var listcategory =  _context.Category.ToList();
            categories.Insert(0, new Category()
            {
                Title = "Root",
                CategoryId = -1
            });
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);

            ViewData["ParentCategoryId"] = new SelectList(items, "CategoryId", "Title", category.ParentCategoryId);
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,ParentCategoryId,Title,Content,Published")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }
            if (category.ParentCategoryId == category.CategoryId)
            {
                ModelState.AddModelError(string.Empty, "select other categories");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (category.ParentCategoryId.Value == -1)
                    {
                        category.ParentCategoryId = null;
                    }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            var qr = (from c in _context.Category select c)
                     .Include(c => c.ParentCategory)                // load parent category
                     .Include(c => c.CategoryChildren);             // load child category

            var categories = (qr.ToList())
                             .Where(c => c.ParentCategory == null).ToList();
            //var listcategory =  _context.Category.ToList();
            categories.Insert(0, new Category()
            {
                Title = "Root",
                CategoryId = -1
            });
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);

            ViewData["ParentCategoryId"] = new SelectList(items, "CategoryId", "Title", category.ParentCategoryId);
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Category
                .Include(c => c.CategoryChildren)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            foreach (var cCategory in category.CategoryChildren)
            {
                cCategory.ParentCategoryId = category.ParentCategoryId;
            }
            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.CategoryId == id);
        }
    }
}

