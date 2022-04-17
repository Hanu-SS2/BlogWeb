using BlogWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogWeb.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private BlogDBContext _context = new BlogDBContext();
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Factory.StartNew(() => {
                var Session = HttpContext.Session;
                int? userID = Session.GetInt32("UserAccountId");

                if (userID != null)
                {
                    List<Favorite> favorites = _context.Favorites.ToList();
                    ViewBag.Favorites = favorites;

                    var user = _context.UserAccounts
                    .FirstOrDefault(u => u.UserAccountId == userID);
                    ViewBag.User = user;
                }
                /*ViewBag.Favorites = ;*/
                return View(_context.Favorites.ToList()); 
            });
        }
    }
}
