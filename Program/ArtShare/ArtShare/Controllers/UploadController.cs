using System.Configuration;
using System.ServiceModel;
using System.Web.Configuration;
using System.Web.Routing;
using ArtShare.Logic;
using ArtShare.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtShare.Properties;
using ShareItServices.TransferService;

namespace ArtShare.Controllers
{
    public class UploadController : Controller
    {
        private ITransferLogic _logic;

        public UploadController()
        {
            _logic = new TransferLogic();
        }

        public UploadController(ITransferLogic logic)
        {
            _logic = logic;
        }
        //
        // GET: /Upload/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Movie()
        {
            return View();
        }

        public ActionResult Music()
        {
            return View();
        }

        public ActionResult Book()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadMovie(UploadMovieModel um)
        {
            if (ModelState.IsValid)
            {
                int result = 0;
                var userCookie = Request.Cookies["user"];
                if (userCookie == null)
                    return RedirectToAction("Index", "Login");
                
                var user = new UserDTO();
                user.Username = userCookie["username"];
                user.Password = userCookie["password"];

                try
                {
                    result = _logic.UploadFile(um, user, um.Details);
                }
                catch (FaultException e)
                {
                    TempData["error"] = e.Message;
                    return RedirectToAction("Index");
                }
                if (result > 0)
                    TempData["success"] = "Congratulations, your upload was a success. You have been redirected to the newly created details page!";
                    return RedirectToAction("Index", "Details", new {id = result});
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult UploadMusic(UploadMusicModel um)
        {
            if (ModelState.IsValid)
            {
                int result = 0;
                var userCookie = Request.Cookies["user"];
                if (userCookie == null)
                    return RedirectToAction("Index", "Login");

                var user = new UserDTO();
                user.Username = userCookie["username"];
                user.Password = userCookie["password"];
                try
                {
                    result = _logic.UploadFile(um, user, um.Details);
                    
                }
                catch (FaultException e)
                {
                    TempData["error"] = e.Message;
                    return RedirectToAction("Index");
                }
                if (result > 0)
                    TempData["success"] = "Congratulations, your upload was a success. You have been redirected to the newly created details page!";
                    return RedirectToAction("Index", "Details", new { id = result });
            }

            return RedirectToAction("Index", "Home");
            
        }

        [HttpPost]
        public ActionResult UploadBook(UploadBookModel um)
        {
            if (ModelState.IsValid)
            {
                int result = 0;
                var userCookie = Request.Cookies["user"];
                if (userCookie == null)
                    return RedirectToAction("Index", "Login");

                var user = new UserDTO();
                user.Username = userCookie["username"];
                user.Password = userCookie["password"];

                um.Details.Tags = um.Details.TagsString.Trim().Trim(',').Split(',').ToList();
                um.Details.Genres = um.Details.GenresString.Trim().Trim(',').Split(',').ToList();

                try
                {
                    result = _logic.UploadFile(um, user, um.Details);
                }
                catch (FaultException e)
                {
                    TempData["error"] = e.Message;
                    return RedirectToAction("Index");
                }
                if (result > 0)
                    TempData["success"] = "Congratulations, your upload was a success. You have been redirected to the newly created details page!";
                    return RedirectToAction("Index", "Details", new { id = result });
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
