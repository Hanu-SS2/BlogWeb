using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWeb.Areas.Admin.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        [MaxLength]
        [Required]
        public string Contents { get; set; }
        //public string Thumb { get; set; }

        public string? imageName { get; set; }
        [NotMapped]
        public IFormFile? image { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime CreateDate { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
