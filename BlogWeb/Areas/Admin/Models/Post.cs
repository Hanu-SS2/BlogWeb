
namespace BlogWeb.Areas.Admin.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string Thumb { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
