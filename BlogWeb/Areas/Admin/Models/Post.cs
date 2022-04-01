
using System.ComponentModel.DataAnnotations;

namespace BlogWeb.Areas.Admin.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string Thumb { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime CreateDate { get; set; }

    }
}
