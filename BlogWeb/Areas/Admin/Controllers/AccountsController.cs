using BlogWeb.Areas.Admin.Models;
using BlogWeb.Data;
using BlogWeb.Extensions;
using BlogWeb.Helpers;
using BlogWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Security.Claims;

namespace BlogWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Accounts
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;//20
            var isAccounts = _context.Account
                .Include(a => a.Role)
                .OrderByDescending(a => a.CreatedAt);
            PagedList<Account> models = new PagedList<Account>(isAccounts, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }


        [HttpGet]
        [AllowAnonymous] // all can access login page
        [Route("login.html", Name = "Login")]
        public IActionResult Login(string returnUrl = null)
        {
            var accountID = HttpContext.Session.GetString("AccountId");
            if (accountID != null) return RedirectToAction("Index", "Home", new { Area = "Admin" });
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("login.html", Name = "Login")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Account kh = _context.Account
                        .Include(p => p.Role)
                        .SingleOrDefault(p => p.Email.ToLower().Trim() == model.Email.ToLower().Trim());//fix trim
                    if (kh == null)
                    {
                        ViewBag.Error = "Email or password incorrect";
                        return View(model);
                    }
                    //string pass = (model.Password.Trim() + kh.Salt.Trim()).ToMD5();
                    string pass = model.Password.Trim().ToMD5();
                    if (kh.Password.Trim() != pass)
                    {
                        ViewBag.Error = "Email or password incorrect";
                        return View(model);
                    }

                    kh.LastLogin = DateTime.Now;
                    _context.Update(kh);
                    await _context.SaveChangesAsync();

                    var AccountID = HttpContext.Session.GetString("AccountId");
                    HttpContext.Session.SetString("AccountId", kh.AccountId.ToString());
                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,kh.FullName),
                        new Claim(ClaimTypes.Email,kh.Email),
                        new Claim("AccountId",kh.AccountId.ToString()),
                        new Claim("RoleId",kh.RoleId.ToString()),
                        new Claim(ClaimTypes.Role,kh.Role.RoleName)
                    };

                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                    await HttpContext.SignInAsync(userPrincipal);

                    //if (Url.IsLocalUrl(returnUrl))
                    //{
                    //    return Redirect(returnUrl);
                    //}
                    //return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }

            }
            catch
            {
                return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            }
            return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
        }

        [Route("change-password.html", Name = "ChangePassword")]
        [Authorize, HttpGet]
        public IActionResult ChangePassword()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/login.html");
            var accountId = HttpContext.Session.GetString("AccountId");
            if (accountId == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            return View();
        }

        [Route("change-password.html", Name = "ChangePassword")]
        [Authorize, HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/login.html");
            var accountId = HttpContext.Session.GetString("AccountId");
            if (accountId == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            if (ModelState.IsValid)
            {
                var account = _context.Account.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(accountId));
                if (account != null) return RedirectToAction("Login", "Account", new { Area = "Admin" });
                try
                {
                    string passnow = (model.PasswordNow + account.Salt.Trim()).ToMD5();
                    if (passnow == account.Password.Trim())
                    {
                        account.Password = (model.Password + account.Salt.Trim()).ToMD5();
                        _context.Update(account);
                        _context.SaveChanges();
                        return RedirectToAction("Profile", "Account", new { Area = "Admin" });
                    }
                    else
                    {
                        return View();
                    }
                }
                catch
                {
                    return View();
                }
            }
            return View();
        }

        [Route("edit-profile.html", Name = "EditProfile")]
        [Authorize, HttpGet]
        public IActionResult EditProfile()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/login.html");
            var accountId = HttpContext.Session.GetString("AccountId");
            var account = _context.Account.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(accountId));
            if (account == null) return NotFound();
            return View(account);
        }

        [Route("edit-profile.html", Name = "EditProfile")]
        [Authorize, HttpPost]
        public IActionResult EditProfile(Account model)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/login.html");
            var accountId = HttpContext.Session.GetString("AccountId");
            if (accountId == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            if (ModelState.IsValid)
            {
                var account = _context.Account.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(accountId));
                try
                {
                    account.FullName = model.FullName;
                    account.Phone = model.Phone;
                    account.Email = model.Email;
                    _context.Update(account);
                    _context.SaveChanges();
                    return RedirectToAction("Profile", "Account", new { Area = "Admin" });
                }
                catch
                {
                    return View(model);
                }
            }
            return View();
        }


        [Route("logout.html", Name = "Logout")]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.SignOutAsync();
                HttpContext.Session.Remove("AccountId");
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/Accounts/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "RoleId", "RoleId");
            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,FullName,Email,Phone,Password,Salt,Active,CreatedAt,RoleId,LastLogin")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,FullName,Email,Phone,Password,Salt,Active,CreatedAt,RoleId,LastLogin")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
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
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "RoleId", "RoleId", account.RoleId);
            return View(account);
        }



        // GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Account.FindAsync(id);
            _context.Account.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Account.Any(e => e.AccountId == id);
        }
    }
}
