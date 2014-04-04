using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using TestClient.UserService;

namespace ArtShare.Controllers
{
    public class AccountController : Controller
    {


        private string token = "7dac496c534911c0ef47bce1de772502b0d6a6c60b1dbd73c1d3f285f36a0f61";


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

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Account/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {

            UserDTO user;

            try
            {
                user = PassUserInformation(collection);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Error passing user to update in edit; " + e.Message);
            }

            try
            {
                using (var us = new UserServiceClient())
                {
                    us.CreateAccount(user, token);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        

        //
        // POST: /Account/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var requestingUser = new UserDTO();
            UserDTO userToUpdate;

            try
            {
                requestingUser.Username = collection["reqUsername"];
                requestingUser.Password = collection["reqUserPassword"];
            }
            catch (Exception e)
            {
                throw new ArgumentException("Error passing requesting user in edit; " + e.Message);
            }

            try
            {
                userToUpdate = PassUserInformation(collection);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Error passing user to update in edit; " + e.Message);
            }


            try
            {

                using (var us = new UserServiceClient())
                {
                    us.UpdateAccounInformation(requestingUser, userToUpdate, token);
                }

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
        public ActionResult Delete(int id, FormCollection collection)
        {

            var requestingUser = new UserDTO();

            try
            {
                requestingUser.Username = collection["reqUsername"];
                requestingUser.Password = collection["reqUserPassword"];
            }
            catch (Exception e)
            {
                throw new ArgumentException("Error passing requesting user in edit; " + e.Message);
            }


            try
            {

                using (var us = new UserServiceClient())
                {
                    us.DeleteAccount(requestingUser, id, token);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public UserDTO PassUserInformation(FormCollection collection)
        {
            return new UserDTO()
            {
                Username = collection["username"],
                Password = collection["password"]
            };
        }
    }
}
