using BlogWeb.Data;
using BlogWeb.Extensions;
using BlogWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace BlogWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly BlogWebContext _context;
        private Extensions.Extension exten;

        public HomeController(BlogWebContext context)
        {
            _context = context;
            exten = new Extensions.Extension(_context);

        }
        public IActionResult Index()
        {
            if (!exten.isLogedIn(HttpContext))
            {
                return RedirectToAction("Login", "Home", new { Area = "" });
            }
            return View();
        }
        // Register 
        public IActionResult Register()
        {
            ViewData["Role"] = new SelectList(_context.Set<Role>(), "RoleId", "RoleName");
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(AdminAccount admin)
        {
            if (ModelState.IsValid)
            {
                var check = _context.AdminAccount.FirstOrDefault(s => s.Email == admin.Email);
                if (check == null)
                {
                    admin.Password = GetMD5(admin.Password);
                    /* _dbContext.Configuration.ValidateOnSaveEnabled = false;*/
                    _context.AdminAccount.Add(admin);
                    _context.SaveChanges();
                    ViewBag.Message = admin.FullName + " has been registered successfully!";
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }
            }
            ViewData["Role"] = new SelectList(_context.Set<Role>(), "RoleId", "RoleName");
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();//remove session
            return RedirectToAction("Login", "Home", new { Area = "" });
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
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
