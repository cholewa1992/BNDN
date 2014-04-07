using System.ComponentModel.DataAnnotations;

namespace ArtShare.Models
{
    public class UpdateAccountModel
    {

        [Display(Name = "User Name")]
        public string Username { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

    }
}