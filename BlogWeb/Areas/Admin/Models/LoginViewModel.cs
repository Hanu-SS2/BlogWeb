using System.ComponentModel.DataAnnotations;

namespace BlogWeb.Areas.Admin.Models
{
    public class LoginViewModel
    {
        [Key]
        [MaxLength(50)]
        [Required(ErrorMessage ="Email is required")]
        [Display(Name ="Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage ="Re-enter your email")]
        public string Email { get; set; }

        [Display(Name ="Password")]
        [Required(ErrorMessage ="Password is required")]
        [MaxLength(30,ErrorMessage = "Password must be between 6-30 characters")]
        [MinLength(6,ErrorMessage = "Password must be between 6-30 characters")]
        public string Password { get; set; }
    }
}
