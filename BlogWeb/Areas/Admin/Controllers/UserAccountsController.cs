using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Data;
using BlogWeb.Models;
using System.Security.Cryptography;
using System.Text;

namespace BlogWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserAccountsController : Controller
    {
        private readonly BlogWebContext _context;
        private Extensions.Extension exten;

        public UserAccountsController(BlogWebContext context)
        {
            _context = context;
            exten = new Extensions.Extension(_context);
        }

        // GET: Admin/UserAccounts
        public async Task<IActionResult> Index()
        {
            if (!exten.isLogedIn(HttpContext))
            {
                return RedirectToAction("Login", "Home", new { Area = "" });
            }
            return _context.UserAccount != null ? 
                          View(await _context.UserAccount.ToListAsync()) :
                          Problem("Entity set 'BlogWebContext.UserAccount'  is null.");
        }

        // GET: Admin/UserAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserAccount == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccount
                .FirstOrDefaultAsync(m => m.UserAccountId == id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }
    }
}
