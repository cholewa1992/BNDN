using System;
using System.Web;
using System.Web.Mvc;
using ArtShare.Models;
using ShareItServices.AuthService;

namespace ArtShare.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Default1/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            //Creates a new model
            var model = new LoginModel();

            //Opens a AuthServiceClient
            using (var client = new AuthServiceClient())
            {
                //Creating user object
                model.User = new UserDTO {Username = username, Password = password};
                //Validating user
                model.User.Id = client.ValidateUser(model.User, Properties.Resources.ClientToken);
                //If the user was validated, set LoggedIn property to true
                model.LoggedIn = model.User.Id > 0;
            }

            var userCookie = new HttpCookie("user", model.User.Id + "");
            userCookie["id"] = model.User.Id + "";
            userCookie["username"] = model.User.Username;
            userCookie.Expires.AddDays(365);
            HttpContext.Response.Cookies.Add(userCookie);

            return RedirectToAction("Index", "Home");
            return View(model);
        }

        public ActionResult Logout()
        {
            if (Request.Cookies["user"] != null)
            {
                var user = new HttpCookie("user")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Value = null
                };
                Response.Cookies.Add(user);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
