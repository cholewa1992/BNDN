using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ShareItServices;
using ShareItServices.AuthService;

namespace ArtShare.Models
{
    public class LoginModel
    {
        [Display(Name = "User")]
        public UserDTO User { get; set; }

        [Display(Name = "Login status")]
        public bool LoggedIn { get; set; }
    }
}