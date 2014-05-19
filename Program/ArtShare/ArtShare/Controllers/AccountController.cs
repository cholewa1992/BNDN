using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ArtShare.Logic;
using ArtShare.Models;
using Microsoft.Ajax.Utilities;
using ShareItServices.UserService;

namespace ArtShare.Controllers
{
    /// <author>
    /// Mathias Pedersen (mkin@itu.dk)
    /// </author>
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
                string username = "";
                string password = "";
                int userId = 0;
                //Checking that the user is logged in
                if (Request.Cookies["user"] != null)
                {
                    username = Request.Cookies["user"].Values["username"];
                    password = Request.Cookies["user"].Values["password"];
                    //userId = int.Parse(Request.Cookies["user"].Values["id"]);
                }

                var model = _accountLogic.GetAccountInformation(username, password, id);

                //Fetching and passing Account model to view
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Edit(int id)
        {
            //Checking if a user is logged in
            if (Request.Cookies["user"] == null)
            {
                TempData["error"] = "You have to login";
                return RedirectToAction("Index", "Home");
            }

            if (_accountLogic.HasRightToEdit(Request.Cookies["user"].Values["username"], Request.Cookies["user"].Values["password"], id)) 
            { 
                return Details(id); 
            }
            else
            {
                TempData["error"] = "You don't have access to this action";
                return RedirectToAction("Details", "Account", new { id });
            }
        }

        [HttpPost]
        public ActionResult Edit(AccountModel am)
        {
            try
            {
                //Checking if a user is logged in
                if (Request.Cookies["user"] == null)
                {
                    TempData["error"] = "You have to login to edit your profil page";
                    return RedirectToAction("Index", "Home");
                }

                var requestingId = int.Parse(Request.Cookies["user"].Values["id"]);
                var requestingUsername = Request.Cookies["user"].Values["username"];
                var requestingPassword = Request.Cookies["user"].Values["password"];


                //Checking if the user editing is the user being edited
                //NOTE: This restricting is only here beacuse it's not possbile for an admin to edit a user with the current backend
                if (_accountLogic.IsUserAdmin(requestingUsername, requestingPassword, requestingId) || requestingUsername == am.Username)
                {
                    if (Request.Cookies["user"].Values["password"] != am.CurrentPassword && !_accountLogic.IsUserAdmin(requestingUsername, requestingPassword, requestingId))
                    {
                        TempData["error"] = "The current password provided was not correct";
                        return RedirectToAction("Edit", "Details", new { am.Id });
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
                    userCookie.Expires = DateTime.Now.AddDays(365);
                    HttpContext.Response.Cookies.Add(userCookie);

                    //Sending the user back to the Account details page
                    TempData["success"] = "Your profile was successfully updated";
                    return RedirectToAction("Details", new { am.Id });
                }
                else
                {
                    TempData["error"] = "A user can only edit his own account";
                    return RedirectToAction("Details", new { am.Id });
                }
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
            TempData["redirectTo"] = TempData["redirectTo"];
            if (Request.Cookies["user"] != null)
                return RedirectToAction("Details", new {Id = Request.Cookies["user"].Values["id"]});
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
                TempData["success"] = "You user account was successfully created. Thank you for joining ArtShare.";
                var login = new LoginLogic().Login(model.Username, model.Password);

                if (login.LoggedIn)
                {
                    var userCookie = new HttpCookie("user");
                    userCookie["id"] = login.User.Id + "";
                    userCookie["username"] = login.User.Username;
                    userCookie["password"] = login.User.Password;
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
            catch(Exception e)
            {
                model.Error = e.Message;
                TempData["error"] = e.Message;
                return View(model);
            }
        }

        public ActionResult UserList()
        {
            string username = "";
            string password = "";
            if (Request.Cookies["user"] != null)
            {
                username = Request.Cookies["user"].Values["username"];
                password = Request.Cookies["user"].Values["password"];
            }
            else
            {
                TempData["error"] = "An admin must be logged in to get a list of all users";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                return View(_accountLogic.GetAllUsers(username, password));
            }
            catch (ArgumentNullException e)
            {
                TempData["error"] = "An admin must be logged in to get a list of all users - Internal";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DeleteUser(int id)
        {
            string username = "";
            string password = "";
            if (Request.Cookies["user"] != null)
            {
                username = Request.Cookies["user"].Values["username"];
                password = Request.Cookies["user"].Values["password"];
            }
            else
            {
                TempData["error"] = "An admin must be logged in to delete a user";
                return RedirectToAction("Index", "Login");
            }
            try
            {
                _accountLogic.DeleteAccount(username, password, id);
                TempData["success"] = "Account with id " + id + " was deleted";
                return RedirectToAction("UserList", "Account");
            }
            catch (ArgumentNullException e)
            {
                TempData["error"] = "An admin must be logged in to get a delete a user";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
