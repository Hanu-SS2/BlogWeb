using BlogWeb.Data;
using BlogWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogWeb.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly BlogWebContext _context;

        public NavigationViewComponent(BlogWebContext context)
        {
            _context = context;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Factory.StartNew(() => {
                var Session = HttpContext.Session;
                int? userID = Session.GetInt32("UserAccountId");

                if (userID != null)
                {
                    List<Favorite> favorites = _context.Favorite.ToList();
                    ViewBag.Favorites = favorites;

                    var user = _context.UserAccount
                    .FirstOrDefault(u => u.UserAccountId == userID);
                    ViewBag.User = user;
                }
                /*ViewBag.Favorites = ;*/
                return View(_context.Favorite.ToList());
            });
        }
    }
}
