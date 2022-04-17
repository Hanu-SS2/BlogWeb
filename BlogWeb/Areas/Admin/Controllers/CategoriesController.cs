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
            var pageSize = 10;
            var lstCategories = _context.Category;
            PagedList<Category> model = new PagedList<Category>(lstCategories, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
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
                .FirstOrDefaultAsync(m => m.Id == id);
            foreach(var cat in _context.Category)
            {
                if(category.Parents == cat.Id)
                {
                    ViewBag.NameParent = cat.Name;
                    break;
                }
                else
                {
                    ViewBag.NameParent = "No";
                }
            }
            ViewBag.ChildList = getChild(category, _context.Category);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            ViewData["parentList"] = new SelectList(_context.Category, "Id", "Name");
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Title,Description,Published,IsRoot,Parents,Level")] Category category)
        {
            if (ModelState.IsValid)
            {
                if(category.IsRoot == true)
                {
                    category.Level = 0;
                    category.Parents = 0;
                }
                else
                {
                    foreach(var cat in _context.Category)
                    {
                        if(cat.Id == category.Parents)
                        {
                            category.Level = cat.Level + 1;
                            break;
                        }
                    }
                }
                _context.Add(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
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
            ViewData["parentList"] = new SelectList(_context.Category.Where(x => x.Id != category.Id), "Id", "Name");
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Title,Description,Published,IsRoot,Parents,Level")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (category.IsRoot == true)
                    {
                        category.Level = 0;
                        category.Parents = 0;
                    }
                    else
                    {
                        foreach (var cat in _context.Category.Where(x => x.Id != category.Id))
                        {
                            if (cat.Id == category.Parents)
                            {
                                category.Level = cat.Level + 1;
                                break;
                            }
                        }
                    }
                    _context.Update(category);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
                .FirstOrDefaultAsync(m => m.Id == id);
            foreach (var cat in _context.Category)
            {
                if (category.Parents == cat.Id)
                {
                    ViewBag.NameParent = cat.Name;
                    break;
                }
                else
                {
                    ViewBag.NameParent = "No";
                }
            }
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
            var category = await _context.Category.FindAsync(id);
            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
        private List<Category> getChild(Category category, DbSet<Category> dbCat)
        {
            List<Category> ChildList = new List<Category>();
            foreach (Category cat in dbCat)
            {
                if(category.Id == cat.Parents)
                {
                    ChildList.Add(cat);
                }
            }
            return ChildList;
        }
    }
}
