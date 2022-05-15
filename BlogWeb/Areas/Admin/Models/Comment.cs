using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWeb.Areas.Admin.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string Content { get; set; }

        public bool hidden { get; set; }
        public int? PostId { get; set; }
        [ForeignKey("PostId")]
        public Post? Post { get; set; }
    }
}
