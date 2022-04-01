#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Areas.Admin.Models;
using BlogWeb.Data;
using BlogWeb.Helpers;
using PagedList.Core;

namespace BlogWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostsController : Controller
    {
        private readonly BlogWebContext _context;

        public PostsController(BlogWebContext context)
        {
            _context = context;
        }

        // GET: Admin/Posts
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE; //20
            //var pageSize = 2; 

            var lsPost = _context.Post.OrderByDescending(x => x.PostId);

            PagedList<Post> posts = new PagedList<Post>(lsPost, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(posts);
        }

        // GET: Admin/Posts/View/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Post
        //        .FirstOrDefaultAsync(m => m.PostId == id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(post);
        //}

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            ViewBag.CurrentTime = DateTime.UtcNow.ToString("s");
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("PostId,Title,Description,Contents,Thumb,CreateDate")] Post post, Microsoft.AspNetCore.Http.IFormFile Thumb)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (Thumb != null)
        //        {
        //            string extension = Path.GetExtension(Thumb.FileName);
        //            string Newname = Utilities.SEOUrl(post.Title) + "preview" + extension;
        //            post.Thumb = await Utilities.UploadFile(Thumb, @"thumb\", Newname.ToLower());
        //        }


        //        _context.Add(post);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(post);
        //}

        public async Task<IActionResult> Create([Bind("PostId,Title,Description,Contents,Thumb,CreateDate")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }


        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.CurrentTime = DateTime.UtcNow.ToString("s");
            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,Contents,Thumb,CreateDate")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
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
