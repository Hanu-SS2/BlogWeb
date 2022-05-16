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
    public class AdminAccountsController : Controller
    {
        private readonly BlogWebContext _context;
        private Extensions.Extension exten;

        public AdminAccountsController(BlogWebContext context)
        {
            _context = context;
            exten = new Extensions.Extension(_context);
        }

        // GET: Admin/AdminAccounts
        public async Task<IActionResult> Index()
        {
            if (!exten.isLogedIn(HttpContext))
            {
                return RedirectToAction("Login", "Home", new { Area = "" });
            }
            var blogWebContext = _context.AdminAccount.Include(a => a.Role);
            return View(await blogWebContext.ToListAsync());
        }

        // GET: Admin/AdminAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AdminAccount == null)
            {
                return NotFound();
            }

            var adminAccount = await _context.AdminAccount
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AdminAccountId == id);
            if (adminAccount == null)
            {
                return NotFound();
            }

            return View(adminAccount);
        }

        // GET: Admin/AdminAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.AdminAccount == null)
            {
                return NotFound();
            }

            var adminAccount = await _context.AdminAccount.FindAsync(id);
            if (adminAccount == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "RoleId", "RoleId", adminAccount.RoleId);
            return View(adminAccount);
        }

        // POST: Admin/AdminAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminAccountId,Email,Password,FullName,RoleId")] AdminAccount adminAccount)
        {
            if (id != adminAccount.AdminAccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    adminAccount.Password = GetMD5(adminAccount.Password);
                    _context.Update(adminAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminAccountExists(adminAccount.AdminAccountId))
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
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "RoleId", "RoleId", adminAccount.RoleId);
            return View(adminAccount);
        }

        // GET: Admin/AdminAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            int? accountId = exten.SessionAdminID(HttpContext);
            if (!exten.isRoleAdmin(accountId))
            {
                return RedirectToAction("Index");
            }

            if (id == null || _context.AdminAccount == null)
            {
                return NotFound();
            }

            var adminAccount = await _context.AdminAccount
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AdminAccountId == id);
            if (adminAccount == null)
            {
                return NotFound();
            }

            return View(adminAccount);
        }

        // POST: Admin/AdminAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AdminAccount == null)
            {
                return Problem("Entity set 'BlogWebContext.AdminAccount'  is null.");
            }
            var adminAccount = await _context.AdminAccount.FindAsync(id);
            if (adminAccount != null)
            {
                _context.AdminAccount.Remove(adminAccount);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminAccountExists(int id)
        {
          return (_context.AdminAccount?.Any(e => e.AdminAccountId == id)).GetValueOrDefault();
        }
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;

        }
    }
}
