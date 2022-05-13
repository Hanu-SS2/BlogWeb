using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWeb.Areas.Admin.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }

        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        [DataType(DataType.Text)]
        public string Content { set; get; }

        public bool Published { get; set; }

        public ICollection<Category>? CategoryChildren { get; set; }

        [ForeignKey("ParentCategoryId")]
        public Category? ParentCategory { set; get; }
        public ICollection<Post>? Posts { get; set; }
    }
}
