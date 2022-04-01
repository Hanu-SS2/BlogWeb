

using System.ComponentModel.DataAnnotations;

namespace BlogWeb.Models
{
    public class Role
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
        }

        public int RoleId { get; set; }
        //[Required]
        public string? RoleName { get; set; }
        //[Required]
        public string? RoleDescription { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
