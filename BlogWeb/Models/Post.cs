using System;
using System.Collections.Generic;

namespace BlogWeb.Models
{
    public partial class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
            Favorites = new HashSet<Favorite>();
        }

        public int PostId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Contents { get; set; }
        public string? Thumb { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? IsHot { get; set; }
        public int? CatId { get; set; }
        public int? AdminAccountId { get; set; }

        public virtual AdminAccount? AdminAccount { get; set; }
        public virtual Category? Cat { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
    }
}
