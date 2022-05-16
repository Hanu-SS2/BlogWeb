using BlogWeb.Data;

namespace BlogWeb.Extensions
{
    public class Extension
    {
        private readonly BlogWebContext _context;

        public Extension(BlogWebContext context)
        {
            _context = context;
        }

        public bool isRoleAdmin(int? AdminAccountId)
        {
            bool isRole = false;
            int roleId = 1;
            var account = _context.AdminAccount.Where(a => a.AdminAccountId == AdminAccountId).FirstOrDefault();
            if (account.RoleId == 1)
            {
                isRole = true;
            }
            else
            {
                isRole = false;
            }
            return isRole;
        }
        public bool isLogedIn(HttpContext http)
        {
            int? AdminAccountId = SessionAdminID(http);
            if (AdminAccountId == null)
            {
                return false;
            }
            return true;
        }
        public int? SessionUserID(HttpContext http)
        {
            int? sessionID = http.Session.GetInt32("UserAccountId");
            if(sessionID == null)
            {
                return null;
            }
            return sessionID;
        }
        public int? SessionAdminID(HttpContext http)
        {
            int? sessionAdminID = http.Session.GetInt32("AdminAccountId");
            if (sessionAdminID == null)
            {
                return null;
            }
            return sessionAdminID;
        }
    }
        
}
