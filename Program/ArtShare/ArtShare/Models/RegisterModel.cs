using System.ComponentModel.DataAnnotations;

namespace ArtShare.Models
{
    public class RegisterModel
    {
        [Display(Name = "User Name")]
        public string Username { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        public string RetypePassword { get; set; }

        public string Error { get; set; }
    }
}