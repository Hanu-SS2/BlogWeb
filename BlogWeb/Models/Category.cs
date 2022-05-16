using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWeb.Models
{
    public class Category
    {
        public Category()
        {
            Posts = new HashSet<Post>();
        }
        [Key]
        public int CatId { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? CatName { get; set; }
        public string? Description { get; set; }
        public bool Published { get; set; }

        public ICollection<Category>? CategoryChildren { get; set; }

        [ForeignKey("ParentCategoryId")]
        public Category? ParentCategory { set; get; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
