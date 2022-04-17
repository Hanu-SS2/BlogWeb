using BlogWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;


namespace BlogWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private BlogDBContext _dbContext = new BlogDBContext();


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
           /* List<Post> listPost = _dbContext.Posts.ToList();*/
        /*    return View(listPost);*/
            return View();

        }

        // Register 
        public IActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserAccount _user)
        {
            if (ModelState.IsValid)
            {
                var check = _dbContext.UserAccounts.FirstOrDefault(s => s.Email == _user.Email);
                if (check == null)
                {
                    _user.Password = GetMD5(_user.Password);
                    /* _dbContext.Configuration.ValidateOnSaveEnabled = false;*/
                    _dbContext.UserAccounts.Add(_user);
                    _dbContext.SaveChanges();
                    ViewBag.Message = _user.FirstName + " " + _user.LastName + " has been registered successfully!";
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }
            }
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLogin userLogin)
        {
            var Session = HttpContext.Session; 
            if (ModelState.IsValid)
            {
                var f_password = GetMD5(userLogin.Password);
                var data = _dbContext.UserAccounts.Where(s => s.Email.Equals(userLogin.Email) && s.Password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    Session.SetString("FullName", data.FirstOrDefault().FirstName + data.FirstOrDefault().LastName);
                    Session.SetString("Email", data.FirstOrDefault().Email);
                    Session.SetInt32("UserAccountId", data.FirstOrDefault().UserAccountId);
                    return RedirectToAction("Index", "Post");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return View();
                }

            }
            return View();
        }

        //Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();//remove session
            return RedirectToAction("Login");
        }

        //create a string MD5
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