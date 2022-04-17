using System;
using System.Collections.Generic;

namespace BlogWeb.Models
{
    public partial class UserAccount
    {
        public UserAccount()
        {
            Comments = new HashSet<Comment>();
            Favorites = new HashSet<Favorite>();
        }

        public int UserAccountId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
    }
}
