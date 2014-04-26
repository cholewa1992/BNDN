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

        private s accountLogic;

        public AccountController()
        {
            accountLogic = new AccountLogic();
        }

        public AccountController(IAccountLogic logic)
        {
            accountLogic = logic;
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

            var modelStub = new AccountModel
            {
                Id = 29,
                Username = "9cki",
                Email = "nhjo@itu.dk",
                Firstname = "Nicki",
                Lastname = "Jørgensen",
                Location = "The Univers",
            };

            TempData["model"] = modelStub;

            return View(modelStub);
        }

        public ActionResult Edit(int id)
        {
            var model = TempData["model"];
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AccountModel am)
        {
            // You get the new model, now edit it!
            TempData["success"] = "You have edited something";
            return RedirectToAction("Details", new { am.Id } );
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
                accountLogic.RegisterAccount(model);

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
