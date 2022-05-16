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
    public class CategoriesController : Controller
    {
        private readonly BlogWebContext _context;
        private Extensions.Extension exten;

        public CategoriesController(BlogWebContext context)
        {
            _context = context;
            exten = new Extensions.Extension(_context);
        }

        // GET: Admin/Categories
        public IActionResult Index(int? page)
        {
            if (!exten.isLogedIn(HttpContext))
            {
                return RedirectToAction("Login", "Home", new { Area = "" });
            }
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
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
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.CatId == id);
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
                    CatId = item.CatId,
                    CatName = prefix + " " + item.CatName
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
            categories.Insert(0, new Category()
            {
                CatName = "Root",
                CatId = -1
            });
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);

            ViewData["ParentCategoryId"] = new SelectList(items, "CatId", "CatName");
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,ParentCategoryId,CatName,Description,Published")] Category category)
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
                CatName = "Root",
                CatId = -1
            });
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);

            ViewData["ParentCategoryId"] = new SelectList(items, "CatId", "CatName");
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
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
                CatName = "Root",
                CatId = -1
            });
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);

            ViewData["ParentCategoryId"] = new SelectList(items, "CatId", "CatName", category.ParentCategoryId);
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CatId,ParentCategoryId,CatName,Description,Published")] Category category)
        {
            if (id != category.CatId)
            {
                return NotFound();
            }
            if (category.ParentCategoryId == category.CatId)
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
                    if (!CategoryExists(category.CatId))
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
                CatName = "Root",
                CatId = -1
            });
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);

            ViewData["ParentCategoryId"] = new SelectList(items, "CatId", "CatName", category.ParentCategoryId);
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            int? accountId = exten.SessionAdminID(HttpContext);
            if (!exten.isRoleAdmin(accountId))
            {
                return RedirectToAction("Index");
            }
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.CatId == id);
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
            if (_context.Category == null)
            {
                return Problem("Entity set 'BlogWebContext.Category'  is null.");
            }
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                _context.Category.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Category?.Any(e => e.CatId == id)).GetValueOrDefault();
        }
    }
}
