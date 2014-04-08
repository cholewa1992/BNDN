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


        

        private IAccountLogic accountLogic;

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
            return View();
        }

        //
        // GET: /Account/Details/5

        public ActionResult Details(int id)
        {
            return View();
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

                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                model.Error = e.Message;
                return View(model);
            }
        }

        

        //
        // POST: /Account/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            
            try
            {

                //TODO implement

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       

        //
        // POST: /Account/Delete/5

        [HttpPost]
        public ActionResult Delete(int id)
        {

            try
            {
                accountLogic.DeleteAccount(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


    }
}
