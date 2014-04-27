using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtShare.Logic;
using ShareItServices.TransferService;

namespace ArtShare.Controllers
{
    public class DownloadController : Controller
    {

        private ITransferLogic _logic;

        public DownloadController()
        {
            _logic = new TransferLogic();
        }

        public DownloadController(ITransferLogic logic)
        {
            _logic = logic;
        }


        //
        // GET: /Download/5

        public ActionResult Index(int mediaItem, string fileName)
        {

            //Checking that the user is logged in
            if (Request.Cookies["user"] == null)
            {
                TempData["error"] = "You have to login to see you profil page";
                return RedirectToAction("Index", "Home");
            }

            var user = new UserDTO() { Username = Request.Cookies["user"].Values["username"], Password = Request.Cookies["user"].Values["password"] };

            var model = _logic.DownloadFile(user, mediaItem);

            var resp = HttpContext.Response;

            resp.ContentType = "application/octet-stream";
            resp.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            resp.AddHeader("Content-Length", model.Stream.Length.ToString());

            return View();
        }

    }
}
