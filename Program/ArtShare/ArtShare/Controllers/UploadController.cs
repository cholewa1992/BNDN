using System.ServiceModel;
using System.Web.Routing;
using ArtShare.Logic;
using ArtShare.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
                try
                {
                    var userCookie = Request.Cookies["user"];
                    var user = new UserDTO();
                    if (userCookie != null)
                    {
                        user.Username = userCookie["username"];
                        user.Password = userCookie["password"];
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }

                    result = _logic.UploadFile(um, user, um.Details);
                }
                catch (FaultException e)
                {
                    TempData["Error"] = e.Message;
                    return RedirectToAction("Index");
                }
                if (result > 0)
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
                try
                {
                    var userCookie = Request.Cookies["user"];
                    var user = new UserDTO();
                    if (userCookie != null)
                    {
                        user.Username = userCookie["username"];
                        user.Password = userCookie["password"];
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }

                    result = _logic.UploadFile(um, user, um.Details);
                }
                catch (FaultException e)
                {
                    TempData["Error"] = e.Message;
                    return RedirectToAction("Index");
                }
                if (result > 0)
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
                try
                {
                    var userCookie = Request.Cookies["user"];
                    var user = new UserDTO();
                    if (userCookie != null)
                    {
                        user.Username = userCookie["username"];
                        user.Password = userCookie["password"];
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }

                    result = _logic.UploadFile(um, user, um.Details);
                }
                catch (FaultException e)
                {
                    TempData["Error"] = e.Message;
                    return RedirectToAction("Index");
                }
                if (result > 0)
                    return RedirectToAction("Index", "Details", new { id = result });
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
