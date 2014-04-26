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

        [HttpPost]
        public ActionResult Upload(UploadModel um)
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
                        RedirectToAction("Index", "Login");
                    }

                    result = _logic.UploadFile(um, user);
                }
                catch (FaultException e)
                {
                    TempData["Error"] = e.Message;
                    RedirectToAction("Index");
                }
                if (result > 0)
                    RedirectToAction("Index", "Details", new {result});
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
