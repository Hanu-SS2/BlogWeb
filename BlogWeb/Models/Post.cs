namespace BlogWeb.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string? Title { get; set; }
        public string? Contents { get; set; }
        public string? Thumb { get; set; }
        public bool Published { get; set; }
        public string? Alias { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? AccountId { get; set; }
        public string? ShortContent { get; set; }
        public string? Author { get; set; }
        public string? Tags { get; set; }
        public int? CartId { get; set; }
        public bool IsHot { get; set; }
        public bool IsNewfeed { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Category? Cart { get; set; }
    }
}
