using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogWeb.Models
{
    public partial class Role
    {
        public Role()
        {
            AdminAccounts = new HashSet<AdminAccount>();
        }

        [Key]
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? RoleDescription { get; set; }

        public virtual ICollection<AdminAccount> AdminAccounts { get; set; }
    }
}