using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtShare.Logic;
using ArtShare.Models;
using Microsoft.Ajax.Utilities;
using ShareItServices.UserService;

namespace ArtShare.Controllers
{
    public class AccountController : Controller
    {

        private IAccountLogic _accountLogic;

        public AccountController()
        {
            _accountLogic = new AccountLogic();
        }

        public AccountController(IAccountLogic logic)
        {
            _accountLogic = logic;
        }

        //
        // GET: /Account/

        public ActionResult Index()
        {
            return RedirectToAction("Register");
        }

        //
        // GET: /Account/Details/5

        public ActionResult Details(int id)
        {
            try
            {
                //Checking that the user is logged in
                if (Request.Cookies["user"] == null)
                {
                    TempData["error"] = "You have to login to see you profil page";
                    return RedirectToAction("Index", "Home");
                }

                //Fetching and passing Account model to view
                return View(_accountLogic.GetAccountInformation(
                    Request.Cookies["user"].Values["username"],
                    Request.Cookies["user"].Values["password"],
                    id));
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Edit(int id)
        {
            return Details(id);
        }

        [HttpPost]
        public ActionResult Edit(AccountModel am)
        {
            try
            {
                //Checking if a user is logged in
                if (Request.Cookies["user"] == null)
                {
                    TempData["error"] = "You have to login to see you profil page";
                    return RedirectToAction("Index", "Home");
                }

                //Checking if a the user editing is the user being edited
                //NOTE: This restricting is only here beacuse it's not possbile for an admin to edit a user with the current backend
                if (Request.Cookies["user"].Values["username"] != am.Username)
                {
                    TempData["error"] = "A user can only edit his own account";
                    return RedirectToAction("Details", new { am.Id });
                }

                //Checking if the user changed his password, otherwise the current password is beging used
                am.Password = string.IsNullOrWhiteSpace(am.Password) ? Request.Cookies["user"].Values["password"] : am.Password;

                //Sending a request to the backend
                _accountLogic.UpdateAccountInformation(Request.Cookies["user"].Values["username"], Request.Cookies["user"].Values["password"], am);

                //Logging in again (Is case the password was changed
                var model = new Logic.LoginLogic().Login(am.Username, am.Password);

                //Showing an exception if the user is not re-loggedin
                if (!model.LoggedIn)
                {
                    TempData["error"] = "Failed to login again - Something went horribly wrong";
                    return RedirectToAction("Index", "Home");
                }

                //Setting cookie with the new password
                var userCookie = new HttpCookie("user");
                userCookie["id"] = model.User.Id + "";
                userCookie["username"] = model.User.Username;
                userCookie["password"] = model.User.Password;
                userCookie.Expires.AddDays(365);
                HttpContext.Response.Cookies.Add(userCookie);

                //Sending the user back to the Account details page
                TempData["success"] = "Your profile was successfully updated";
                return RedirectToAction("Details", new { am.Id });
            }
            catch(Exception e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        //
        // GET: /Account/Create

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            try
            {
                _accountLogic.RegisterAccount(model);
                TempData["success"] = "You user account was successfully created. You can now login";
                return RedirectToAction("Index", "Home");
            }
            catch(Exception e)
            {
                model.Error = e.Message;
                TempData["error"] = e.Message;
                return View(model);
            }
        }
    }
}
