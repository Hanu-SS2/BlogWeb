using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogWeb.Models
{
    public partial class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }
        public int? UserAccountId { get; set; }
        public int? PostId { get; set; }

        public virtual Post? Post { get; set; }
        public virtual UserAccount? UserAccount { get; set; }
    }
}