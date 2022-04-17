using System;
using System.Collections.Generic;

namespace BlogWeb.Models
{
    public partial class Category
    {
        public Category()
        {
            Posts = new HashSet<Post>();
        }

        public int CatId { get; set; }
        public string? CatName { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
