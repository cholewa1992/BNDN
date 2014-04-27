using System;
using System.Collections.Generic;
using System.IO;
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

        public ActionResult Index(int id)
        {

            //Checking that the user is logged in
            var userCookie = Request.Cookies["user"];
            if (userCookie == null)
            {
                TempData["error"] = "You have to be logged in to download files.";
                return RedirectToAction("Index", "Home");
            }

            var user = new UserDTO() { Username = userCookie["username"], Password = userCookie["password"] };
            string fileName = TempData["fileName"] as string;
            try
            {
                string fileExtension;
                var stream = _logic.DownloadFile(user, id, out fileExtension);
                fileName = fileName + fileExtension;
                return File(stream, MimeMapping.GetMimeMapping(fileName), fileName);
            }
            catch (Exception e)
            {
                TempData["error"] = e;
                return RedirectToAction("Index", "Details", new {id = id});
            }
            
        }

    }
}
