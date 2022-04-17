namespace BlogWeb.Areas.Admin.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Published { get; set; }
        public bool IsRoot { get; set; }
        public int Parents { get; set; }
        public int Level { get; set; }
    }
}
