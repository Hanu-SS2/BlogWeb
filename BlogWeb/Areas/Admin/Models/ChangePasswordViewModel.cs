using System.ComponentModel.DataAnnotations;

namespace BlogWeb.Areas.Admin.Models
{
    public class ChangePasswordViewModel
    {
        [Key]
        public int AccountId { get; set; }
        [Display(Name ="Current password")]
        public string PasswordNow { get; set; }
        [Display (Name ="New password")]
        [Required(ErrorMessage = "Please enter current password")]
        [MinLength(6,ErrorMessage = "You need to set a password of at least 5 characters")]
        public string Password { get; set; }
        [MinLength(6,ErrorMessage = "You need to set a password of at least 5 characters")]
        [Display(Name ="Please enter new password")]
        [Compare("Password", ErrorMessage = "Passwords are not the same")]
        public string ConfirmPassword { get; set; }
    }
}
