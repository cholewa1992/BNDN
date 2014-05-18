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
            var model = new Logic.LoginLogic().Login(username, password);

            if (model.LoggedIn)
            {
                var userCookie = new HttpCookie("user");
                userCookie["id"] = model.User.Id + "";
                userCookie["username"] = model.User.Username;
                userCookie["password"] = model.User.Password;
                userCookie.Expires = DateTime.Now.AddDays(365);
                HttpContext.Response.Cookies.Add(userCookie);
                if (TempData["redirectTo"] != null)
                    return Redirect(TempData["redirectTo"] as string);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = "Username or password was incorrect";
                return RedirectToAction("Index", "Login");
            }
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
