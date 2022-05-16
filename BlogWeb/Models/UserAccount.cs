using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogWeb.Models
{
    public partial class UserAccount
    {
        public UserAccount()
        {
            Comments = new HashSet<Comment>();
            Favorites = new HashSet<Favorite>();
        }
        [Key]
        public int UserAccountId { get; set; }
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string? Email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")]
        public string? Password { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string? FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]

        public string? LastName { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
    }
}