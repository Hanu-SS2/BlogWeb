namespace BlogWeb.Areas.Admin.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        public string FullName { get; set; }
    }
}
