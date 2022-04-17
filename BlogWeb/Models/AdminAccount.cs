using System;
using System.Collections.Generic;

namespace BlogWeb.Models
{
    public partial class AdminAccount
    {
        public AdminAccount()
        {
            Posts = new HashSet<Post>();
        }
        public int AdminAccountId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
