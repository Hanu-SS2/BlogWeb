using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogWeb.Models
{
    public partial class AdminAccount
    {
        public AdminAccount()
        {
            Posts = new HashSet<Post>();
        }
        [Key]
        public int AdminAccountId { get; set; }
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string? Email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")]
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}